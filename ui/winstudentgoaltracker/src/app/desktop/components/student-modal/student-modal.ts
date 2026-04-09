import { Component, inject, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ModalShell } from '../modal-shell/modal-shell';
import { StudentService } from '../../../shared/services/student.service';
import { StudentCardDto } from '../../../shared/classes/student-card.dto';
import { toIsoDateString } from '../../../shared/utils/format-date';

@Component({
    selector: 'app-student-modal',
    imports: [FormsModule, ModalShell],
    templateUrl: './student-modal.html',
    styleUrl: './student-modal.scss',
})
export class StudentModal {
    private readonly studentService = inject(StudentService);

    /** Optional: when provided the modal operates in edit mode. */
    readonly student = input<StudentCardDto | null>(null);

    /** Emits the newly created student (add mode). */
    readonly studentCreated = output<StudentCardDto>();

    /** Emits when an existing student has been saved (edit mode). */
    readonly saved = output<void>();

    /** Emits when the modal is dismissed without saving. */
    readonly closed = output<void>();

    protected readonly isSubmitting = signal(false);
    protected readonly errorMessage = signal<string | null>(null);

    protected identifier = '';
    protected nextIepDate = '';

    protected get isEditMode(): boolean {
        return !!this.student();
    }

    protected get modalTitle(): string {
        return this.isEditMode ? 'Edit Student' : 'Add Student';
    }

    protected get submitLabel(): string {
        return this.isEditMode ? 'Save' : 'Add Student';
    }

    ngOnInit() {
        const s = this.student();
        if (s) {
            // Edit mode — populate form from the existing student
            this.identifier = s.identifier;
            this.nextIepDate = s.nextIepDate ? toIsoDateString(s.nextIepDate) : '';
        }
    }

    async onSubmit() {
        if (!this.identifier.trim()) return;
        this.errorMessage.set(null);
        this.isSubmitting.set(true);

        if (this.isEditMode) {
            const result = await this.studentService.updateStudent(this.student()!.studentId, {
                identifier: this.identifier,
                nextIepDate: this.nextIepDate || null,
            });
            this.isSubmitting.set(false);

            if (result.success) {
                this.studentService.notifyDataChanged();
                this.saved.emit();
            } else {
                this.errorMessage.set(result.message);
            }
        } else {
            const result = await this.studentService.createStudent({
                identifier: this.identifier,
                programYear: null,
                enrollmentDate: null,
                nextIepDate: this.nextIepDate ? new Date(this.nextIepDate) : null,
            });
            this.isSubmitting.set(false);

            if (result.success) {
                this.studentCreated.emit(result.payload!);
            } else {
                this.errorMessage.set(result.message);
            }
        }
    }
}
