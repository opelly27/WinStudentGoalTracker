import { Component, inject, input } from '@angular/core';
import { Router } from '@angular/router';
import { StudentCardDto } from '../../../shared/classes/student-card.dto';

@Component({
  selector: 'app-student-card',
  imports: [],
  templateUrl: './student-card.html',
  styleUrl: './student-card.scss',
})
export class StudentCard {

  // ************************** Constructor **************************

  // ************************** Declarations *************************

  private readonly router = inject(Router);
  readonly student = input.required<StudentCardDto>();

  // ************************** Properties ***************************

  // ************************ Public Methods *************************

  // ************************ Event Handlers *************************

  // *****************************************************************
  // Navigates to the goals page for this student.
  // *****************************************************************
  onCardClick() {
    this.router.navigate(['students', this.student().studentId, 'goals']);
  }

  // ********************** Support Procedures ***********************
}
