import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTableModule } from '@angular/material/table';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Contact } from '../../../core/models/models';
import { ContactService } from '../../../core/services/contact.service';
import { CurrencyFormatPipe } from '../../../shared/pipes/currency-format.pipe';

@Component({
  selector: 'app-contact-detail',
  standalone: true,
  imports: [
    CommonModule, RouterLink, MatCardModule, MatTableModule, MatChipsModule,
    MatButtonModule, MatIconModule, MatProgressBarModule, CurrencyFormatPipe
  ],
  template: `
    <div class="page-container" *ngIf="!loading; else loadingTpl">
      <ng-container *ngIf="contact">
        <div class="page-header">
          <h1>{{ contact.firstName }} {{ contact.lastName }}</h1>
          <div class="actions-cell">
            <a mat-raised-button color="primary" [routerLink]="['/contacts', contact.id, 'edit']">
              <mat-icon>edit</mat-icon> Edit
            </a>
            <a mat-button routerLink="/contacts">Back to list</a>
          </div>
        </div>

        <mat-card>
          <mat-card-content>
            <p><strong>Phone:</strong> {{ contact.phoneNumber || '—' }}</p>
            <p><strong>Email:</strong> {{ contact.emailAddress }}</p>
          </mat-card-content>
        </mat-card>

        <h2>Properties Owned (past &amp; present)</h2>
        <table mat-table [dataSource]="contact.ownedProperties">
          <ng-container matColumnDef="property">
            <th mat-header-cell *matHeaderCellDef>Property</th>
            <td mat-cell *matCellDef="let o"><a [routerLink]="['/properties', o.propertyId]">{{ o.propertyName }}</a></td>
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
          <ng-container matColumnDef="status">
            <th mat-header-cell *matHeaderCellDef>Status</th>
            <td mat-cell *matCellDef="let o">
              <span [class.muted]="!o.isCurrentOwner">{{ o.isCurrentOwner ? 'Current owner' : 'Past owner' }}</span>
            </td>
          </ng-container>
          <tr mat-header-row *matHeaderRowDef="columns"></tr>
          <tr mat-row *matRowDef="let row; columns: columns"></tr>
        </table>
        <p *ngIf="contact.ownedProperties.length === 0" class="muted">This contact owns no properties.</p>
      </ng-container>
    </div>
    <ng-template #loadingTpl><div class="page-container"><mat-progress-bar mode="indeterminate"></mat-progress-bar></div></ng-template>
  `,
  styles: [`h2 { margin-top: 24px; }`]
})
export class ContactDetailComponent implements OnInit {
  contact?: Contact;
  loading = true;
  readonly columns = ['property', 'from', 'till', 'acquisition', 'status'];

  constructor(private route: ActivatedRoute, private contactService: ContactService) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.contactService.getById(id).subscribe({
      next: (c) => { this.contact = c; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }
}
