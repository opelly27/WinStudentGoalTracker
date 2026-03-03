import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { StudentGoalSummary } from '../../../shared/classes/student-goal';
import { DummyStudentService } from '../../../shared/services/dummy-student.service';
import { StudentService } from '../../../shared/services/student.service';

@Component({
  selector: 'app-student-goals',
  imports: [],
  templateUrl: './student-goals.html',
  styleUrl: './student-goals.scss',
})
export class StudentGoals {

  // ************************** Constructor **************************

  constructor() {
    this.loadGoals();
  }

  // ************************** Declarations *************************

  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly studentService = inject(StudentService);

  private readonly studentId = this.route.snapshot.paramMap.get('studentId') ?? '';
  protected readonly data = signal<StudentGoalSummary | null>(null);

  // TODO show this in the UI
  public errorMessage = signal<String | null>(null);

  // ************************** Properties ***************************

  // ************************ Public Methods *************************

  // ************************ Event Handlers *************************

  // *****************************************************************
  // Navigates back to the student list.
  // *****************************************************************
  onBack() {
    this.router.navigate(['students'], { relativeTo: this.route.parent });
  }

  // *****************************************************************
  // Navigates to the add-progress-event page for the selected goal.
  // *****************************************************************
  onGoalClick(goalId: string, goalTitle: string) {
    this.router.navigate(
      ['students', this.studentId, 'goals', goalId, 'add-event'],
      { queryParams: { goalTitle, studentIdentifier: this.data()?.studentIdentifier } },
    );
  }

  // ********************** Support Procedures ***********************

  // *****************************************************************
  // Reads the student ID from the route param and loads their goals.
  // *****************************************************************
  private loadGoals() {
    if (!this.studentId) return;

    this.studentService.getGoalsForStudent(this.studentId).then(result => {

      if (!result.success)
      {
        this.errorMessage.set(result.message)
      }

      else
      {
        this.data.set(result.payload);
      }
      
    });
  }
}
