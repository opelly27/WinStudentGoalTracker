import { Component, inject, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CreateGoalDto } from '../../../shared/classes/create-goal.dto';
import { StudentGoalItem } from '../../../shared/classes/student-goal';
import { StudentService } from '../../../shared/services/student.service';

@Component({
    selector: 'app-add-goal-modal',
    imports: [FormsModule],
    templateUrl: './add-goal-modal.html',
    styleUrl: './add-goal-modal.scss',
})
export class AddGoalModal {

    // ************************** Constructor **************************

    // ************************** Declarations *************************

    private readonly studentService = inject(StudentService);

    readonly studentId = input.required<string>();
    readonly goalCreated = output<StudentGoalItem>();
    readonly cancelled = output<void>();

    protected readonly isSubmitting = signal(false);
    protected readonly errorMessage = signal<string | null>(null);

    protected form: CreateGoalDto = {
        title: '',
        description: '',
        category: '',
    };

    // ************************** Properties ***************************

    // ************************ Public Methods *************************

    // ************************ Event Handlers *************************

    async onSubmit() {
        this.errorMessage.set(null);
        this.isSubmitting.set(true);

        const result = await this.studentService.createGoal(this.studentId(), this.form);

        this.isSubmitting.set(false);

        if (!result.success) {
            this.errorMessage.set(result.message);
            return;
        }

        this.goalCreated.emit(result.payload!);
    }

    onCancel() {
        this.cancelled.emit();
    }

    // ********************** Support Procedures ***********************
}
