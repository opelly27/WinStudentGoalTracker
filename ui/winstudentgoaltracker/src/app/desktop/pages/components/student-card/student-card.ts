import { Component, input } from '@angular/core';
import { DatePipe } from '@angular/common';
import { StudentCardDto } from '../../../../shared/models/dto/student-card.dto';

@Component({
  selector: 'app-student-card',
  imports: [DatePipe],
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
