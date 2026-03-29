import { Component, inject, signal, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { Subscription } from 'rxjs';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { lucideArrowLeft, lucideActivity, lucideListChecks } from '@ng-icons/lucide';
import { HlmButton } from '@spartan-ng/helm/button';
import { HlmIcon } from '@spartan-ng/helm/icon';
import { HlmInput } from '@spartan-ng/helm/input';
import { HlmTextarea } from '@spartan-ng/helm/textarea';
import { StudentService } from '../../../shared/services/student.service';
import { StudentGoalItem } from '../../../shared/classes/student-goal';

@Component({
  selector: 'app-goal-card-full',
  imports: [FormsModule, DatePipe, NgIcon, HlmIcon, HlmButton, HlmInput, HlmTextarea],
  providers: [provideIcons({ lucideArrowLeft, lucideActivity, lucideListChecks })],
  templateUrl: './goal-card-full.html',
  styleUrl: './goal-card-full.scss',
})
export class GoalCardFull implements OnDestroy {

  // ************************** Constructor **************************

  constructor() {
    this.paramSub = this.route.paramMap.subscribe(params => {
      this.studentId = params.get('studentId')!;
      this.goalId = params.get('goalId')!;
      this.loadGoal();
    });
  }

  // ************************** Declarations *************************

  private readonly studentService = inject(StudentService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly paramSub: Subscription;

  private studentId!: string;
  private goalId!: string;

  protected readonly loaded = signal(false);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly successMessage = signal<string | null>(null);
  protected readonly saving = signal(false);

  // Form fields
  protected description = '';
  protected category = '';
  protected baseline = '';
  protected targetCompletionDate: string | null = null;
  protected closeDate: string | null = null;
  protected achieved: boolean | null = null;
  protected closeNotes: string | null = null;

  // Read-only metadata
  protected progressEventCount = 0;
  protected benchmarkCount = 0;

  // Snapshot
  private savedDescription = '';
  private savedCategory = '';
  private savedBaseline = '';
  private savedTargetCompletionDate: string | null = null;
  private savedCloseDate: string | null = null;
  private savedAchieved: boolean | null = null;
  private savedCloseNotes: string | null = null;

  // ************************** Properties ***************************

  // *****************************************************************
  // Returns true if form values differ from the saved snapshot.
  // *****************************************************************
  hasChanges(): boolean {
    return this.description !== this.savedDescription
      || this.category !== this.savedCategory
      || this.baseline !== this.savedBaseline
      || this.targetCompletionDate !== this.savedTargetCompletionDate
      || this.closeDate !== this.savedCloseDate
      || this.achieved !== this.savedAchieved
      || this.closeNotes !== this.savedCloseNotes;
  }

  // ************************ Public Methods *************************

  // ************************ Event Handlers *************************

  // *****************************************************************
  // Saves changes to the goal via the API.
  // *****************************************************************
  async onSave() {
    this.saving.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    const result = await this.studentService.updateGoal(this.studentId, this.goalId, {
      description: this.description,
      category: this.category,
      baseline: this.baseline,
      targetCompletionDate: this.targetCompletionDate,
      closeDate: this.closeDate,
      achieved: this.achieved,
      closeNotes: this.closeNotes,
    });

    this.saving.set(false);

    if (result.success) {
      this.savedDescription = this.description;
      this.savedCategory = this.category;
      this.savedBaseline = this.baseline;
      this.savedTargetCompletionDate = this.targetCompletionDate;
      this.savedCloseDate = this.closeDate;
      this.savedAchieved = this.achieved;
      this.savedCloseNotes = this.closeNotes;
      this.successMessage.set('Changes saved.');
      this.studentService.notifyDataChanged();
    } else {
      this.errorMessage.set(result.message);
    }
  }

  // *****************************************************************
  // Reverts form fields to the last-saved snapshot.
  // *****************************************************************
  onCancel() {
    this.description = this.savedDescription;
    this.category = this.savedCategory;
    this.baseline = this.savedBaseline;
    this.targetCompletionDate = this.savedTargetCompletionDate;
    this.closeDate = this.savedCloseDate;
    this.achieved = this.savedAchieved;
    this.closeNotes = this.savedCloseNotes;
    this.errorMessage.set(null);
    this.successMessage.set(null);
  }

  onBack() {
    this.router.navigate(['/students', this.studentId, 'goals']);
  }

  onProgressEvents() {
    this.router.navigate(['/students', this.studentId, 'goals', this.goalId, 'progress']);
  }

  onBenchmarks() {
    this.router.navigate(['/students', this.studentId, 'goals', this.goalId, 'benchmarks']);
  }

  ngOnDestroy() {
    this.paramSub.unsubscribe();
  }

  // ********************** Support Procedures ***********************

  // *****************************************************************
  // Normalizes an API date string to YYYY-MM-DD for <input type="date">.
  // *****************************************************************
  private toDateInput(value: string | null): string | null {
    if (!value) return null;
    return value.substring(0, 10);
  }

  // *****************************************************************
  // Loads the goal by finding it in the student's goal list.
  // *****************************************************************
  private loadGoal() {
    this.loaded.set(false);
    this.studentService.getGoalsForStudent(this.studentId).then(result => {
      if (!result.success || !result.payload) {
        this.errorMessage.set(result.message);
        return;
      }

      const goal = result.payload.goals.find(g => g.goalId === this.goalId);
      if (!goal) {
        this.errorMessage.set('Goal not found.');
        return;
      }

      this.description = goal.description;
      this.category = goal.category;
      this.baseline = goal.baseline;
      this.targetCompletionDate = this.toDateInput(goal.targetCompletionDate);
      this.closeDate = this.toDateInput(goal.closeDate);
      this.achieved = goal.achieved;
      this.closeNotes = goal.closeNotes;
      this.progressEventCount = goal.progressEventCount;
      this.benchmarkCount = goal.benchmarkCount;

      this.savedDescription = goal.description;
      this.savedCategory = goal.category;
      this.savedBaseline = goal.baseline;
      this.savedTargetCompletionDate = this.toDateInput(goal.targetCompletionDate);
      this.savedCloseDate = this.toDateInput(goal.closeDate);
      this.savedAchieved = goal.achieved;
      this.savedCloseNotes = goal.closeNotes;
      this.loaded.set(true);
    });
  }
}
