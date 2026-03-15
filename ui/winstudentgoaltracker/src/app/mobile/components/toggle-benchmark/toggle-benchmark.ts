import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-toggle-benchmark',
  imports: [],
  templateUrl: './toggle-benchmark.html',
  styleUrl: './toggle-benchmark.scss',
})
export class ToggleBenchmark {

  // ************************** Declarations *************************

  readonly label = input.required<string>();
  readonly checked = input.required<boolean>();
  readonly toggled = output<void>();

  // ************************ Event Handlers *************************

  onTap() {
    this.toggled.emit();
  }
}
