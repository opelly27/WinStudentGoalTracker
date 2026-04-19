import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Api } from '../../services/api';

@Component({
    selector: 'app-register',
    imports: [FormsModule, RouterLink],
    templateUrl: './register.html',
    styleUrl: './register.css',
})
export class Register {

    // ************************** Constructor **************************

    private readonly api = inject(Api);
    private readonly router = inject(Router);

    // ************************** Declarations *************************

    name = '';
    email = '';
    password = '';
    districtName = '';
    districtContactEmail = '';
    programName = '';
    programDescription = '';

    // ************************** Properties ***************************

    protected readonly loading = signal(false);
    protected readonly error = signal<string | null>(null);
    protected readonly success = signal(false);

    // ************************ Event Handlers *************************

    onRegister() {
        this.error.set(null);
        this.loading.set(true);

        this.api.register({
            email: this.email,
            password: this.password,
            name: this.name,
            districtName: this.districtName,
            districtContactEmail: this.districtContactEmail || undefined,
            programName: this.programName,
            programDescription: this.programDescription || undefined,
        }).subscribe({
            next: (res) => {
                this.loading.set(false);
                if (res.success) {
                    this.success.set(true);
                } else {
                    this.error.set(res.message);
                }
            },
            error: () => {
                this.loading.set(false);
                this.error.set('An unexpected error occurred. Please try again.');
            },
        });
    }
}
