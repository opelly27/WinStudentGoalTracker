import { Component, input } from '@angular/core';
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

    readonly goal = input.required<StudentGoalItem>();

    // ************************** Properties ***************************

    // ************************ Public Methods *************************

    // ************************ Event Handlers *************************

    // ********************** Support Procedures ***********************
}
