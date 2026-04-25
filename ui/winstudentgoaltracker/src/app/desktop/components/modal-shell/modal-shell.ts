import { Component, ElementRef, HostListener, inject, input, OnDestroy, OnInit, output } from '@angular/core';

@Component({
    selector: 'app-modal-shell',
    templateUrl: './modal-shell.html',
    styleUrl: './modal-shell.scss',
})
export class ModalShell implements OnInit, OnDestroy {
    readonly title = input.required<string>();
    readonly closed = output<void>();

    private readonly el = inject(ElementRef);
    private previousFocus: HTMLElement | null = null;

    ngOnInit() {
        this.previousFocus = document.activeElement as HTMLElement;
        requestAnimationFrame(() => {
            const focusable = this.el.nativeElement.querySelector<HTMLElement>(
                'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
            );
            focusable?.focus();
        });
    }

    ngOnDestroy() {
        this.previousFocus?.focus();
    }

    @HostListener('keydown.escape')
    onEscape() {
        this.closed.emit();
    }

    onOverlayClick() {
        this.closed.emit();
    }

    onClose() {
        this.closed.emit();
    }
}
