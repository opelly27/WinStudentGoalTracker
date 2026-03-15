import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { describeHttpError } from '../../../shared/classes/http-errors';
import { StudentService } from '../../../shared/services/student.service';

interface BenchmarkCheckItem {
  benchmarkId: string;
  label: string;
  checked: boolean;
}

import { ToggleBenchmark } from '../../components/toggle-benchmark/toggle-benchmark';

@Component({
  selector: 'app-add-progress-event',
  imports: [FormsModule, ToggleBenchmark],
  templateUrl: './add-progress-event.html',
  styleUrl: './add-progress-event.scss',
})
export class AddProgressEvent {

  // ************************** Constructor **************************

  constructor() {
    this.goalCategory.set(this.route.snapshot.queryParamMap.get('goalCategory') ?? '');
    this.studentIdentifier.set(this.route.snapshot.queryParamMap.get('studentIdentifier') ?? '');
    this.studentId = this.route.snapshot.paramMap.get('studentId') ?? '';
    this.goalId = this.route.snapshot.paramMap.get('goalId') ?? '';
    this.loadBenchmarks();
  }

  // ************************** Declarations *************************

  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly studentService = inject(StudentService);

  private readonly studentId: string;
  private readonly goalId: string;

  protected readonly goalCategory = signal('');
  protected readonly studentIdentifier = signal('');
  protected readonly notes = signal('');
  protected readonly error = signal<string | null>(null);
  protected readonly saving = signal(false);
  protected readonly benchmarkItems = signal<BenchmarkCheckItem[]>([]);

  // ************************** Properties ***************************

  // *****************************************************************
  // True when there is content to save.
  // *****************************************************************
  protected readonly canSave = computed(() =>
    this.notes().trim().length > 0 && !this.saving(),
  );

  // ************************ Public Methods *************************

  // ************************ Event Handlers *************************

  // *****************************************************************
  // Navigates back to the student's goal list.
  // *****************************************************************
  onBack() {
    this.router.navigate(['students', this.studentId, 'goals']);
  }

  // *****************************************************************
  // Toggles a benchmark checkbox.
  // *****************************************************************
  onToggleBenchmark(benchmarkId: string) {
    this.benchmarkItems.update(items =>
      items.map(b => b.benchmarkId === benchmarkId ? { ...b, checked: !b.checked } : b)
    );
  }

  // *****************************************************************
  // Saves the progress event with optional benchmark associations.
  // On success, returns to the goal list.
  // *****************************************************************
  async onSave() {
    this.error.set(null);
    this.saving.set(true);

    const checkedIds = this.benchmarkItems()
      .filter(b => b.checked)
      .map(b => b.benchmarkId);

    try {
      const result = await this.studentService.addProgressEvent(
        this.studentId, this.goalId, this.notes().trim(),
        checkedIds.length > 0 ? checkedIds : undefined
      );
      this.saving.set(false);
      if (result.success) {
        this.router.navigate(['students', this.studentId, 'goals']);
      } else {
        this.error.set(result.message);
      }
    } catch (err) {
      this.saving.set(false);
      this.error.set(describeHttpError(err as HttpErrorResponse));
    }
  }

  // ********************** Support Procedures ***********************

  // *****************************************************************
  // Loads benchmarks for the current goal to populate checkboxes.
  // *****************************************************************
  private async loadBenchmarks() {
    const result = await this.studentService.getBenchmarksForStudent(this.studentId);
    if (result.success && result.payload) {
      const goalBenchmarks = result.payload.benchmarks.filter(b => b.goalId === this.goalId);
      this.benchmarkItems.set(goalBenchmarks.map(b => ({
        benchmarkId: b.benchmarkId,
        label: b.shortName || b.benchmark,
        checked: false,
      })));
    }
  }
}
