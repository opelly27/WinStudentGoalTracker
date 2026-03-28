import { Component, inject, signal, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { lucideArrowUp, lucideTarget } from '@ng-icons/lucide';
import { HlmButton } from '@spartan-ng/helm/button';
import { HlmIcon } from '@spartan-ng/helm/icon';
import { StudentService } from '../../../shared/services/student.service';
import { StudentCardDto } from '../../../shared/classes/student-card.dto';

@Component({
  selector: 'app-student-card-full',
  imports: [FormsModule, NgIcon, HlmIcon, HlmButton],
  providers: [provideIcons({ lucideArrowUp, lucideTarget })],
  templateUrl: './student-card-full.html',
  styleUrl: './student-card-full.scss',
})
export class StudentCardFull implements OnDestroy {

  // ************************** Constructor **************************

  constructor() {
    this.paramSub = this.route.paramMap.subscribe(params => {
      this.studentId = params.get('studentId')!;
      this.loadStudent();
    });
  }

  // ************************** Declarations *************************

  private readonly studentService = inject(StudentService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly paramSub: Subscription;

  private studentId!: string;

  protected readonly errorMessage = signal<string | null>(null);
  protected readonly successMessage = signal<string | null>(null);
  protected readonly saving = signal(false);
  protected readonly loaded = signal(false);
  protected readonly fading = signal(false);
  private successTimer: any = null;

  // Form fields — always editable
  protected identifier = '';
  protected nextIepDate = '';

  // Snapshot of last-saved values for cancel
  private savedIdentifier = '';
  private savedNextIepDate = '';

  // ************************** Properties ***************************

  // *****************************************************************
  // Returns true if form values differ from the saved snapshot.
  // *****************************************************************
  hasChanges(): boolean {
    return this.identifier !== this.savedIdentifier
      || this.nextIepDate !== this.savedNextIepDate;
  }

  // ************************ Public Methods *************************

  // ************************ Event Handlers *************************

  // *****************************************************************
  // Saves changes to the student via the API.
  // *****************************************************************
  async onSave() {
    this.saving.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    const result = await this.studentService.updateStudent(this.studentId, {
      identifier: this.identifier,
      nextIepDate: this.nextIepDate || null,
    });

    this.saving.set(false);

    if (result.success) {
      this.savedIdentifier = this.identifier;
      this.savedNextIepDate = this.nextIepDate;
      this.showSuccessTemporarily('Changes saved.');
      this.studentService.notifyDataChanged();
    } else {
      this.errorMessage.set(result.message);
    }
  }

  // *****************************************************************
  // Reverts form fields to the last-saved snapshot.
  // *****************************************************************
  onCancel() {
    this.identifier = this.savedIdentifier;
    this.nextIepDate = this.savedNextIepDate;
    this.errorMessage.set(null);
    this.successMessage.set(null);
  }

  onBack() {
    this.router.navigate(['/students']);
  }

  onGoals() {
    this.router.navigate(['/students', this.studentId, 'goals']);
  }

  ngOnDestroy() {
    this.paramSub.unsubscribe();
    if (this.successTimer) clearTimeout(this.successTimer);
  }

  // ********************** Support Procedures ***********************

  // *****************************************************************
  // Shows a success message for 4 seconds, then fades it out over 1s.
  // *****************************************************************
  private showSuccessTemporarily(message: string) {
    if (this.successTimer) clearTimeout(this.successTimer);
    this.fading.set(false);
    this.successMessage.set(message);

    this.successTimer = setTimeout(() => {
      this.fading.set(true);
      this.successTimer = setTimeout(() => {
        this.successMessage.set(null);
        this.fading.set(false);
      }, 1000);
    }, 4000);
  }

  // *****************************************************************
  // Loads the student by ID and populates form fields.
  // *****************************************************************
  private loadStudent() {
    if (!this.loaded()) {
      this.loaded.set(false);
    }
    this.studentService.getStudentById(this.studentId).then(result => {
      if (result.success && result.payload) {
        const s = result.payload;
        this.identifier = s.identifier;
        this.nextIepDate = this.toDateInput(s.nextIepDate);

        this.savedIdentifier = this.identifier;
        this.savedNextIepDate = this.nextIepDate;
        this.loaded.set(true);
      } else {
        this.errorMessage.set(result.message);
      }
    });
  }

  // *****************************************************************
  // Converts a Date to a YYYY-MM-DD string for date input binding.
  // *****************************************************************
  private toDateInput(date: Date | null): string {
    if (!date) return '';
    const d = new Date(date);
    return d.toISOString().split('T')[0];
  }
}
