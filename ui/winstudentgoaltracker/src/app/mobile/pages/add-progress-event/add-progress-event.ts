import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { describeHttpError } from '../../../shared/classes/http-errors';
import { DummyStudentService } from '../../../shared/services/dummy-student.service';
import { StudentService } from '../../../shared/services/student.service';

@Component({
  selector: 'app-add-progress-event',
  imports: [FormsModule],
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
  // Saves the progress event. On success, returns to the goal list.
  // On failure, displays the error message from the API.
  // *****************************************************************
  async onSave() {
    this.error.set(null);
    this.saving.set(true);

    try {
      const result = await this.studentService.addProgressEvent(this.studentId, this.goalId, this.notes().trim());
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
}
