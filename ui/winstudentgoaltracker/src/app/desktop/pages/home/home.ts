import { Component, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-home',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home {

  // ************************** Constructor **************************

  // ************************** Declarations *************************

  protected readonly sidebarExpanded = signal(false);

  // ************************** Properties ***************************

  // ************************ Public Methods *************************

  // ************************ Event Handlers *************************

  onToggleSidebar() {
    this.sidebarExpanded.update(v => !v);
  }

  // ********************** Support Procedures ***********************
}
