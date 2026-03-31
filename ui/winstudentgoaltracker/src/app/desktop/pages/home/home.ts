import { Component, computed, effect, inject, OnDestroy, signal } from '@angular/core';
import { NavigationEnd, Router, RouterLink, RouterOutlet } from '@angular/router';
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';
import { Auth } from '../../../shared/services/auth';
import { StudentService } from '../../../shared/services/student.service';
import { StudentCardDto } from '../../../shared/classes/student-card.dto';
import { SidebarNode } from '../../../shared/classes/sidebar-node';
import { SidebarTreeNode } from '../../components/sidebar-tree-node/sidebar-tree-node';
import { HlmSidebarImports, HlmSidebarService } from '@spartan-ng/helm/sidebar';

@Component({
  selector: 'app-home',
  imports: [RouterOutlet, RouterLink, SidebarTreeNode, HlmSidebarImports],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home implements OnDestroy {

  // ************************** Constructor **************************

  constructor() {
    this.loadStudents();

    // Reload the sidebar tree whenever data changes elsewhere.
    let initialized = false;
    effect(() => {
      this.studentService.dataVersion();
      if (initialized) {
        this.loadStudents();
      }
      initialized = true;
    });

    // Auto-expand sidebar nodes to match the current route.
    this.routeSub = this.router.events.pipe(
      filter(e => e instanceof NavigationEnd)
    ).subscribe(() => {
      this.expandToRoute(this.router.url);
    });

    // Patch individual sidebar node labels without a full rebuild.
    this.labelSub = this.studentService.sidebarLabelUpdate$.subscribe(update => {
      this.patchNodeLabel(this.sidebarTree(), update.routerLink, update.label);
    });
  }

  // ************************** Declarations *************************

  protected readonly auth = inject(Auth);
  private readonly router = inject(Router);
  private readonly studentService = inject(StudentService);
  protected readonly sidebarService = inject(HlmSidebarService);
  private readonly routeSub: Subscription;
  private readonly labelSub: Subscription;
  protected readonly sidebarExpanded = computed(() =>
    this.sidebarService.isMobile() ? this.sidebarService.openMobile() : this.sidebarService.open()
  );
  protected readonly sidebarTree = signal<SidebarNode[]>([]);

  // ************************** Properties ***************************

  // ************************ Public Methods *************************

  // ************************ Event Handlers *************************

  onToggleSidebar() {
    this.sidebarService.toggleSidebar();
  }

  // *****************************************************************
  // Logs the user out and sends them back to the login screen.
  // *****************************************************************
  onLogout() {
    this.auth.logout().subscribe();
    this.auth.forceLogout();
  }

  ngOnDestroy() {
    this.routeSub.unsubscribe();
    this.labelSub.unsubscribe();
  }

  // ********************** Support Procedures ***********************

  // *****************************************************************
  // Recursively walks the sidebar tree to find a node whose
  // routerLink matches the given link, and updates its label.
  // *****************************************************************
  private patchNodeLabel(nodes: SidebarNode[], routerLink: string[], label: string): boolean {
    for (const node of nodes) {
      if (node.routerLink && node.routerLink.join('/') === routerLink.join('/')) {
        node.label = label;
        return true;
      }
      if (node.children && this.patchNodeLabel(node.children, routerLink, label)) {
        return true;
      }
    }
    return false;
  }

  // *****************************************************************
  // Loads student list, sorts by identifier, and builds the sidebar
  // tree with lazy-loading callbacks for goals and benchmarks.
  // *****************************************************************
  private loadStudents() {
    this.studentService.getMyStudents().then(data => {
      if (data.success) {
        const sorted = (data.payload || []).sort((a, b) =>
          a.identifier.localeCompare(b.identifier, undefined, { sensitivity: 'base' })
        );
        this.sidebarTree.set(this.buildTree(sorted));
        this.expandToRoute(this.router.url);
      }
    });
  }

  // *****************************************************************
  // Builds the sidebar node tree from a list of students.
  // *****************************************************************
  private buildTree(students: StudentCardDto[]): SidebarNode[] {
    return [{
      label: 'My Students',
      routerLink: ['/students'],
      expanded: true,
      childCount: students.length,
      children: students.map(s => ({
        label: s.identifier,
        routerLink: ['/students', s.studentId],
        childCount: s.goalCount > 0 ? 1 : 0,
        children: s.goalCount > 0 ? [{
          label: 'Goals',
          routerLink: ['/students', s.studentId, 'goals'],
          childCount: s.goalCount,
          loadChildren: () => this.loadGoalNodes(s.studentId),
        }] : undefined,
      })),
    },
    {
      label: 'Reports',
      routerLink: ['/reports'],
    }];
  }

  // *****************************************************************
  // Lazy-loads individual goal nodes for a student. Called when
  // the "Goals" node is expanded for the first time.
  // *****************************************************************
  private async loadGoalNodes(studentId: string): Promise<SidebarNode[]> {
    const result = await this.studentService.getGoalsForStudent(studentId);
    if (!result.success || !result.payload) return [];

    return result.payload.goals.map(goal => ({
      label: goal.category,
      routerLink: ['/students', studentId, 'goals', goal.goalId],
      childCount: 2,
      children: [
        {
          label: goal.progressEventCount > 0 ? `Progress Events (${goal.progressEventCount})` : 'Progress Events',
          routerLink: ['/students', studentId, 'goals', goal.goalId, 'progress'],
        },
        {
          label: 'Benchmarks',
          routerLink: ['/students', studentId, 'goals', goal.goalId, 'benchmarks'],
          childCount: goal.benchmarkCount,
          loadChildren: goal.benchmarkCount > 0
            ? () => this.loadBenchmarkNodes(studentId, goal.goalId)
            : undefined,
        },
      ],
    }));
  }

  // *****************************************************************
  // Lazy-loads benchmark leaf nodes for a goal. Called when a
  // "Benchmarks" node is expanded for the first time.
  // *****************************************************************
  private async loadBenchmarkNodes(studentId: string, goalId: string): Promise<SidebarNode[]> {
    const result = await this.studentService.getBenchmarksForStudent(studentId);
    if (!result.success || !result.payload) return [];

    return result.payload.benchmarks
      .filter(b => b.goalId === goalId)
      .map(b => ({
        label: b.shortName || b.benchmark,
        routerLink: ['/students', studentId, 'goals', goalId, 'benchmarks', b.benchmarkId],
      }));
  }

  // *****************************************************************
  // Walks the sidebar tree and expands any node whose routerLink is
  // a prefix of the current URL. Triggers lazy loading if needed.
  // Returns true if the current URL matches or is a descendant of
  // any node in the given list.
  // *****************************************************************
  private async expandToRoute(url: string, nodes?: SidebarNode[]): Promise<boolean> {
    const tree = nodes || this.sidebarTree();
    let matched = false;

    for (const node of tree) {
      const nodePath = node.routerLink ? node.routerLink.join('/') : '';

      // Check if this node is the target or an ancestor of the target.
      const isMatch = nodePath !== '' && url === nodePath;
      const isAncestor = nodePath !== '' && url.startsWith(nodePath + '/');

      if (isMatch || isAncestor) {
        matched = true;

        if (isAncestor) {
          // Expand this node to reveal children.
          if (node.loadChildren && !node.children) {
            node.children = await node.loadChildren();
          }
          node.expanded = true;

          // Continue down the tree.
          if (node.children) {
            await this.expandToRoute(url, node.children);
          }
        }
      }
    }

    return matched;
  }
}

