import { Component, inject, signal, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { Subscription } from 'rxjs';
import { StudentService } from '../../../shared/services/student.service';
import { BenchmarkDto } from '../../../shared/classes/benchmark.dto';

@Component({
  selector: 'app-benchmark-card-full',
  imports: [FormsModule, DatePipe],
  templateUrl: './benchmark-card-full.html',
  styleUrl: './benchmark-card-full.scss',
})
export class BenchmarkCardFull implements OnDestroy {

  // ************************** Constructor **************************

  constructor() {
    this.paramSub = this.route.paramMap.subscribe(params => {
      this.studentId = params.get('studentId')!;
      this.goalId = params.get('goalId')!;
      this.benchmarkId = params.get('benchmarkId') ?? null;
      this.loadBenchmark();
    });
  }

  // ************************** Declarations *************************

  private readonly studentService = inject(StudentService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly paramSub: Subscription;

  private studentId!: string;
  private goalId!: string;
  private benchmarkId: string | null = null;

  protected readonly loaded = signal(false);
  protected readonly isNew = signal(false);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly successMessage = signal<string | null>(null);
  protected readonly saving = signal(false);

  // Form fields
  protected benchmarkText = '';
  protected shortName = '';
  private savedBenchmarkText = '';
  private savedShortName = '';

  // Read-only metadata
  protected goalCategory = '';
  protected createdByName = '';
  protected createdAt: Date | null = null;
  protected updatedAt: Date | null = null;

  // ************************** Properties ***************************

  // *****************************************************************
  // Returns true if the benchmark text has unsaved changes.
  // *****************************************************************
  hasChanges(): boolean {
    return this.benchmarkText !== this.savedBenchmarkText
      || this.shortName !== this.savedShortName;
  }

  // ************************ Public Methods *************************

  // ************************ Event Handlers *************************

  // *****************************************************************
  // Saves changes or creates a new benchmark.
  // *****************************************************************
  async onSave() {
    this.saving.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    if (this.isNew()) {
      const result = await this.studentService.createBenchmark(this.studentId, {
        goalId: this.goalId,
        benchmark: this.benchmarkText,
        shortName: this.shortName || undefined,
      });
      this.saving.set(false);
      if (result.success) {
        this.successMessage.set('Benchmark created.');
        this.savedBenchmarkText = this.benchmarkText;
        this.savedShortName = this.shortName;
        this.studentService.notifyDataChanged();
        if (result.payload?.benchmarkId) {
          this.router.navigate(['/students', this.studentId, 'goals', this.goalId, 'benchmarks', result.payload.benchmarkId]);
        }
      } else {
        this.errorMessage.set(result.message);
      }
    } else {
      const result = await this.studentService.updateBenchmark(this.studentId, this.benchmarkId!, this.benchmarkText, this.shortName || undefined);
      this.saving.set(false);
      if (result.success) {
        this.savedBenchmarkText = this.benchmarkText;
        this.savedShortName = this.shortName;
        this.successMessage.set('Changes saved.');
      } else {
        this.errorMessage.set(result.message);
      }
    }
  }

  // *****************************************************************
  // Reverts the benchmark text to the last-saved value.
  // *****************************************************************
  onCancel() {
    this.benchmarkText = this.savedBenchmarkText;
    this.shortName = this.savedShortName;
    this.errorMessage.set(null);
    this.successMessage.set(null);
  }

  onBack() {
    this.router.navigate(['/students', this.studentId, 'goals', this.goalId]);
  }

  ngOnDestroy() {
    this.paramSub.unsubscribe();
  }

  // ********************** Support Procedures ***********************

  // *****************************************************************
  // Loads existing benchmark data or sets up new-benchmark state.
  // *****************************************************************
  private loadBenchmark() {
    if (!this.benchmarkId) {
      this.isNew.set(true);
      this.benchmarkText = '';
      this.shortName = '';
      this.savedBenchmarkText = '';
      this.savedShortName = '';
      this.loadGoalCategory();
      this.loaded.set(true);
      return;
    }

    this.isNew.set(false);
    this.studentService.getBenchmarksForStudent(this.studentId).then(result => {
      if (!result.success || !result.payload) {
        this.errorMessage.set(result.message);
        return;
      }

      const bm = result.payload.benchmarks.find(b => b.benchmarkId === this.benchmarkId);
      if (!bm) {
        this.errorMessage.set('Benchmark not found.');
        return;
      }

      this.benchmarkText = bm.benchmark;
      this.shortName = bm.shortName ?? '';
      this.savedBenchmarkText = bm.benchmark;
      this.savedShortName = bm.shortName ?? '';
      this.goalCategory = bm.goalCategory;
      this.createdByName = bm.createdByName;
      this.createdAt = bm.createdAt;
      this.updatedAt = bm.updatedAt;
      this.loaded.set(true);
    });
  }

  // *****************************************************************
  // Loads the goal category for a new benchmark.
  // *****************************************************************
  private loadGoalCategory() {
    this.studentService.getGoalsForStudent(this.studentId).then(result => {
      if (result.success && result.payload) {
        const goal = result.payload.goals.find(g => g.goalId === this.goalId);
        this.goalCategory = goal?.category ?? '';
      }
    });
  }
}
