import { Component, computed, input } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { StudentCardDto } from '../../../shared/classes/student-card.dto';

@Component({
  selector: 'app-student-card',
  imports: [DatePipe, RouterLink],
  templateUrl: './student-card.html',
  styleUrl: './student-card.scss',
})
export class StudentCard {

  // ************************** Constructor **************************

  // ************************** Declarations *************************

  readonly student = input.required<StudentCardDto>();

  // ************************** Properties ***************************

  // ************************ Public Methods *************************

  // ************************ Event Handlers *************************

  // ********************** Support Procedures ***********************
}
