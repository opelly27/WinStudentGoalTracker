import { Component, computed, inject, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { lucideX } from '@ng-icons/lucide';
import { HlmButton } from '@spartan-ng/helm/button';
import { HlmIcon } from '@spartan-ng/helm/icon';
import { HlmInput } from '@spartan-ng/helm/input';
import { HlmTextarea } from '@spartan-ng/helm/textarea';
import { CreateGoalDto } from '../../../shared/classes/create-goal.dto';
import { StudentGoalItem } from '../../../shared/classes/student-goal';
import { StudentService } from '../../../shared/services/student.service';

@Component({
    selector: 'app-add-goal-modal',
    imports: [FormsModule, NgIcon, HlmIcon, HlmButton, HlmInput, HlmTextarea],
    providers: [provideIcons({ lucideX })],
    templateUrl: './add-goal-modal.html',
    styleUrl: './add-goal-modal.scss',
})
export class AddGoalModal {

    // ************************** Constructor **************************

    // ************************** Declarations *************************

    private readonly studentService = inject(StudentService);

    readonly studentId = input.required<string>();
    readonly existingGoals = input.required<StudentGoalItem[]>();
    readonly nextIepDate = input<string | null>();
    readonly goalCreated = output<StudentGoalItem>();
    readonly cancelled = output<void>();

    protected readonly isSubmitting = signal(false);
    protected readonly errorMessage = signal<string | null>(null);

    protected readonly parentGoalOptions = computed(() =>
        this.existingGoals().filter(g => g.goalParentId === null)
    );

    protected form: CreateGoalDto = {
        description: '',
        category: '',
        baseline: '',
        goalParentId: null,
        targetCompletionDate: null,
    };

    // *****************************************************************
    // Pre-fills targetCompletionDate from the student's nextIepDate.
    // *****************************************************************
    ngOnInit() {
        const iepDate = this.nextIepDate?.();
        if (iepDate) {
            this.form.targetCompletionDate = iepDate;
        }
    }

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
