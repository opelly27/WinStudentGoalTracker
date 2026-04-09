import { Component, computed, effect, inject, signal, untracked } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { StudentService } from '../../../shared/services/student.service';
import { StudentCardDto } from '../../../shared/classes/student-card.dto';
import { StudentGoalItem } from '../../../shared/classes/student-goal';
import { BenchmarkDto } from '../../../shared/classes/benchmark.dto';
import { StudentFullProfileDto, ProgressEventWithGoalDto, ProgressEventBenchmarkLink } from '../../../shared/classes/student-full-profile.dto';
import { GoalModal } from '../goal-modal/goal-modal';
import { EditBenchmarkModal } from '../edit-benchmark-modal/edit-benchmark-modal';
import { EditEventModal } from '../edit-event-modal/edit-event-modal';

type TabView = 'benchmarks' | 'progress';

@Component({
    selector: 'app-workspace',
    imports: [GoalModal, EditBenchmarkModal, EditEventModal],
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
                this.selectedGoalId.set(null);
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

        // When dataVersion changes and we have a student loaded, refresh.
        // Use untracked() for studentId so this effect only re-runs on
        // dataVersion changes — route params already handle studentId changes.
        let initialized = false;
        effect(() => {
            this.studentService.dataVersion();
            if (initialized) {
                const id = untracked(() => this.studentId());
                if (id) {
                    this.studentService.invalidateProfile(id);
                    this.loadStudentData(id);
                }
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
    protected readonly progressEvents = signal<ProgressEventWithGoalDto[]>([]);
    protected readonly progressEventBenchmarks = signal<ProgressEventBenchmarkLink[]>([]);
    protected readonly selectedGoalId = signal<string | null>(null);
    protected readonly activeTab = signal<TabView>('benchmarks');

    // Modal states
    protected readonly showGoalModal = signal<StudentGoalItem | 'add' | null>(null);
    protected readonly showEditBenchmarkModal = signal<BenchmarkDto | 'new' | null>(null);
    protected readonly showEditEventModal = signal<ProgressEventWithGoalDto | null | 'new'>(null);

    // ************************** Properties ***************************

    protected readonly selectedGoal = computed<StudentGoalItem | null>(() => {
        const id = this.selectedGoalId();
        if (!id) return this.goals().length > 0 ? this.goals()[0] : null;
        return this.goals().find(g => g.goalId === id) ?? null;
    });


    protected readonly goalBenchmarks = computed<BenchmarkDto[]>(() => {
        const goalId = this.selectedGoal()?.goalId;
        if (!goalId) return [];
        return this.benchmarks().filter(b => b.goalId === goalId);
    });

    protected readonly goalProgressEvents = computed<ProgressEventWithGoalDto[]>(() => {
        const goalId = this.selectedGoal()?.goalId;
        if (!goalId) return [];
        return this.progressEvents()
            .filter(e => e.goalId === goalId)
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
        this.router.navigate(['/students', this.studentId(), 'goals', goalId]);
    }

    onTabChange(tab: TabView) {
        this.activeTab.set(tab);
    }

    // Modal handlers
    onEditGoal() {
        this.showGoalModal.set(this.selectedGoal()!);
    }

    onGoalSaved() {
        this.showGoalModal.set(null);
        this.refetchProfile();
    }

    onAddGoal() {
        this.showGoalModal.set('add');
    }

    onGoalCreated(goal: StudentGoalItem) {
        this.showGoalModal.set(null);
        this.studentService.notifyDataChanged();
        this.refetchProfile().then(() => {
            this.selectedGoalId.set(goal.goalId);
        });
    }

    onEditBenchmark(b: BenchmarkDto) {
        this.showEditBenchmarkModal.set(b);
    }

    onEditBenchmarkSaved() {
        this.showEditBenchmarkModal.set(null);
        this.refetchProfile();
    }

    onAddBenchmark() {
        this.showEditBenchmarkModal.set('new');
    }

    onNewEvent() {
        this.showEditEventModal.set('new');
    }

    onEditEvent(ev: ProgressEventWithGoalDto) {
        this.showEditEventModal.set(ev);
    }

    onEventSaved() {
        this.showEditEventModal.set(null);
        this.refetchProfile();
    }

    // *****************************************************************
    // Returns the benchmark IDs associated with a given progress event,
    // read from the cached profile data.
    // *****************************************************************
    getBenchmarkIdsForEvent(progressEventId: string): string[] {
        return this.progressEventBenchmarks()
            .filter(link => link.progressEventId === progressEventId)
            .map(link => link.benchmarkId);
    }

    // ************************ Formatting Helpers **********************

    formatDate(d: string | Date | null): string {
        if (!d) return '';
        const date = new Date(d);
        return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' });
    }

    // ********************** Support Procedures ***********************

    private async loadStudentData(studentId: string) {
        const result = await this.studentService.getFullProfile(studentId);

        if (!result.success || !result.payload) return;

        const profile = result.payload;
        this.student.set(profile.student);
        this.goals.set(profile.goals);
        this.benchmarks.set(profile.benchmarks);
        this.progressEvents.set(profile.progressEvents);
        this.progressEventBenchmarks.set(profile.progressEventBenchmarks);

        // Auto-select first goal if none selected
        if (!this.selectedGoalId() && profile.goals.length > 0) {
            this.selectedGoalId.set(profile.goals[0].goalId);
        }
    }

    private async refetchProfile(): Promise<void> {
        const id = this.studentId();
        if (!id) return;
        this.studentService.invalidateProfile(id);
        await this.loadStudentData(id);
    }
}
