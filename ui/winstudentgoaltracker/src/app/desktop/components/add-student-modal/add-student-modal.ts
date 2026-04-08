import { Component, inject, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ModalShell } from '../modal-shell/modal-shell';
import { CreateStudentDto } from '../../../shared/classes/create-student.dto';
import { StudentCardDto } from '../../../shared/classes/student-card.dto';
import { StudentService } from '../../../shared/services/student.service';

@Component({
    selector: 'app-add-student-modal',
    imports: [FormsModule, ModalShell],
    templateUrl: './add-student-modal.html',
    styleUrl: './add-student-modal.scss',
})
export class AddStudentModal {
    private readonly studentService = inject(StudentService);

    readonly studentCreated = output<StudentCardDto>();
    readonly cancelled = output<void>();

    protected readonly isSubmitting = signal(false);
    protected readonly errorMessage = signal<string | null>(null);

    protected form: CreateStudentDto = {
        identifier: '',
        programYear: null,
        enrollmentDate: null,
        nextIepDate: null,
    };

    async onSubmit() {
        if (!this.form.identifier.trim()) return;
        this.errorMessage.set(null);
        this.isSubmitting.set(true);

        const result = await this.studentService.createStudent(this.form);
        this.isSubmitting.set(false);

        if (!result.success) {
            this.errorMessage.set(result.message);
            return;
        }

        this.studentCreated.emit(result.payload!);
    }
}
