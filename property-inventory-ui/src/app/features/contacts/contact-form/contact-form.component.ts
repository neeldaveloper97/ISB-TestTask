import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ContactService } from '../../../core/services/contact.service';

@Component({
  selector: 'app-contact-form',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, RouterLink, MatCardModule, MatFormFieldModule,
    MatInputModule, MatButtonModule, MatProgressBarModule
  ],
  template: `
    <div class="page-container">
      <div class="page-header"><h1>{{ isEdit ? 'Edit Contact' : 'New Contact' }}</h1></div>
      <mat-progress-bar *ngIf="loading" mode="indeterminate"></mat-progress-bar>

      <mat-card *ngIf="!loading">
        <mat-card-content>
          <form [formGroup]="form" (ngSubmit)="submit()">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>First name</mat-label>
              <input matInput formControlName="firstName" />
              <mat-error *ngIf="form.controls.firstName.hasError('required')">First name is required</mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Last name</mat-label>
              <input matInput formControlName="lastName" />
              <mat-error *ngIf="form.controls.lastName.hasError('required')">Last name is required</mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Phone number</mat-label>
              <input matInput formControlName="phoneNumber" />
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Email</mat-label>
              <input matInput formControlName="emailAddress" />
              <mat-error *ngIf="form.controls.emailAddress.hasError('required')">Email is required</mat-error>
              <mat-error *ngIf="form.controls.emailAddress.hasError('email')">Enter a valid email</mat-error>
            </mat-form-field>

            <div class="actions-cell">
              <button mat-raised-button color="primary" type="submit" [disabled]="form.invalid || saving">
                {{ isEdit ? 'Save' : 'Create' }}
              </button>
              <a mat-button routerLink="/contacts">Cancel</a>
            </div>
          </form>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`.full-width { display: block; }`]
})
export class ContactFormComponent implements OnInit {
  isEdit = false;
  loading = false;
  saving = false;
  id?: string;

  form = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    phoneNumber: [''],
    emailAddress: ['', [Validators.required, Validators.email]]
  });

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private contactService: ContactService
  ) {}

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id') ?? undefined;
    this.isEdit = !!this.id;
    if (this.isEdit && this.id) {
      this.loading = true;
      this.contactService.getById(this.id).subscribe({
        next: (c) => {
          this.form.patchValue({
            firstName: c.firstName, lastName: c.lastName,
            phoneNumber: c.phoneNumber, emailAddress: c.emailAddress
          });
          this.loading = false;
        },
        error: () => { this.loading = false; }
      });
    }
  }

  submit(): void {
    if (this.form.invalid) return;
    this.saving = true;
    const v = this.form.getRawValue();

    if (this.isEdit && this.id) {
      this.contactService.update(this.id, {
        id: this.id,
        firstName: v.firstName!, lastName: v.lastName!,
        phoneNumber: v.phoneNumber!, emailAddress: v.emailAddress!
      }).subscribe({
        next: () => this.router.navigate(['/contacts', this.id]),
        error: () => { this.saving = false; }
      });
    } else {
      this.contactService.create({
        firstName: v.firstName!, lastName: v.lastName!,
        phoneNumber: v.phoneNumber!, emailAddress: v.emailAddress!
      }).subscribe({
        next: (c) => this.router.navigate(['/contacts', c.id]),
        error: () => { this.saving = false; }
      });
    }
  }
}
