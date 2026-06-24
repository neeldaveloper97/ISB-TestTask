import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTableModule } from '@angular/material/table';
import { RouterLink } from '@angular/router';
import { Property } from '../../../core/models/models';
import { PropertyService } from '../../../core/services/property.service';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { CurrencyFormatPipe } from '../../../shared/pipes/currency-format.pipe';

@Component({
  selector: 'app-property-list',
  standalone: true,
  imports: [
    CommonModule, FormsModule, RouterLink, MatTableModule, MatPaginatorModule,
    MatFormFieldModule, MatInputModule, MatButtonModule, MatIconModule,
    MatProgressBarModule, MatDialogModule, CurrencyFormatPipe
  ],
  template: `
    <div class="page-container">
      <div class="page-header">
        <h1>Properties</h1>
        <a mat-raised-button color="primary" routerLink="/properties/new">
          <mat-icon>add</mat-icon> New Property
        </a>
      </div>

      <div class="filter-bar">
        <mat-form-field appearance="outline">
          <mat-label>Name</mat-label>
          <input matInput [(ngModel)]="nameFilter" (keyup.enter)="applyFilter()" />
        </mat-form-field>
        <mat-form-field appearance="outline">
          <mat-label>Address</mat-label>
          <input matInput [(ngModel)]="addressFilter" (keyup.enter)="applyFilter()" />
        </mat-form-field>
        <button mat-raised-button (click)="applyFilter()">Search</button>
        <button mat-button (click)="clearFilter()">Clear</button>
      </div>

      <mat-progress-bar *ngIf="loading" mode="indeterminate"></mat-progress-bar>

      <table mat-table [dataSource]="properties" *ngIf="!loading">
        <ng-container matColumnDef="name">
          <th mat-header-cell *matHeaderCellDef>Name</th>
          <td mat-cell *matCellDef="let p"><a [routerLink]="['/properties', p.id]">{{ p.name }}</a></td>
        </ng-container>
        <ng-container matColumnDef="address">
          <th mat-header-cell *matHeaderCellDef>Address</th>
          <td mat-cell *matCellDef="let p">{{ p.address }}</td>
        </ng-container>
        <ng-container matColumnDef="price">
          <th mat-header-cell *matHeaderCellDef>Price</th>
          <td mat-cell *matCellDef="let p">{{ p.price | currencyFormat:p.currency }}</td>
        </ng-container>
        <ng-container matColumnDef="registered">
          <th mat-header-cell *matHeaderCellDef>Registered</th>
          <td mat-cell *matCellDef="let p">{{ p.dateOfRegistration | date:'mediumDate' }}</td>
        </ng-container>
        <ng-container matColumnDef="actions">
          <th mat-header-cell *matHeaderCellDef></th>
          <td mat-cell *matCellDef="let p">
            <div class="actions-cell">
              <a mat-icon-button [routerLink]="['/properties', p.id, 'edit']" aria-label="Edit"><mat-icon>edit</mat-icon></a>
              <button mat-icon-button color="warn" (click)="confirmDelete(p)" aria-label="Delete"><mat-icon>delete</mat-icon></button>
            </div>
          </td>
        </ng-container>

        <tr mat-header-row *matHeaderRowDef="columns"></tr>
        <tr mat-row *matRowDef="let row; columns: columns"></tr>
      </table>

      <p *ngIf="!loading && properties.length === 0" class="muted">No properties found.</p>

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
export class PropertyListComponent implements OnInit {
  properties: Property[] = [];
  totalCount = 0;
  page = 1;
  pageSize = 10;
  loading = true;
  nameFilter = '';
  addressFilter = '';
  readonly columns = ['name', 'address', 'price', 'registered', 'actions'];

  constructor(private propertyService: PropertyService, private dialog: MatDialog) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading = true;
    this.propertyService.getAll({
      page: this.page,
      pageSize: this.pageSize,
      name: this.nameFilter || undefined,
      address: this.addressFilter || undefined
    }).subscribe({
      next: (res) => { this.properties = res.data; this.totalCount = res.totalCount; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  applyFilter(): void { this.page = 1; this.load(); }

  clearFilter(): void {
    this.nameFilter = '';
    this.addressFilter = '';
    this.page = 1;
    this.load();
  }

  onPage(event: PageEvent): void {
    this.page = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.load();
  }

  confirmDelete(p: Property): void {
    const ref = this.dialog.open(ConfirmDialogComponent, {
      data: { title: 'Delete property', message: `Delete "${p.name}"? This cannot be undone.`, confirmText: 'Delete' }
    });
    ref.afterClosed().subscribe((confirmed) => {
      if (confirmed) {
        this.propertyService.delete(p.id).subscribe(() => this.load());
      }
    });
  }
}
