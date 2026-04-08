import { Component, effect, inject, OnDestroy, signal } from '@angular/core';
import { NavigationEnd, Router, RouterLink, RouterOutlet } from '@angular/router';
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';
import { Auth } from '../../../shared/services/auth';
import { StudentService } from '../../../shared/services/student.service';
import { StudentCardDto } from '../../../shared/classes/student-card.dto';
import { SidebarNode } from '../../../shared/classes/sidebar-node';
import { SidebarTreeNode } from '../../components/sidebar-tree-node/sidebar-tree-node';

@Component({
  selector: 'app-home',
  imports: [RouterOutlet, RouterLink, SidebarTreeNode],
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
  private readonly routeSub: Subscription;
  private readonly labelSub: Subscription;
  protected readonly sidebarExpanded = signal(true);
  protected readonly sidebarTree = signal<SidebarNode[]>([]);

  // ************************** Properties ***************************

  // ************************ Public Methods *************************

  // ************************ Event Handlers *************************

  onToggleSidebar() {
    this.sidebarExpanded.update(v => !v);
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
  // When scope is 'all', groups students by owning user.
  // *****************************************************************
  private loadStudents() {
    // Fetch with 'all' scope so the sidebar can show grouped nodes
    // when the StudentCardList toggle is active.
    this.studentService.getMyStudents('all').then(data => {
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
  // Groups students by ownerName — "My Students" first, then other
  // owners' groups sorted alphabetically.
  // *****************************************************************
  private buildTree(students: StudentCardDto[]): SidebarNode[] {
    // Group students by ownerName. Students with isMine go into "My Students".
    const myStudents: StudentCardDto[] = [];
    const otherGroups = new Map<string, StudentCardDto[]>();

    for (const s of students) {
      if (s.isMine !== false) {
        myStudents.push(s);
      } else {
        const key = s.ownerName ?? 'Unknown';
        if (!otherGroups.has(key)) otherGroups.set(key, []);
        otherGroups.get(key)!.push(s);
      }
    }

    const nodes: SidebarNode[] = [{
      label: 'My Students',
      routerLink: ['/students'],
      expanded: true,
      childCount: myStudents.length,
      children: myStudents.map(s => this.buildStudentNode(s)),
    }];

    // Add other users' groups sorted by owner name.
    const sortedOwners = [...otherGroups.keys()].sort((a, b) =>
      a.localeCompare(b, undefined, { sensitivity: 'base' })
    );

    for (const ownerName of sortedOwners) {
      const group = otherGroups.get(ownerName)!;
      const firstName = ownerName.split(' ')[0];
      nodes.push({
        label: `${firstName}'s Students`,
        routerLink: ['/students'],
        expanded: false,
        childCount: group.length,
        children: group.map(s => this.buildStudentNode(s)),
      });
    }

    nodes.push({
      label: 'Reports',
      routerLink: ['/reports'],
    });

    return nodes;
  }

  // *****************************************************************
  // Builds a single student sidebar node with lazy-loaded goal
  // children.
  // *****************************************************************
  private buildStudentNode(s: StudentCardDto): SidebarNode {
    return {
      label: s.identifier,
      routerLink: ['/students', s.studentId],
      childCount: s.goalCount > 0 ? 1 : 0,
      children: s.goalCount > 0 ? [{
        label: 'Goals',
        routerLink: ['/students', s.studentId, 'goals'],
        childCount: s.goalCount,
        loadChildren: () => this.loadGoalNodes(s.studentId),
      }] : undefined,
    };
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

