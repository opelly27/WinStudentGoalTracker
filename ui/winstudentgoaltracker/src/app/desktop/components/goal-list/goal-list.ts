import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { StudentGoalItem } from '../../../shared/classes/student-goal';
import { StudentService } from '../../../shared/services/student.service';
import { GoalCard } from '../goal-card/goal-card';
import { AddGoalModal } from '../add-goal-modal/add-goal-modal';

@Component({
    selector: 'app-goal-list',
    imports: [GoalCard, AddGoalModal],
    templateUrl: './goal-list.html',
    styleUrl: './goal-list.scss',
})
export class GoalList {

    // ************************** Constructor **************************

    constructor() {
        this.studentId = this.route.snapshot.paramMap.get('studentId')!;
        this.loadGoals();
    }

    // ************************** Declarations *************************

    private readonly studentService = inject(StudentService);
    private readonly route = inject(ActivatedRoute);
    private readonly router = inject(Router);

    protected readonly studentId: string;
    protected readonly studentIdentifier = signal<string | null>(null);
    protected readonly goals = signal<StudentGoalItem[]>([]);
    protected readonly showAddModal = signal(false);
    protected readonly errorMessage = signal<string | null>(null);

    // ************************** Properties ***************************

    // ************************ Public Methods *************************

    // ************************ Event Handlers *************************

    onAddGoal() {
        this.showAddModal.set(true);
    }

    onGoalCreated(goal: StudentGoalItem) {
        this.goals.update(list => [...list, goal]);
        this.showAddModal.set(false);
    }

    onModalCancelled() {
        this.showAddModal.set(false);
    }

    onBack() {
        this.router.navigate(['/students']);
    }

    // ********************** Support Procedures ***********************

    // *****************************************************************
    // Loads goals for the student from the service.
    // *****************************************************************
    private loadGoals() {
        this.studentService.getGoalsForStudent(this.studentId).then(data => {
            if (!data.success) {
                this.errorMessage.set(data.message);
            } else {
                this.studentIdentifier.set(data.payload?.studentIdentifier ?? null);
                this.goals.set(data.payload?.goals ?? []);
            }
        });
    }
}
