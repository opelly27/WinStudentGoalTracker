import { Component, input } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ProgressEventDto } from '../../../shared/classes/progress-event.dto';

@Component({
  selector: 'app-progress-item',
  imports: [DatePipe],
  templateUrl: './progress-item.html',
  styleUrl: './progress-item.scss',
})
export class ProgressItem {

  // ************************** Constructor **************************

  // ************************** Declarations *************************

  readonly event = input.required<ProgressEventDto>();

  // ************************** Properties ***************************

  // ************************ Public Methods *************************

  // ************************ Event Handlers *************************

  // ********************** Support Procedures ***********************
}
