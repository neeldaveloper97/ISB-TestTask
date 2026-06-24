import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSelectModule } from '@angular/material/select';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CURRENCIES } from '../../../core/models/models';
import { PropertyService } from '../../../core/services/property.service';

@Component({
  selector: 'app-property-form',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, RouterLink, MatCardModule, MatFormFieldModule,
    MatInputModule, MatSelectModule, MatButtonModule, MatProgressBarModule
  ],
  template: `
    <div class="page-container">
      <div class="page-header"><h1>{{ isEdit ? 'Edit Property' : 'New Property' }}</h1></div>
      <mat-progress-bar *ngIf="loading" mode="indeterminate"></mat-progress-bar>

      <mat-card *ngIf="!loading">
        <mat-card-content>
          <form [formGroup]="form" (ngSubmit)="submit()">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Name</mat-label>
              <input matInput formControlName="name" />
              <mat-error *ngIf="form.controls.name.hasError('required')">Name is required</mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Address</mat-label>
              <input matInput formControlName="address" />
              <mat-error *ngIf="form.controls.address.hasError('required')">Address is required</mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Price</mat-label>
              <input matInput type="number" formControlName="price" />
              <mat-error *ngIf="form.controls.price.hasError('required')">Price is required</mat-error>
              <mat-error *ngIf="form.controls.price.hasError('min')">Price must be ≥ 0</mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Currency</mat-label>
              <mat-select formControlName="currency">
                <mat-option *ngFor="let c of currencies" [value]="c">{{ c }}</mat-option>
              </mat-select>
            </mat-form-field>

            <div class="actions-cell">
              <button mat-raised-button color="primary" type="submit" [disabled]="form.invalid || saving">
                {{ isEdit ? 'Save' : 'Create' }}
              </button>
              <a mat-button routerLink="/properties">Cancel</a>
            </div>
          </form>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`.full-width { display: block; } mat-form-field { margin-right: 16px; }`]
})
export class PropertyFormComponent implements OnInit {
  isEdit = false;
  loading = false;
  saving = false;
  id?: string;
  readonly currencies = CURRENCIES;

  form = this.fb.group({
    name: ['', Validators.required],
    address: ['', Validators.required],
    price: [0, [Validators.required, Validators.min(0)]],
    currency: ['EUR', Validators.required]
  });

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private propertyService: PropertyService
  ) {}

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id') ?? undefined;
    this.isEdit = !!this.id;
    if (this.isEdit && this.id) {
      this.loading = true;
      this.propertyService.getById(this.id).subscribe({
        next: (p) => {
          this.form.patchValue({ name: p.name, address: p.address, price: p.price, currency: p.currency });
          this.loading = false;
        },
        error: () => { this.loading = false; }
      });
    }
  }

  submit(): void {
    if (this.form.invalid) return;
    this.saving = true;
    const value = this.form.getRawValue();

    if (this.isEdit && this.id) {
      this.propertyService.update(this.id, {
        id: this.id,
        name: value.name!,
        address: value.address!,
        price: value.price!,
        currency: value.currency!
      }).subscribe({
        next: () => this.router.navigate(['/properties', this.id]),
        error: () => { this.saving = false; }
      });
    } else {
      this.propertyService.create({
        name: value.name!,
        address: value.address!,
        price: value.price!,
        currency: value.currency!
      }).subscribe({
        next: (p) => this.router.navigate(['/properties', p.id]),
        error: () => { this.saving = false; }
      });
    }
  }
}
