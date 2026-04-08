import { Component, inject, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ModalShell } from '../modal-shell/modal-shell';
import { StudentService } from '../../../shared/services/student.service';
import { StudentGoalItem } from '../../../shared/classes/student-goal';

@Component({
    selector: 'app-edit-goal-modal',
    imports: [FormsModule, ModalShell],
    templateUrl: './edit-goal-modal.html',
    styleUrl: './edit-goal-modal.scss',
})
export class EditGoalModal {
    private readonly studentService = inject(StudentService);

    readonly studentId = input.required<string>();
    readonly goal = input.required<StudentGoalItem>();
    readonly saved = output<void>();
    readonly closed = output<void>();

    protected readonly saving = signal(false);
    protected readonly errorMessage = signal<string | null>(null);

    protected category = '';
    protected description = '';
    protected baseline = '';
    protected targetCompletionDate: string | null = null;

    ngOnInit() {
        const g = this.goal();
        this.category = g.category;
        this.description = g.description;
        this.baseline = g.baseline;
        this.targetCompletionDate = g.targetCompletionDate ? g.targetCompletionDate.substring(0, 10) : null;
    }

    async onSave() {
        if (!this.category.trim() || !this.description.trim()) return;
        this.saving.set(true);
        this.errorMessage.set(null);

        const result = await this.studentService.updateGoal(this.studentId(), this.goal().goalId, {
            category: this.category,
            description: this.description,
            baseline: this.baseline,
            targetCompletionDate: this.targetCompletionDate,
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
