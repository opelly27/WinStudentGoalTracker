import { Component, inject, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ModalShell } from '../modal-shell/modal-shell';
import { StudentService } from '../../../shared/services/student.service';
import { BenchmarkDto } from '../../../shared/classes/benchmark.dto';
import { ProgressEventWithGoalDto } from '../../../shared/classes/student-full-profile.dto';
import { GOAL_COLOR } from '../../../shared/classes/category-colors';

@Component({
    selector: 'app-edit-event-modal',
    imports: [FormsModule, ModalShell],
    templateUrl: './edit-event-modal.html',
    styleUrl: './edit-event-modal.scss',
})
export class EditEventModal {
    private readonly studentService = inject(StudentService);

    readonly studentId = input.required<string>();
    readonly goalId = input.required<string>();

    readonly benchmarks = input<BenchmarkDto[]>([]);
    /** Benchmark IDs already associated with this event (from cached profile). */
    readonly eventBenchmarkIds = input<string[]>([]);
    /** null for new event, populated for edit */
    readonly event = input<ProgressEventWithGoalDto | null>(null);
    readonly saved = output<void>();
    readonly closed = output<void>();

    protected readonly saving = signal(false);
    protected readonly errorMessage = signal<string | null>(null);
    protected readonly selectedBenchmarkIds = signal<Set<string>>(new Set());

    protected content = '';

    ngOnInit() {
        const ev = this.event();
        if (ev) {
            this.content = ev.content;
            this.selectedBenchmarkIds.set(new Set(this.eventBenchmarkIds()));
        }
    }

    get isNew(): boolean {
        return this.event() === null;
    }

    get colors() {
        return GOAL_COLOR;
    }

    isBenchmarkSelected(id: string): boolean {
        return this.selectedBenchmarkIds().has(id);
    }

    toggleBenchmark(id: string) {
        this.selectedBenchmarkIds.update(set => {
            const next = new Set(set);
            if (next.has(id)) next.delete(id);
            else next.add(id);
            return next;
        });
    }

    async onSave() {
        if (!this.content.trim()) return;
        this.saving.set(true);
        this.errorMessage.set(null);

        const benchmarkIds = [...this.selectedBenchmarkIds()];

        if (this.isNew) {
            const result = await this.studentService.addProgressEvent(
                this.studentId(), this.goalId(), this.content.trim(),
                benchmarkIds.length > 0 ? benchmarkIds : undefined,
            );
            this.saving.set(false);
            if (result.success) {
                this.studentService.notifyDataChanged();
                this.saved.emit();
            } else {
                this.errorMessage.set(result.message);
            }
        } else {
            const result = await this.studentService.updateProgressEvent(
                this.studentId(), this.event()!.progressEventId, this.content.trim(), benchmarkIds,
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
}
