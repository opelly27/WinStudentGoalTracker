import { Component, inject, input } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { StudentGoalItem } from '../../../shared/classes/student-goal';

@Component({
    selector: 'app-goal-card',
    imports: [DatePipe],
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
    // Navigates to the goal detail page.
    // *****************************************************************
    onCardClick() {
        const studentId = this.route.snapshot.paramMap.get('studentId')!;
        this.router.navigate(['/students', studentId, 'goals', this.goal().goalId]);
    }

    // *****************************************************************
    // Navigates to the benchmarks page for this goal.
    // *****************************************************************
    onBenchmarksClick() {
        const studentId = this.route.snapshot.paramMap.get('studentId')!;
        this.router.navigate(['/students', studentId, 'goals', this.goal().goalId, 'benchmarks']);
    }

    // *****************************************************************
    // Navigates to the progress events page for this goal.
    // *****************************************************************
    onProgressEventsClick() {
        const studentId = this.route.snapshot.paramMap.get('studentId')!;
        this.router.navigate(['/students', studentId, 'goals', this.goal().goalId, 'progress']);
    }

    // ********************** Support Procedures ***********************
}
