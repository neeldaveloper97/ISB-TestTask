import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, MatToolbarModule, MatButtonModule, MatIconModule],
  template: `
    <mat-toolbar color="primary">
      <a class="brand" routerLink="/dashboard">
        <mat-icon>apartment</mat-icon>
        <span class="brand-name">Property Inventory</span>
      </a>
      <span class="spacer"></span>
      <nav class="nav-links">
        <a mat-button routerLink="/dashboard" routerLinkActive="active-link">Dashboard</a>
        <a mat-button routerLink="/properties" routerLinkActive="active-link">Properties</a>
        <a mat-button routerLink="/contacts" routerLinkActive="active-link">Contacts</a>
      </nav>
    </mat-toolbar>
    <router-outlet></router-outlet>
  `,
  styles: [`
    .spacer { flex: 1 1 auto; }
    .brand {
      display: inline-flex; align-items: center; gap: 10px;
      color: #fff; font-weight: 700; font-size: 1.15rem; letter-spacing: 0.01em;
    }
    .brand:hover { text-decoration: none; }
    .brand mat-icon { font-size: 26px; height: 26px; width: 26px; }
    .nav-links { display: flex; gap: 4px; }
    .nav-links a { color: rgba(255, 255, 255, 0.82); border-radius: 8px; }
    .nav-links a:hover { color: #fff; }
    .active-link {
      color: #fff !important; font-weight: 600;
      background: rgba(255, 255, 255, 0.18);
    }
    @media (max-width: 599px) {
      .brand-name { display: none; }
    }
  `]
})
export class AppComponent {}
