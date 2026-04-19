import { Component, inject } from '@angular/core';
import { ToastService } from '../../../shared/services/toast.service';

@Component({
    selector: 'app-toast-host',
    templateUrl: './toast-host.html',
    styleUrl: './toast-host.scss',
})
export class ToastHost {
    protected readonly toast = inject(ToastService);
}
