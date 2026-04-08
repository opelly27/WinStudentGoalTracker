import { Component, inject, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ModalShell } from '../modal-shell/modal-shell';
import { CreateGoalDto } from '../../../shared/classes/create-goal.dto';
import { StudentGoalItem } from '../../../shared/classes/student-goal';
import { StudentService } from '../../../shared/services/student.service';

@Component({
    selector: 'app-goal-modal',
    imports: [FormsModule, ModalShell],
    templateUrl: './goal-modal.html',
    styleUrl: './goal-modal.scss',
})
export class GoalModal {
    private readonly studentService = inject(StudentService);

    /** Required: the student this goal belongs to. */
    readonly studentId = input.required<string>();

    /** Optional: when provided the modal operates in edit mode. */
    readonly goal = input<StudentGoalItem | null>(null);

    /** Optional: used to pre-fill the target completion date in add mode. */
    readonly nextIepDate = input<string | null>(null);

    /** Emits the newly created goal (add mode). */
    readonly goalCreated = output<StudentGoalItem>();

    /** Emits when an existing goal has been saved (edit mode). */
    readonly saved = output<void>();

    /** Emits when the modal is dismissed without saving. */
    readonly closed = output<void>();

    protected readonly isSubmitting = signal(false);
    protected readonly errorMessage = signal<string | null>(null);

    protected form: CreateGoalDto = {
        description: '',
        category: '',
        baseline: '',
        goalParentId: null,
        targetCompletionDate: null,
    };

    protected get isEditMode(): boolean {
        return !!this.goal();
    }

    protected get modalTitle(): string {
        return this.isEditMode ? 'Edit Goal' : 'Add Goal';
    }

    protected get submitLabel(): string {
        return this.isEditMode ? 'Save' : 'Add Goal';
    }

    ngOnInit() {
        const existing = this.goal();
        if (existing) {
            // Edit mode — populate form from the existing goal
            this.form.category = existing.category;
            this.form.description = existing.description;
            this.form.baseline = existing.baseline;
            this.form.goalParentId = existing.goalParentId;
            this.form.targetCompletionDate = existing.targetCompletionDate
                ? existing.targetCompletionDate.substring(0, 10)
                : null;
        } else {
            // Add mode — pre-fill target date from IEP if available
            const iepDate = this.nextIepDate?.();
            if (iepDate) {
                this.form.targetCompletionDate = iepDate;
            }
        }
    }

    async onSubmit() {
        if (!this.form.category.trim()) return;
        this.errorMessage.set(null);
        this.isSubmitting.set(true);

        if (this.isEditMode) {
            const result = await this.studentService.updateGoal(
                this.studentId(),
                this.goal()!.goalId,
                {
                    category: this.form.category,
                    description: this.form.description,
                    baseline: this.form.baseline,
                    targetCompletionDate: this.form.targetCompletionDate,
                },
            );
            this.isSubmitting.set(false);

            if (result.success) {
                this.studentService.notifyDataChanged();
                this.saved.emit();
            } else {
                this.errorMessage.set(result.message);
            }
        } else {
            const result = await this.studentService.createGoal(this.studentId(), this.form);
            this.isSubmitting.set(false);

            if (result.success) {
                this.goalCreated.emit(result.payload!);
            } else {
                this.errorMessage.set(result.message);
            }
        }
    }
}
