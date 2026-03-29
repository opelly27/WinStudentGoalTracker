import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { lucideArrowLeft, lucidePlus } from '@ng-icons/lucide';
import { HlmButton } from '@spartan-ng/helm/button';
import { HlmIcon } from '@spartan-ng/helm/icon';
import { BenchmarkDto } from '../../../shared/classes/benchmark.dto';
import { StudentService } from '../../../shared/services/student.service';
import { BenchmarkCard } from '../benchmark-card/benchmark-card';

@Component({
  selector: 'app-benchmark-list',
  imports: [BenchmarkCard, NgIcon, HlmIcon, HlmButton],
  providers: [provideIcons({ lucideArrowLeft, lucidePlus })],
  templateUrl: './benchmark-list.html',
  styleUrl: './benchmark-list.scss',
})
export class BenchmarkList {

  // ************************** Constructor **************************

  constructor() {
    this.studentId = this.route.snapshot.paramMap.get('studentId')!;
    this.goalId = this.route.snapshot.paramMap.get('goalId') || '';
    this.loadBenchmarks();
    this.loadGoalCategory();
  }

  // ************************** Declarations *************************

  private readonly studentService = inject(StudentService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  protected readonly studentId: string;
  protected readonly goalId: string;
  protected readonly goalCategory = signal<string | null>(null);
  protected readonly benchmarks = signal<BenchmarkDto[]>([]);
  protected readonly errorMessage = signal<string | null>(null);

  // ************************** Properties ***************************

  // ************************ Public Methods *************************

  // ************************ Event Handlers *************************

  onAddBenchmark() {
    this.router.navigate(['/students', this.studentId, 'goals', this.goalId, 'benchmarks', 'new']);
  }

  onBack() {
    this.router.navigate(['/students', this.studentId, 'goals']);
  }

  // ********************** Support Procedures ***********************

  // *****************************************************************
  // Loads benchmarks for the student from the service. Also sets
  // goalCategory from the first benchmark's data.
  // *****************************************************************
  private loadBenchmarks() {
    this.studentService.getBenchmarksForStudent(this.studentId).then(data => {
      if (!data.success) {
        this.errorMessage.set(data.message);
      } else {
        const benchmarks = data.payload?.benchmarks ?? [];
        this.benchmarks.set(benchmarks);

        // Set the goal category from benchmark data if available
        if (this.goalId && benchmarks.length > 0) {
          const match = benchmarks.find(b => b.goalId === this.goalId);
          if (match) this.goalCategory.set(match.goalCategory);
        }

        // If we still don't have a category, load it from the goals API
        if (!this.goalCategory()) {
          this.loadGoalCategory();
        }
      }
    });
  }

  // *****************************************************************
  // Loads the goal category from the goals API as a fallback when
  // no benchmarks exist yet to extract it from.
  // *****************************************************************
  private loadGoalCategory() {
    if (!this.goalId) return;
    this.studentService.getGoalsForStudent(this.studentId).then(result => {
      if (!result.success || !result.payload) return;
      const goal = result.payload.goals.find(g => g.goalId === this.goalId);
      this.goalCategory.set(goal?.category ?? null);
    });
  }
}
