import { Component, inject, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ModalShell } from '../modal-shell/modal-shell';
import { StudentService } from '../../../shared/services/student.service';
import { StudentCardDto } from '../../../shared/classes/student-card.dto';

@Component({
    selector: 'app-edit-student-modal',
    imports: [FormsModule, ModalShell],
    templateUrl: './edit-student-modal.html',
    styleUrl: './edit-student-modal.scss',
})
export class EditStudentModal {
    private readonly studentService = inject(StudentService);

    readonly student = input.required<StudentCardDto>();
    readonly saved = output<void>();
    readonly closed = output<void>();

    protected readonly saving = signal(false);
    protected readonly errorMessage = signal<string | null>(null);

    protected identifier = '';
    protected nextIepDate = '';

    ngOnInit() {
        const s = this.student();
        this.identifier = s.identifier;
        this.nextIepDate = s.nextIepDate ? new Date(s.nextIepDate).toISOString().split('T')[0] : '';
    }

    async onSave() {
        if (!this.identifier.trim()) return;
        this.saving.set(true);
        this.errorMessage.set(null);

        const result = await this.studentService.updateStudent(this.student().studentId, {
            identifier: this.identifier,
            nextIepDate: this.nextIepDate || null,
        });

        this.saving.set(false);

        if (result.success) {
            this.studentService.notifyDataChanged();
            this.saved.emit();
        } else {
            this.errorMessage.set(result.message);
        }
    }
}
