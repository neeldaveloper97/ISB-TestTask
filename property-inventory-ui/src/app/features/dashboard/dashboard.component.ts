import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTableModule } from '@angular/material/table';
import { DashboardRow } from '../../core/models/models';
import { DashboardService } from '../../core/services/dashboard.service';
import { CurrencyFormatPipe } from '../../shared/pipes/currency-format.pipe';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatProgressBarModule, CurrencyFormatPipe],
  template: `
    <div class="page-container">
      <div class="page-header"><h1>Dashboard — Ownership Changes</h1></div>
      <mat-progress-bar *ngIf="loading" mode="indeterminate"></mat-progress-bar>

      <table mat-table [dataSource]="rows" *ngIf="!loading">
        <ng-container matColumnDef="id">
          <th mat-header-cell *matHeaderCellDef>ID</th>
          <td mat-cell *matCellDef="let r"><span class="muted">{{ r.id | slice:0:8 }}</span></td>
        </ng-container>

        <ng-container matColumnDef="propertyName">
          <th mat-header-cell *matHeaderCellDef>Property Name</th>
          <td mat-cell *matCellDef="let r">{{ r.propertyName }}</td>
        </ng-container>

        <ng-container matColumnDef="askingPrice">
          <th mat-header-cell *matHeaderCellDef>Asking Price</th>
          <td mat-cell *matCellDef="let r">{{ r.askingPrice | currencyFormat:r.askingPriceCurrency }}</td>
        </ng-container>

        <ng-container matColumnDef="owner">
          <th mat-header-cell *matHeaderCellDef>Owner</th>
          <td mat-cell *matCellDef="let r">{{ r.ownerName }}</td>
        </ng-container>

        <ng-container matColumnDef="dateOfPurchase">
          <th mat-header-cell *matHeaderCellDef>Date of Purchase</th>
          <td mat-cell *matCellDef="let r">{{ r.dateOfPurchase | date:'mediumDate' }}</td>
        </ng-container>

        <ng-container matColumnDef="soldAtOriginal">
          <th mat-header-cell *matHeaderCellDef>Sold At (original)</th>
          <td mat-cell *matCellDef="let r">{{ r.soldAtPriceOriginal | currencyFormat:r.soldAtPriceCurrency }}</td>
        </ng-container>

        <ng-container matColumnDef="soldAtUsd">
          <th mat-header-cell *matHeaderCellDef>Sold At (USD)</th>
          <td mat-cell *matCellDef="let r">
            <span *ngIf="r.soldAtPriceUsd !== null; else na">{{ r.soldAtPriceUsd | currencyFormat:'USD' }}</span>
            <ng-template #na><span class="muted">N/A</span></ng-template>
          </td>
        </ng-container>

        <tr mat-header-row *matHeaderRowDef="columns"></tr>
        <tr mat-row *matRowDef="let row; columns: columns"></tr>
      </table>

      <p *ngIf="!loading && rows.length === 0" class="muted">No ownership changes recorded.</p>
    </div>
  `
})
export class DashboardComponent implements OnInit {
  rows: DashboardRow[] = [];
  loading = true;
  readonly columns = ['id', 'propertyName', 'askingPrice', 'owner', 'dateOfPurchase', 'soldAtOriginal', 'soldAtUsd'];

  constructor(private dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.dashboardService.getDashboard().subscribe({
      next: (rows) => { this.rows = rows; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }
}
