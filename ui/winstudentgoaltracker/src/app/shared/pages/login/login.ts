import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Auth } from '../../services/auth';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {

  // ************************** Constructor **************************

  // ************************** Declarations *************************

  private readonly router = inject(Router);
  protected readonly auth = inject(Auth);
  protected email = '';
  protected password = '';
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);

  // ************************** Properties ***************************

  // ************************ Public Methods *************************

  // ************************ Event Handlers *************************

  onLogin() {
    this.error.set(null);
    this.loading.set(true);

    this.auth.login(this.email, this.password).subscribe({
      next: (res) => {
        this.loading.set(false);
        if (!res.success) {
          this.error.set(res.message);
        }
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Unable to reach the server.');
      },
    });
  }

  onSelectProgram(programId: string) {
    this.error.set(null);
    this.loading.set(true);

    this.auth.selectProgram(programId).subscribe({
      next: (res) => {
        this.loading.set(false);
        if (!res.success) {
          this.error.set(res.message);
        }
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Unable to reach the server.');
      },
    });
  }

  onHome() {
    this.router.navigateByUrl('/');
  }

  onLogout() {
    this.auth.logout().subscribe();
  }

  onBackToLogin() {
    this.error.set(null);
    // Clear session state so we go back to the login form
    this.auth.logout().subscribe();
  }

  // ********************** Support Procedures ***********************
}
