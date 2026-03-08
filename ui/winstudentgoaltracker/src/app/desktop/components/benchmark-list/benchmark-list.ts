import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BenchmarkDto } from '../../../shared/classes/benchmark.dto';
import { StudentService } from '../../../shared/services/student.service';
import { BenchmarkCard } from '../benchmark-card/benchmark-card';

@Component({
  selector: 'app-benchmark-list',
  imports: [BenchmarkCard],
  templateUrl: './benchmark-list.html',
  styleUrl: './benchmark-list.scss',
})
export class BenchmarkList {

  // ************************** Constructor **************************

  constructor() {
    this.studentId = this.route.snapshot.paramMap.get('studentId')!;
    this.goalId = this.route.snapshot.paramMap.get('goalId') || '';
    this.loadBenchmarks();
  }

  // ************************** Declarations *************************

  private readonly studentService = inject(StudentService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  protected readonly studentId: string;
  protected readonly goalId: string;
  protected readonly studentIdentifier = signal<string | null>(null);
  protected readonly benchmarks = signal<BenchmarkDto[]>([]);
  protected readonly errorMessage = signal<string | null>(null);

  // ************************** Properties ***************************

  // ************************ Public Methods *************************

  // ************************ Event Handlers *************************

  onAddBenchmark() {
    this.router.navigate(['/students', this.studentId, 'goals', this.goalId, 'benchmarks', 'new']);
  }

  onBack() {
    this.router.navigate(['/students', this.studentId, 'goals', this.goalId]);
  }

  // ********************** Support Procedures ***********************

  // *****************************************************************
  // Loads benchmarks for the student from the service.
  // *****************************************************************
  private loadBenchmarks() {
    this.studentService.getBenchmarksForStudent(this.studentId).then(data => {
      if (!data.success) {
        this.errorMessage.set(data.message);
      } else {
        this.studentIdentifier.set(data.payload?.studentIdentifier ?? null);
        this.benchmarks.set(data.payload?.benchmarks ?? []);
      }
    });
  }
}
