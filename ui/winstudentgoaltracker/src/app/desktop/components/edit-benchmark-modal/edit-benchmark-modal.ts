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
    readonly benchmark = input.required<BenchmarkDto>();
    readonly saved = output<void>();
    readonly closed = output<void>();

    protected readonly saving = signal(false);
    protected readonly errorMessage = signal<string | null>(null);

    protected shortName = '';
    protected benchmarkText = '';

    ngOnInit() {
        const b = this.benchmark();
        this.shortName = b.shortName ?? '';
        this.benchmarkText = b.benchmark;
    }

    async onSave() {
        if (!this.shortName.trim()) return;
        this.saving.set(true);
        this.errorMessage.set(null);

        const result = await this.studentService.updateBenchmark(
            this.studentId(),
            this.benchmark().benchmarkId,
            this.benchmarkText,
            this.shortName,
        );

        this.saving.set(false);

        if (result.success) {
            this.studentService.notifyDataChanged();
            this.saved.emit();
        } else {
            this.errorMessage.set(result.message);
        }
    }
}
