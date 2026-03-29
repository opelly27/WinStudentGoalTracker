import { Component, inject, signal, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { Subscription } from 'rxjs';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { lucideArrowUp } from '@ng-icons/lucide';
import { HlmButton } from '@spartan-ng/helm/button';
import { HlmIcon } from '@spartan-ng/helm/icon';
import { HlmInput } from '@spartan-ng/helm/input';
import { HlmTextarea } from '@spartan-ng/helm/textarea';
import { StudentService } from '../../../shared/services/student.service';
import { BenchmarkDto } from '../../../shared/classes/benchmark.dto';

interface BenchmarkCheckItem {
  benchmarkId: string;
  label: string;
  checked: boolean;
}

@Component({
  selector: 'app-progress-edit',
  imports: [FormsModule, DatePipe, NgIcon, HlmIcon, HlmButton, HlmInput, HlmTextarea],
  providers: [provideIcons({ lucideArrowUp })],
  templateUrl: './progress-edit.html',
  styleUrl: './progress-edit.scss',
})
export class ProgressEdit implements OnDestroy {

  // ************************** Constructor **************************

  constructor() {
    this.paramSub = this.route.paramMap.subscribe(params => {
      this.studentId = params.get('studentId')!;
      this.goalId = params.get('goalId')!;
      this.progressEventId = params.get('progressEventId') ?? null;
      this.loadData();
    });
  }

  // ************************** Declarations *************************

  private readonly studentService = inject(StudentService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly paramSub: Subscription;

  private studentId!: string;
  private goalId!: string;
  private progressEventId: string | null = null;

  protected readonly loaded = signal(false);
  protected readonly isNew = signal(false);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly successMessage = signal<string | null>(null);
  protected readonly saving = signal(false);

  // Form fields
  protected content = '';
  private savedContent = '';

  // Benchmark checkboxes
  protected benchmarkItems = signal<BenchmarkCheckItem[]>([]);
  private savedBenchmarkSelections: Set<string> = new Set();

  // Read-only metadata
  protected goalCategory = '';
  protected createdByName = '';
  protected createdAt: Date | null = null;

  // ************************** Properties ***************************

  // *****************************************************************
  // Returns true if the form has unsaved changes.
  // *****************************************************************
  hasChanges(): boolean {
    if (this.content !== this.savedContent) return true;
    const current = new Set(this.benchmarkItems().filter(b => b.checked).map(b => b.benchmarkId));
    if (current.size !== this.savedBenchmarkSelections.size) return true;
    for (const id of current) {
      if (!this.savedBenchmarkSelections.has(id)) return true;
    }
    return false;
  }

  // ************************ Public Methods *************************

  // ************************ Event Handlers *************************

  // *****************************************************************
  // Saves the progress event (create or update) and any benchmark
  // associations based on the checked checkboxes.
  // *****************************************************************
  async onSave() {
    this.saving.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    const checkedIds = this.benchmarkItems()
      .filter(b => b.checked)
      .map(b => b.benchmarkId);

    if (this.isNew()) {
      const result = await this.studentService.addProgressEvent(
        this.studentId, this.goalId, this.content.trim(),
        checkedIds.length > 0 ? checkedIds : undefined
      );
      this.saving.set(false);
      if (result.success) {
        this.successMessage.set('Progress event created.');
        this.savedContent = this.content;
        this.savedBenchmarkSelections = new Set(checkedIds);
        this.studentService.notifyDataChanged();
        if (result.payload?.progressEventId) {
          this.router.navigate(['/students', this.studentId, 'goals', this.goalId, 'progress', result.payload.progressEventId]);
        }
      } else {
        this.errorMessage.set(result.message);
      }
    } else {
      const result = await this.studentService.updateProgressEvent(
        this.studentId, this.progressEventId!, this.content.trim(), checkedIds
      );
      this.saving.set(false);
      if (result.success) {
        this.savedContent = this.content;
        this.savedBenchmarkSelections = new Set(checkedIds);
        this.successMessage.set('Changes saved.');
      } else {
        this.errorMessage.set(result.message);
      }
    }
  }

  // *****************************************************************
  // Reverts the form to the last-saved state.
  // *****************************************************************
  onCancel() {
    this.content = this.savedContent;
    this.benchmarkItems.update(items =>
      items.map(b => ({ ...b, checked: this.savedBenchmarkSelections.has(b.benchmarkId) }))
    );
    this.errorMessage.set(null);
    this.successMessage.set(null);
  }

  onBack() {
    this.router.navigate(['/students', this.studentId, 'goals', this.goalId, 'progress']);
  }

  // *****************************************************************
  // Toggles a benchmark checkbox.
  // *****************************************************************
  onToggleBenchmark(benchmarkId: string) {
    this.benchmarkItems.update(items =>
      items.map(b => b.benchmarkId === benchmarkId ? { ...b, checked: !b.checked } : b)
    );
  }

  ngOnDestroy() {
    this.paramSub.unsubscribe();
  }

  // ********************** Support Procedures ***********************

  // *****************************************************************
  // Loads all data needed for the form: goal category, benchmarks,
  // and (for edit mode) the existing event content + associations.
  // *****************************************************************
  private async loadData() {
    this.loaded.set(false);

    // Load goal category
    const goalsResult = await this.studentService.getGoalsForStudent(this.studentId);
    if (goalsResult.success && goalsResult.payload) {
      const goal = goalsResult.payload.goals.find(g => g.goalId === this.goalId);
      this.goalCategory = goal?.category ?? '';
    }

    // Load benchmarks for this goal
    const bmResult = await this.studentService.getBenchmarksForStudent(this.studentId);
    const goalBenchmarks = (bmResult.success && bmResult.payload)
      ? bmResult.payload.benchmarks.filter(b => b.goalId === this.goalId)
      : [];

    if (!this.progressEventId) {
      // New event mode
      this.isNew.set(true);
      this.content = '';
      this.savedContent = '';
      this.savedBenchmarkSelections = new Set();
      this.benchmarkItems.set(goalBenchmarks.map(b => ({
        benchmarkId: b.benchmarkId,
        label: b.shortName || b.benchmark,
        checked: false,
      })));
      this.loaded.set(true);
      return;
    }

    // Edit mode — load existing event
    this.isNew.set(false);
    const eventsResult = await this.studentService.getProgressEventsForGoal(this.goalId);
    if (!eventsResult.success || !eventsResult.payload) {
      this.errorMessage.set(eventsResult.message);
      this.loaded.set(true);
      return;
    }

    const event = eventsResult.payload.find(e => e.progressEventId === this.progressEventId);
    if (!event) {
      this.errorMessage.set('Progress event not found.');
      this.loaded.set(true);
      return;
    }

    this.content = event.content;
    this.savedContent = event.content;
    this.createdByName = event.createdByName;
    this.createdAt = event.createdAt;

    // Load existing benchmark associations
    const assocResult = await this.studentService.getProgressEventBenchmarks(this.progressEventId!);
    const associatedIds = new Set(assocResult.success && assocResult.payload ? assocResult.payload : []);
    this.savedBenchmarkSelections = new Set(associatedIds);

    this.benchmarkItems.set(goalBenchmarks.map(b => ({
      benchmarkId: b.benchmarkId,
      label: b.shortName || b.benchmark,
      checked: associatedIds.has(b.benchmarkId),
    })));

    this.loaded.set(true);
  }
}
