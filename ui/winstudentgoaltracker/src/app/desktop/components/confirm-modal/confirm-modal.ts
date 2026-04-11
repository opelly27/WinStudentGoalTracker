import { Component, input, output, signal } from '@angular/core';
import { ModalShell } from '../modal-shell/modal-shell';

@Component({
    selector: 'app-confirm-modal',
    imports: [ModalShell],
    templateUrl: './confirm-modal.html',
    styleUrl: './confirm-modal.scss',
})
export class ConfirmModal {

    // ************************** Declarations *************************

    readonly title = input('Confirm');
    readonly message = input.required<string>();
    readonly confirmLabel = input('Delete');
    readonly cancelLabel = input('Cancel');
    readonly destructive = input(false);
    readonly doubleConfirm = input(false);

    readonly confirmed = output<void>();
    readonly closed = output<void>();

    // ************************** Properties ***************************

    protected readonly awaitingSecondConfirm = signal(false);

    // ************************ Event Handlers *************************

    // *****************************************************************
    // When doubleConfirm is enabled, the first click transitions to a
    // second confirmation state. The second click emits confirmed.
    // *****************************************************************
    onConfirm() {
        if (this.doubleConfirm() && !this.awaitingSecondConfirm()) {
            this.awaitingSecondConfirm.set(true);
            return;
        }
        this.confirmed.emit();
    }

    onCancel() {
        this.awaitingSecondConfirm.set(false);
        this.closed.emit();
    }
}
