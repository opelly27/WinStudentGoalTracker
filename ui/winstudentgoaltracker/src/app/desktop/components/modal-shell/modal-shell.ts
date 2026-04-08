import { Component, input, output } from '@angular/core';

@Component({
    selector: 'app-modal-shell',
    templateUrl: './modal-shell.html',
    styleUrl: './modal-shell.scss',
})
export class ModalShell {
    readonly title = input.required<string>();
    readonly closed = output<void>();

    onOverlayClick() {
        this.closed.emit();
    }

    onClose() {
        this.closed.emit();
    }
}
