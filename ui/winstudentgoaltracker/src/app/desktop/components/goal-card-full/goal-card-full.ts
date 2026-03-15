import { Component, inject, signal, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { StudentService } from '../../../shared/services/student.service';
import { StudentGoalItem } from '../../../shared/classes/student-goal';

@Component({
  selector: 'app-goal-card-full',
  imports: [FormsModule],
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

  // Read-only metadata
  protected progressEventCount = 0;
  protected benchmarkCount = 0;

  // Snapshot
  private savedDescription = '';
  private savedCategory = '';
  private savedBaseline = '';

  // ************************** Properties ***************************

  // *****************************************************************
  // Returns true if form values differ from the saved snapshot.
  // *****************************************************************
  hasChanges(): boolean {
    return this.description !== this.savedDescription
      || this.category !== this.savedCategory
      || this.baseline !== this.savedBaseline;
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
    });

    this.saving.set(false);

    if (result.success) {
      this.savedDescription = this.description;
      this.savedCategory = this.category;
      this.savedBaseline = this.baseline;
      this.successMessage.set('Changes saved.');
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
      this.progressEventCount = goal.progressEventCount;
      this.benchmarkCount = goal.benchmarkCount;

      this.savedDescription = goal.description;
      this.savedCategory = goal.category;
      this.savedBaseline = goal.baseline;
      this.loaded.set(true);
    });
  }
}
