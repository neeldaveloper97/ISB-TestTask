import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTableModule } from '@angular/material/table';
import { Router, RouterLink } from '@angular/router';
import { Contact } from '../../../core/models/models';
import { ContactService } from '../../../core/services/contact.service';

@Component({
  selector: 'app-contact-list',
  standalone: true,
  imports: [
    CommonModule, FormsModule, RouterLink, MatTableModule, MatPaginatorModule,
    MatFormFieldModule, MatInputModule, MatButtonModule, MatIconModule, MatProgressBarModule
  ],
  template: `
    <div class="page-container">
      <div class="page-header">
        <h1>Contacts</h1>
        <a mat-raised-button color="primary" routerLink="/contacts/new"><mat-icon>add</mat-icon> New Contact</a>
      </div>

      <div class="filter-bar">
        <mat-form-field appearance="outline">
          <mat-label>First name</mat-label>
          <input matInput [(ngModel)]="firstNameFilter" (keyup.enter)="applyFilter()" />
        </mat-form-field>
        <mat-form-field appearance="outline">
          <mat-label>Last name</mat-label>
          <input matInput [(ngModel)]="lastNameFilter" (keyup.enter)="applyFilter()" />
        </mat-form-field>
        <mat-form-field appearance="outline">
          <mat-label>Email</mat-label>
          <input matInput [(ngModel)]="emailFilter" (keyup.enter)="applyFilter()" />
        </mat-form-field>
        <button mat-raised-button (click)="applyFilter()">Search</button>
        <button mat-button (click)="clearFilter()">Clear</button>
      </div>

      <mat-progress-bar *ngIf="loading" mode="indeterminate"></mat-progress-bar>

      <table mat-table [dataSource]="contacts" *ngIf="!loading">
        <ng-container matColumnDef="name">
          <th mat-header-cell *matHeaderCellDef>Name</th>
          <td mat-cell *matCellDef="let c"><a [routerLink]="['/contacts', c.id]">{{ c.firstName }} {{ c.lastName }}</a></td>
        </ng-container>
        <ng-container matColumnDef="phone">
          <th mat-header-cell *matHeaderCellDef>Phone</th>
          <td mat-cell *matCellDef="let c">{{ c.phoneNumber }}</td>
        </ng-container>
        <ng-container matColumnDef="email">
          <th mat-header-cell *matHeaderCellDef>Email</th>
          <td mat-cell *matCellDef="let c">{{ c.emailAddress }}</td>
        </ng-container>
        <ng-container matColumnDef="actions">
          <th mat-header-cell *matHeaderCellDef></th>
          <td mat-cell *matCellDef="let c">
            <div class="actions-cell">
              <button mat-icon-button (click)="onView(c)" aria-label="View"><mat-icon>visibility</mat-icon></button>
              <a mat-icon-button [routerLink]="['/contacts', c.id, 'edit']" aria-label="Edit"><mat-icon>edit</mat-icon></a>
            </div>
          </td>
        </ng-container>

        <tr mat-header-row *matHeaderRowDef="columns"></tr>
        <tr mat-row *matRowDef="let row; columns: columns"></tr>
      </table>

      <p *ngIf="!loading && contacts.length === 0" class="muted">No contacts found.</p>

      <mat-paginator
        [length]="totalCount"
        [pageSize]="pageSize"
        [pageIndex]="page - 1"
        [pageSizeOptions]="[5, 10, 25, 50]"
        (page)="onPage($event)">
      </mat-paginator>
    </div>
  `
})
export class ContactListComponent implements OnInit {
  contacts: Contact[] = [];
  totalCount = 0;
  page = 1;
  pageSize = 10;
  loading = true;
  firstNameFilter = '';
  lastNameFilter = '';
  emailFilter = '';
  readonly columns = ['name', 'phone', 'email', 'actions'];

  constructor(private contactService: ContactService, private router: Router) {}

  ngOnInit(): void { this.load(); }

  onView(c: Contact): void { this.router.navigate(['/contacts', c.id]); }

  load(): void {
    this.loading = true;
    this.contactService.getAll({
      page: this.page,
      pageSize: this.pageSize,
      firstName: this.firstNameFilter || undefined,
      lastName: this.lastNameFilter || undefined,
      email: this.emailFilter || undefined
    }).subscribe({
      next: (res) => { this.contacts = res.data; this.totalCount = res.totalCount; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  applyFilter(): void { this.page = 1; this.load(); }

  clearFilter(): void {
    this.firstNameFilter = '';
    this.lastNameFilter = '';
    this.emailFilter = '';
    this.page = 1;
    this.load();
  }

  onPage(event: PageEvent): void {
    this.page = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.load();
  }
}
