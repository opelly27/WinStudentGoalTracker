import { Component, inject, input } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { StudentGoalItem } from '../../../shared/classes/student-goal';

@Component({
    selector: 'app-goal-card',
    imports: [],
    templateUrl: './goal-card.html',
    styleUrl: './goal-card.scss',
})
export class GoalCard {

    // ************************** Constructor **************************

    // ************************** Declarations *************************

    private readonly router = inject(Router);
    private readonly route = inject(ActivatedRoute);

    readonly goal = input.required<StudentGoalItem>();

    // ************************** Properties ***************************

    // ************************ Public Methods *************************

    // ************************ Event Handlers *************************

    // *****************************************************************
    // Navigates to the progress events page for this goal.
    // *****************************************************************
    onCardClick() {
        const studentId = this.route.snapshot.paramMap.get('studentId')!;
        this.router.navigate(['/students', studentId, 'goals', this.goal().goalId, 'progress']);
    }

    // ********************** Support Procedures ***********************
}
