import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, TranslateModule],
  template: `
    <div class="auth-bg d-flex align-items-center justify-content-center min-vh-100">
      <div class="card shadow-lg p-4" style="max-width: 400px; width: 100%;">
        <div class="text-center mb-4">
          <img src="assets/logo.png" alt="Logo" width="64" height="64" class="mb-2 rounded-circle bg-light shadow-sm">
          <h2 class="mb-1">{{ 'REGISTER.TITLE' | translate }}</h2>
        </div>
        <form [formGroup]="registerForm" (ngSubmit)="onSubmit()">
          <div class="mb-3">
            <label for="username" class="form-label">{{ 'REGISTER.USERNAME' | translate }}</label>
            <input id="username" type="text" class="form-control" formControlName="username" [class.is-invalid]="submitted && registerForm.controls['username'].invalid">
            <div *ngIf="submitted && registerForm.controls['username'].invalid" class="invalid-feedback">
              {{ 'REGISTER.USERNAME_REQUIRED' | translate }}
            </div>
          </div>
          <div class="mb-3">
            <label for="password" class="form-label">{{ 'REGISTER.PASSWORD' | translate }}</label>
            <input id="password" type="password" class="form-control" formControlName="password" [class.is-invalid]="submitted && registerForm.controls['password'].invalid">
            <div *ngIf="submitted && registerForm.controls['password'].invalid" class="invalid-feedback">
              {{ 'REGISTER.PASSWORD_REQUIRED' | translate }}
            </div>
          </div>
          <div class="mb-3">
            <label for="confirmPassword" class="form-label">{{ 'REGISTER.CONFIRM_PASSWORD' | translate }}</label>
            <input id="confirmPassword" type="password" class="form-control" formControlName="confirmPassword" [class.is-invalid]="submitted && registerForm.controls['confirmPassword'].invalid">
            <div *ngIf="submitted && registerForm.controls['confirmPassword'].invalid" class="invalid-feedback">
              {{ 'REGISTER.CONFIRM_PASSWORD_REQUIRED' | translate }}
            </div>
            <div *ngIf="submitted && registerForm.errors?.['passwordMismatch']" class="invalid-feedback d-block">
              {{ 'REGISTER.PASSWORD_MISMATCH' | translate }}
            </div>
          </div>
          <div class="mb-3">
            <label for="firstName" class="form-label">{{ 'REGISTER.FIRST_NAME' | translate }}</label>
            <input id="firstName" type="text" class="form-control" formControlName="firstName" [class.is-invalid]="submitted && registerForm.controls['firstName'].invalid">
            <div *ngIf="submitted && registerForm.controls['firstName'].invalid" class="invalid-feedback">
              {{ 'REGISTER.FIRST_NAME_REQUIRED' | translate }}
            </div>
          </div>
          <div class="mb-3">
            <label for="surname" class="form-label">{{ 'REGISTER.SURNAME' | translate }}</label>
            <input id="surname" type="text" class="form-control" formControlName="surname" [class.is-invalid]="submitted && registerForm.controls['surname'].invalid">
            <div *ngIf="submitted && registerForm.controls['surname'].invalid" class="invalid-feedback">
              {{ 'REGISTER.SURNAME_REQUIRED' | translate }}
            </div>
          </div>
          <div class="mb-3">
            <label for="lastName" class="form-label">{{ 'REGISTER.LAST_NAME' | translate }}</label>
            <input id="lastName" type="text" class="form-control" formControlName="lastName" [class.is-invalid]="submitted && registerForm.controls['lastName'].invalid">
            <div *ngIf="submitted && registerForm.controls['lastName'].invalid" class="invalid-feedback">
              {{ 'REGISTER.LAST_NAME_REQUIRED' | translate }}
            </div>
          </div>
          <div *ngIf="error" class="alert alert-danger">{{ error }}</div>
          <button type="submit" class="btn btn-primary w-100">{{ 'REGISTER.SUBMIT' | translate }}</button>
        </form>
        <div class="mt-3 text-center">
          <button type="button" class="btn btn-outline-secondary w-100" (click)="goToLogin()">
            {{ 'REGISTER.HAVE_ACCOUNT' | translate }}
          </button>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  registerForm: FormGroup;
  submitted = false;
  error: string | null = null;

  constructor(private fb: FormBuilder, private router: Router, private auth: AuthService) {
    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required],
      firstName: ['', Validators.required],
      surname: ['', Validators.required],
      lastName: ['', Validators.required]
    }, { validators: this.passwordMatchValidator });
  }

  passwordMatchValidator(form: FormGroup) {
    return form.get('password')!.value === form.get('confirmPassword')!.value ? null : { passwordMismatch: true };
  }

  onSubmit() {
    this.submitted = true;
    this.error = null;
    if (this.registerForm.invalid) return;
    const { username, password, firstName, surname, lastName } = this.registerForm.value;
    this.auth.register({ username, password, firstName, surname, lastName }).subscribe({
      next: () => this.router.navigate(['/login']),
      error: err => this.error = err
    });
  }

  goToLogin() {
    this.router.navigate(['/login']);
  }
} 