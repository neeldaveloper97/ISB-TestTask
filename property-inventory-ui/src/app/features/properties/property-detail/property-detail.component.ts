import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AssignOwner, Contact, CURRENCIES, Property } from '../../../core/models/models';
import { ContactService } from '../../../core/services/contact.service';
import { PropertyService } from '../../../core/services/property.service';
import { CurrencyFormatPipe } from '../../../shared/pipes/currency-format.pipe';

@Component({
  selector: 'app-property-detail',
  standalone: true,
  imports: [
    CommonModule, FormsModule, RouterLink, MatCardModule, MatTableModule, MatListModule,
    MatButtonModule, MatIconModule, MatFormFieldModule, MatInputModule, MatSelectModule,
    MatProgressBarModule, CurrencyFormatPipe
  ],
  template: `
    <div class="page-container" *ngIf="!loading; else loadingTpl">
      <ng-container *ngIf="property">
        <div class="page-header">
          <h1>{{ property.name }}</h1>
          <div class="actions-cell">
            <a mat-raised-button color="primary" [routerLink]="['/properties', property.id, 'edit']">
              <mat-icon>edit</mat-icon> Edit
            </a>
            <a mat-button routerLink="/properties">Back to list</a>
          </div>
        </div>

        <mat-card>
          <mat-card-content>
            <p><strong>Address:</strong> {{ property.address }}</p>
            <p><strong>Current price:</strong> {{ property.price | currencyFormat:property.currency }}</p>
            <p><strong>Registered:</strong> {{ property.dateOfRegistration | date:'mediumDate' }}</p>
            <p><strong>Current owner:</strong> {{ currentOwnerName() }}</p>
          </mat-card-content>
        </mat-card>

        <h2>Ownership Timeline</h2>
        <table mat-table [dataSource]="property.ownerships">
          <ng-container matColumnDef="owner">
            <th mat-header-cell *matHeaderCellDef>Owner</th>
            <td mat-cell *matCellDef="let o">{{ o.contactName }}</td>
          </ng-container>
          <ng-container matColumnDef="from">
            <th mat-header-cell *matHeaderCellDef>From</th>
            <td mat-cell *matCellDef="let o">{{ o.effectiveFrom | date:'mediumDate' }}</td>
          </ng-container>
          <ng-container matColumnDef="till">
            <th mat-header-cell *matHeaderCellDef>Till</th>
            <td mat-cell *matCellDef="let o">{{ o.effectiveTill ? (o.effectiveTill | date:'mediumDate') : 'Current' }}</td>
          </ng-container>
          <ng-container matColumnDef="acquisition">
            <th mat-header-cell *matHeaderCellDef>Acquisition Price</th>
            <td mat-cell *matCellDef="let o">{{ o.acquisitionPrice | currencyFormat:o.acquisitionCurrency }}</td>
          </ng-container>
          <tr mat-header-row *matHeaderRowDef="ownershipColumns"></tr>
          <tr mat-row *matRowDef="let row; columns: ownershipColumns"></tr>
        </table>
        <p *ngIf="property.ownerships.length === 0" class="muted">No ownership records.</p>

        <h2>Price History</h2>
        <table mat-table [dataSource]="property.priceHistories">
          <ng-container matColumnDef="amount">
            <th mat-header-cell *matHeaderCellDef>Amount</th>
            <td mat-cell *matCellDef="let h">{{ h.amount | currencyFormat:h.currency }}</td>
          </ng-container>
          <ng-container matColumnDef="date">
            <th mat-header-cell *matHeaderCellDef>Effective Date</th>
            <td mat-cell *matCellDef="let h">{{ h.effectiveDate | date:'mediumDate' }}</td>
          </ng-container>
          <tr mat-header-row *matHeaderRowDef="priceColumns"></tr>
          <tr mat-row *matRowDef="let row; columns: priceColumns"></tr>
        </table>
        <p *ngIf="property.priceHistories.length === 0" class="muted">No price history.</p>

        <mat-card class="assign-card">
          <mat-card-header><mat-card-title>Assign / Transfer Owner</mat-card-title></mat-card-header>
          <mat-card-content>
            <div class="filter-bar">
              <mat-form-field appearance="outline">
                <mat-label>Contact</mat-label>
                <mat-select [(ngModel)]="assign.contactId">
                  <mat-option *ngFor="let c of contacts" [value]="c.id">{{ c.firstName }} {{ c.lastName }}</mat-option>
                </mat-select>
              </mat-form-field>
              <mat-form-field appearance="outline">
                <mat-label>Acquisition Price</mat-label>
                <input matInput type="number" [(ngModel)]="assign.acquisitionPrice" />
              </mat-form-field>
              <mat-form-field appearance="outline">
                <mat-label>Currency</mat-label>
                <mat-select [(ngModel)]="assign.acquisitionCurrency">
                  <mat-option *ngFor="let cur of currencies" [value]="cur">{{ cur }}</mat-option>
                </mat-select>
              </mat-form-field>
              <button mat-raised-button color="primary" [disabled]="!assign.contactId || saving" (click)="assignOwner()">
                Assign Owner
              </button>
            </div>
          </mat-card-content>
        </mat-card>
      </ng-container>
    </div>
    <ng-template #loadingTpl><div class="page-container"><mat-progress-bar mode="indeterminate"></mat-progress-bar></div></ng-template>
  `,
  styles: [`h2 { margin-top: 24px; } .assign-card { margin-top: 24px; }`]
})
export class PropertyDetailComponent implements OnInit {
  property?: Property;
  contacts: Contact[] = [];
  loading = true;
  saving = false;
  readonly currencies = CURRENCIES;
  readonly ownershipColumns = ['owner', 'from', 'till', 'acquisition'];
  readonly priceColumns = ['amount', 'date'];

  assign: AssignOwner = { contactId: '', acquisitionPrice: 0, acquisitionCurrency: 'EUR' };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private propertyService: PropertyService,
    private contactService: ContactService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.load(id);
    this.contactService.getAll({ pageSize: 100 }).subscribe((res) => (this.contacts = res.data));
  }

  load(id: string): void {
    this.loading = true;
    this.propertyService.getById(id).subscribe({
      next: (p) => { this.property = p; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  currentOwnerName(): string {
    const current = this.property?.ownerships.find((o) => o.isCurrentOwner);
    return current ? current.contactName : 'None';
  }

  assignOwner(): void {
    if (!this.property) return;
    this.saving = true;
    this.propertyService.assignOwner(this.property.id, this.assign).subscribe({
      next: (p) => {
        this.property = p;
        this.saving = false;
        this.assign = { contactId: '', acquisitionPrice: 0, acquisitionCurrency: 'EUR' };
      },
      error: () => { this.saving = false; }
    });
  }
}
