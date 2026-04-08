import { Component, computed, effect, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { StudentService } from '../../../shared/services/student.service';
import { StudentCardDto } from '../../../shared/classes/student-card.dto';
import { StudentGoalItem } from '../../../shared/classes/student-goal';
import { BenchmarkDto } from '../../../shared/classes/benchmark.dto';
import { ProgressEventDto } from '../../../shared/classes/progress-event.dto';
import { getCategoryColor, CategoryColor } from '../../../shared/classes/category-colors';
import { EditGoalModal } from '../edit-goal-modal/edit-goal-modal';
import { EditBenchmarkModal } from '../edit-benchmark-modal/edit-benchmark-modal';
import { EditEventModal } from '../edit-event-modal/edit-event-modal';
import { AddGoalModal } from '../add-goal-modal/add-goal-modal';

type TabView = 'benchmarks' | 'progress';

@Component({
    selector: 'app-workspace',
    imports: [EditGoalModal, EditBenchmarkModal, EditEventModal, AddGoalModal],
    templateUrl: './workspace.html',
    styleUrl: './workspace.scss',
})
export class Workspace {

    // ************************** Constructor **************************

    constructor() {
        // React to route param changes
        this.route.paramMap.subscribe(params => {
            const studentId = params.get('studentId');
            const goalId = params.get('goalId');

            if (studentId && studentId !== this.studentId()) {
                this.studentId.set(studentId);
                this.loadStudentData(studentId);
            } else if (!studentId) {
                this.student.set(null);
                this.studentId.set(null);
            }

            if (goalId) {
                this.selectedGoalId.set(goalId);
            }
        });

        // When dataVersion changes and we have a student loaded, refresh
        let initialized = false;
        effect(() => {
            this.studentService.dataVersion();
            if (initialized && this.studentId()) {
                this.loadStudentData(this.studentId()!);
            }
            initialized = true;
        });
    }

    // ************************** Declarations *************************

    private readonly studentService = inject(StudentService);
    private readonly route = inject(ActivatedRoute);
    private readonly router = inject(Router);

    protected readonly studentId = signal<string | null>(null);
    protected readonly student = signal<StudentCardDto | null>(null);
    protected readonly goals = signal<StudentGoalItem[]>([]);
    protected readonly benchmarks = signal<BenchmarkDto[]>([]);
    protected readonly progressEvents = signal<ProgressEventDto[]>([]);
    protected readonly selectedGoalId = signal<string | null>(null);
    protected readonly activeTab = signal<TabView>('benchmarks');

    // Modal states
    protected readonly showAddGoalModal = signal(false);
    protected readonly showEditGoalModal = signal(false);
    protected readonly showEditBenchmarkModal = signal<BenchmarkDto | null>(null);
    protected readonly showEditEventModal = signal<ProgressEventDto | null | 'new'>(null);

    // ************************** Properties ***************************

    protected readonly selectedGoal = computed<StudentGoalItem | null>(() => {
        const id = this.selectedGoalId();
        if (!id) return this.goals().length > 0 ? this.goals()[0] : null;
        return this.goals().find(g => g.goalId === id) ?? null;
    });

    protected readonly goalColors = computed<CategoryColor>(() => {
        return getCategoryColor(this.selectedGoal()?.category ?? '');
    });

    protected readonly goalBenchmarks = computed<BenchmarkDto[]>(() => {
        const goalId = this.selectedGoal()?.goalId;
        if (!goalId) return [];
        return this.benchmarks().filter(b => b.goalId === goalId);
    });

    protected readonly sortedProgressEvents = computed<ProgressEventDto[]>(() => {
        return [...this.progressEvents()]
            .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
    });

    protected readonly nextIepDate = computed<string>(() => {
        const s = this.student();
        if (!s?.nextIepDate) return '';
        return new Date(s.nextIepDate).toISOString().split('T')[0];
    });

    // ************************ Event Handlers *************************

    onSelectGoal(goalId: string) {
        this.selectedGoalId.set(goalId);
        this.activeTab.set('benchmarks');
        this.loadGoalDetails(goalId);
        this.router.navigate(['/students', this.studentId(), 'goals', goalId]);
    }

    onTabChange(tab: TabView) {
        this.activeTab.set(tab);
    }

    // Modal handlers
    onEditGoal() {
        this.showEditGoalModal.set(true);
    }

    onEditGoalSaved() {
        this.showEditGoalModal.set(false);
        this.loadStudentData(this.studentId()!);
    }

    onAddGoal() {
        this.showAddGoalModal.set(true);
    }

    onGoalCreated(goal: StudentGoalItem) {
        this.showAddGoalModal.set(false);
        this.studentService.notifyDataChanged();
        this.loadStudentData(this.studentId()!).then(() => {
            this.selectedGoalId.set(goal.goalId);
            this.loadGoalDetails(goal.goalId);
        });
    }

    onEditBenchmark(b: BenchmarkDto) {
        this.showEditBenchmarkModal.set(b);
    }

    onEditBenchmarkSaved() {
        this.showEditBenchmarkModal.set(null);
        this.loadStudentData(this.studentId()!);
    }

    onAddBenchmark() {
        // Navigate to the new benchmark route (still uses the old page for creation)
        this.router.navigate(['/students', this.studentId(), 'goals', this.selectedGoal()!.goalId, 'benchmarks', 'new']);
    }

    onNewEvent() {
        this.showEditEventModal.set('new');
    }

    onEditEvent(ev: ProgressEventDto) {
        this.showEditEventModal.set(ev);
    }

    onEventSaved() {
        this.showEditEventModal.set(null);
        if (this.selectedGoal()) {
            this.loadGoalDetails(this.selectedGoal()!.goalId);
        }
    }

    // ************************ Formatting Helpers **********************

    getCatColor(category: string): CategoryColor {
        return getCategoryColor(category);
    }

    formatDate(d: string | Date | null): string {
        if (!d) return '';
        const date = new Date(d);
        return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' });
    }

    truncate(text: string, max: number): string {
        return text.length > max ? text.slice(0, max) + '…' : text;
    }

    getRelatedEventCount(benchmarkId: string): number {
        // We don't have benchmark associations in the event DTO from the list endpoint,
        // so we return 0. The mockup shows this but we can't derive it without extra API calls.
        return 0;
    }

    // ********************** Support Procedures ***********************

    private async loadStudentData(studentId: string) {
        const [studentResult, goalsResult, bmResult] = await Promise.all([
            this.studentService.getStudentById(studentId),
            this.studentService.getGoalsForStudent(studentId),
            this.studentService.getBenchmarksForStudent(studentId),
        ]);

        if (studentResult.success && studentResult.payload) {
            this.student.set(studentResult.payload);
        }

        if (goalsResult.success && goalsResult.payload) {
            this.goals.set(goalsResult.payload.goals);
            // Auto-select first goal if none selected
            if (!this.selectedGoalId() && goalsResult.payload.goals.length > 0) {
                this.selectedGoalId.set(goalsResult.payload.goals[0].goalId);
            }
        }

        if (bmResult.success && bmResult.payload) {
            this.benchmarks.set(bmResult.payload.benchmarks);
        }

        // Load progress events for selected goal
        const goalId = this.selectedGoalId();
        if (goalId) {
            this.loadGoalDetails(goalId);
        }
    }

    private async loadGoalDetails(goalId: string) {
        const result = await this.studentService.getProgressEventsForGoal(goalId);
        if (result.success) {
            this.progressEvents.set(result.payload ?? []);
        }
    }
}
