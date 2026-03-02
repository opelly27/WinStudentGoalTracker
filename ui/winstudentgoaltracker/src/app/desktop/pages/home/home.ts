import { Component, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { Auth } from '../../../shared/services/auth';

@Component({
  selector: 'app-home',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home {

  // ************************** Constructor **************************

  // ************************** Declarations *************************

  private readonly auth = inject(Auth);
  protected readonly sidebarExpanded = signal(false);

  // ************************** Properties ***************************

  // ************************ Public Methods *************************

  // ************************ Event Handlers *************************

  onToggleSidebar() {
    this.sidebarExpanded.update(v => !v);
  }

  // *****************************************************************
  // Logs the user out and sends them back to the login screen.
  // *****************************************************************
  onLogout() {
    this.auth.logout().subscribe();
    this.auth.forceLogout();
  }

  // ********************** Support Procedures ***********************
}
