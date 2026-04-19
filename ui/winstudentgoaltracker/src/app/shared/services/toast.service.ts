import { Injectable, signal } from '@angular/core';

export type ToastType = 'success' | 'error' | 'info';

export interface Toast {
    id: number;
    message: string;
    type: ToastType;
}

@Injectable({ providedIn: 'root' })
export class ToastService {
    private nextId = 0;
    readonly toasts = signal<Toast[]>([]);

    show(message: string, type: ToastType = 'success') {
        const id = ++this.nextId;
        this.toasts.update(list => [...list, { id, message, type }]);
        setTimeout(() => this.dismiss(id), 3000);
    }

    dismiss(id: number) {
        this.toasts.update(list => list.filter(t => t.id !== id));
    }
}
