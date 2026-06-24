import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, MatToolbarModule, MatButtonModule],
  template: `
    <mat-toolbar color="primary">
      <span>Property Inventory</span>
      <span class="spacer"></span>
      <a mat-button routerLink="/dashboard" routerLinkActive="active-link">Dashboard</a>
      <a mat-button routerLink="/properties" routerLinkActive="active-link">Properties</a>
      <a mat-button routerLink="/contacts" routerLinkActive="active-link">Contacts</a>
    </mat-toolbar>
    <router-outlet></router-outlet>
  `,
  styles: [`
    .spacer { flex: 1 1 auto; }
    .active-link { font-weight: 700; text-decoration: underline; }
    a { color: inherit; }
  `]
})
export class AppComponent {}
