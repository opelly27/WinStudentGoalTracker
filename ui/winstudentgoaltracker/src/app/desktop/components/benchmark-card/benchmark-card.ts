import { Component, input } from '@angular/core';
import { DatePipe } from '@angular/common';
import { BenchmarkDto } from '../../../shared/classes/benchmark.dto';

@Component({
  selector: 'app-benchmark-card',
  imports: [DatePipe],
  templateUrl: './benchmark-card.html',
  styleUrl: './benchmark-card.scss',
})
export class BenchmarkCard {

  // ************************** Constructor **************************

  // ************************** Declarations *************************

  readonly benchmark = input.required<BenchmarkDto>();

  // ************************** Properties ***************************

  // ************************ Public Methods *************************

  // ************************ Event Handlers *************************

  // ********************** Support Procedures ***********************
}
