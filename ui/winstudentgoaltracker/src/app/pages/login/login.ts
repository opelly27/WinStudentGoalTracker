import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Auth } from '../../services/auth';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  template: `
    <!-- Phase 1: Email & Password -->
    @if (!auth.isAuthenticated() && !auth.isSelectingProgram()) {
      <div class="card">
        <h2>Login</h2>

        @if (error()) {
          <p class="error">{{ error() }}</p>
        }

        <form (ngSubmit)="onLogin()">
          <label>
            Email
            <input type="email" [(ngModel)]="email" name="email" required />
          </label>

          <label>
            Password
            <input type="password" [(ngModel)]="password" name="password" required />
          </label>

          <button type="submit" [disabled]="loading()">
            {{ loading() ? 'Signing in...' : 'Sign in' }}
          </button>
        </form>
      </div>
    }

    <!-- Phase 2: Program Selection -->
    @if (auth.isSelectingProgram()) {
      <div class="card">
        <h2>Select a Program</h2>
        <p class="subtitle">Choose which program to log into.</p>

        @if (error()) {
          <p class="error">{{ error() }}</p>
        }

        <div class="program-list">
          @for (program of auth.programs(); track program.programId) {
            <button
              class="program-button"
              [disabled]="loading()"
              (click)="onSelectProgram(program.programId)"
            >
              <span class="program-name">{{ program.programName }}</span>
              <span class="program-meta">{{ program.roleDisplayName }}{{ program.isPrimary ? ' (Primary)' : '' }}</span>
            </button>
          }
        </div>

        <button class="link-button" (click)="onBackToLogin()">Back to login</button>
      </div>
    }

    <!-- Authenticated: User Info -->
    @if (auth.isAuthenticated()) {
      <div class="card">
        <h2>Authenticated</h2>

        @if (auth.user(); as user) {
          <dl>
            <dt>User ID</dt>
            <dd class="mono">{{ user.userId }}</dd>

            <dt>Email</dt>
            <dd>{{ user.email }}</dd>

            <dt>Program ID</dt>
            <dd class="mono">{{ user.programId }}</dd>

            <dt>Role</dt>
            <dd>{{ user.role }}</dd>
          </dl>
        }

        <button (click)="onLogout()">Logout</button>
      </div>
    }
  `,
  styles: `
    :host {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 100vh;
      font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
      background: #f5f5f5;
      padding: 1rem;
    }

    .card {
      background: #fff;
      border-radius: 8px;
      box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
      padding: 2rem;
      width: 100%;
      max-width: 420px;
    }

    h2 {
      margin: 0 0 0.5rem;
      font-size: 1.5rem;
    }

    .subtitle {
      margin: 0 0 1rem;
      color: #666;
      font-size: 0.875rem;
    }

    form {
      display: flex;
      flex-direction: column;
      gap: 1rem;
      margin-top: 1rem;
    }

    label {
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
      font-size: 0.875rem;
      font-weight: 500;
      color: #333;
    }

    input {
      padding: 0.625rem 0.75rem;
      border: 1px solid #ddd;
      border-radius: 6px;
      font-size: 0.9375rem;
      outline: none;
      transition: border-color 0.15s;
    }

    input:focus {
      border-color: #4f46e5;
    }

    button[type='submit'],
    button:not(.program-button):not(.link-button) {
      padding: 0.625rem 1rem;
      background: #4f46e5;
      color: #fff;
      border: none;
      border-radius: 6px;
      font-size: 0.9375rem;
      font-weight: 500;
      cursor: pointer;
      transition: background 0.15s;
      margin-top: 0.5rem;
    }

    button[type='submit']:hover,
    button:not(.program-button):not(.link-button):hover {
      background: #4338ca;
    }

    button:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    .error {
      background: #fef2f2;
      color: #dc2626;
      padding: 0.625rem 0.75rem;
      border-radius: 6px;
      font-size: 0.875rem;
      margin: 0.5rem 0;
    }

    .program-list {
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
    }

    .program-button {
      display: flex;
      flex-direction: column;
      align-items: flex-start;
      gap: 0.125rem;
      padding: 0.75rem 1rem;
      background: #f9fafb;
      border: 1px solid #e5e7eb;
      border-radius: 6px;
      cursor: pointer;
      transition: background 0.15s, border-color 0.15s;
      text-align: left;
      width: 100%;
    }

    .program-button:hover {
      background: #eef2ff;
      border-color: #4f46e5;
    }

    .program-name {
      font-weight: 500;
      font-size: 0.9375rem;
      color: #111;
    }

    .program-meta {
      font-size: 0.8125rem;
      color: #666;
    }

    .link-button {
      background: none;
      border: none;
      color: #4f46e5;
      cursor: pointer;
      font-size: 0.875rem;
      padding: 0.5rem 0;
      margin-top: 0.75rem;
    }

    .link-button:hover {
      text-decoration: underline;
    }

    dl {
      display: grid;
      grid-template-columns: auto 1fr;
      gap: 0.5rem 1rem;
      margin: 1rem 0;
      font-size: 0.9375rem;
    }

    dt {
      font-weight: 500;
      color: #555;
    }

    dd {
      margin: 0;
      color: #111;
      word-break: break-all;
    }

    .mono {
      font-family: ui-monospace, SFMono-Regular, Menlo, monospace;
      font-size: 0.8125rem;
    }
  `,
})
export class Login {
  protected readonly auth = inject(Auth);

  protected email = '';
  protected password = '';
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);

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

  onLogout() {
    this.auth.logout().subscribe();
  }

  onBackToLogin() {
    this.error.set(null);
    // Clear session state so we go back to the login form
    this.auth.logout().subscribe();
  }
}
