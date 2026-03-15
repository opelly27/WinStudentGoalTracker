import { Component, inject, input } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
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

  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  readonly event = input.required<ProgressEventDto>();

  // ************************** Properties ***************************

  // ************************ Public Methods *************************

  // ************************ Event Handlers *************************

  onEdit() {
    this.router.navigate([this.event().progressEventId], { relativeTo: this.route });
  }

  // ********************** Support Procedures ***********************
}
