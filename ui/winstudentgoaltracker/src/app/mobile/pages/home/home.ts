import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { DummyMobileHomeMeta, MobileHomeMeta } from '../../../shared/services/dummy-mobile-home-meta.service';
import { Auth } from '../../../shared/services/auth';

@Component({
  selector: 'app-home',
  imports: [RouterOutlet],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home {

  // ************************** Constructor **************************

  constructor() {
    this.loadMeta();
  }

  // ************************** Declarations *************************

  private readonly metaService = inject(DummyMobileHomeMeta);
  private readonly auth = inject(Auth);

  protected readonly meta = signal<MobileHomeMeta | null>(null);


  // TODO show this in the UI
  public errorMessage = signal<String | null>(null);

  // ************************** Properties ***************************

  // ************************ Public Methods *************************

  // ************************ Event Handlers *************************

  // *****************************************************************
  // Logs the user out and sends them back to the login screen.
  // *****************************************************************
  onLogout() {
    this.auth.logout().subscribe();
    this.auth.forceLogout();
  }

  // ********************** Support Procedures ***********************

  private loadMeta() {
    this.metaService.getMeta().then(data => {

      if (!data.success)
      {
        this.errorMessage.set(data.message);
      }
      else
      {
        this.meta.set(data.payload);
      }
    });
  }
}
