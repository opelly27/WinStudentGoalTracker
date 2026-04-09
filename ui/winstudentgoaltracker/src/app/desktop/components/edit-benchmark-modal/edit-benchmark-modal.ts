import { Component, inject, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ModalShell } from '../modal-shell/modal-shell';
import { StudentService } from '../../../shared/services/student.service';
import { BenchmarkDto } from '../../../shared/classes/benchmark.dto';

@Component({
    selector: 'app-edit-benchmark-modal',
    imports: [FormsModule, ModalShell],
    templateUrl: './edit-benchmark-modal.html',
    styleUrl: './edit-benchmark-modal.scss',
})
export class EditBenchmarkModal {
    private readonly studentService = inject(StudentService);

    readonly studentId = input.required<string>();
    readonly goalId = input.required<string>();

    /** null for new benchmark, populated for edit */
    readonly benchmark = input<BenchmarkDto | null>(null);
    readonly saved = output<void>();
    readonly closed = output<void>();

    protected readonly saving = signal(false);
    protected readonly recommending = signal(false);
    protected readonly errorMessage = signal<string | null>(null);
    protected readonly recommendError = signal<string | null>(null);

    protected shortName = '';
    protected benchmarkText = '';

    protected get isEditMode(): boolean {
        return !!this.benchmark();
    }

    protected get modalTitle(): string {
        return this.isEditMode ? 'Edit Benchmark' : 'Add Benchmark';
    }

    protected get submitLabel(): string {
        return this.isEditMode ? 'Save' : 'Add Benchmark';
    }

    ngOnInit() {
        const b = this.benchmark();
        if (b) {
            this.shortName = b.shortName ?? '';
            this.benchmarkText = b.benchmark;
        }
    }

    async onGetRecommendation() {
        this.recommending.set(true);
        this.recommendError.set(null);

        const result = await this.studentService.getBenchmarkRecommendation(this.studentId(), this.goalId());

        this.recommending.set(false);

        if (result.success && result.payload) {
            this.benchmarkText = result.payload.benchmark;
            this.shortName = result.payload.shortName;
        } else {
            this.recommendError.set(result.message);
        }
    }

    async onSave() {
        if (!this.benchmarkText.trim()) return;
        this.saving.set(true);
        this.errorMessage.set(null);

        if (this.isEditMode) {
            const result = await this.studentService.updateBenchmark(
                this.studentId(),
                this.benchmark()!.benchmarkId,
                this.benchmarkText,
                this.shortName || undefined,
            );
            this.saving.set(false);

            if (result.success) {
                this.studentService.notifyDataChanged();
                this.saved.emit();
            } else {
                this.errorMessage.set(result.message);
            }
        } else {
            const result = await this.studentService.createBenchmark(this.studentId(), {
                goalId: this.goalId(),
                benchmark: this.benchmarkText,
                shortName: this.shortName || undefined,
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
}
