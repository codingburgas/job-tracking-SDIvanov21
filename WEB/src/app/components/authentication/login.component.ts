import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, TranslateModule],
  template: `
    <div class="auth-bg d-flex align-items-center justify-content-center min-vh-100">
      <div class="card shadow-lg p-4" style="max-width: 400px; width: 100%;">
        <div class="text-center mb-4">
          <img src="assets/logo.png" alt="Logo" width="64" height="64" class="mb-2 rounded-circle bg-light shadow-sm">
          <h2 class="mb-1">{{ 'LOGIN.TITLE' | translate }}</h2>
        </div>
        <form [formGroup]="loginForm" (ngSubmit)="onSubmit()">
          <div class="mb-3">
            <label for="username" class="form-label">{{ 'LOGIN.USERNAME' | translate }}</label>
            <input id="username" type="text" class="form-control" formControlName="username" [class.is-invalid]="submitted && loginForm.controls['username'].invalid">
            <div *ngIf="submitted && loginForm.controls['username'].invalid" class="invalid-feedback">
              {{ 'LOGIN.USERNAME_REQUIRED' | translate }}
            </div>
          </div>
          <div class="mb-3">
            <label for="password" class="form-label">{{ 'LOGIN.PASSWORD' | translate }}</label>
            <input id="password" type="password" class="form-control" formControlName="password" [class.is-invalid]="submitted && loginForm.controls['password'].invalid">
            <div *ngIf="submitted && loginForm.controls['password'].invalid" class="invalid-feedback">
              {{ 'LOGIN.PASSWORD_REQUIRED' | translate }}
            </div>
          </div>
          <div *ngIf="error" class="alert alert-danger">{{ error }}</div>
          <button type="submit" class="btn btn-primary w-100">{{ 'LOGIN.SUBMIT' | translate }}</button>
        </form>
        <div class="mt-3 text-center">
          <button type="button" class="btn btn-outline-secondary w-100" (click)="goToRegister()">
            {{ 'LOGIN.NO_ACCOUNT' | translate }}
          </button>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  loginForm: FormGroup;
  submitted = false;
  error: string | null = null;

  constructor(private fb: FormBuilder, private router: Router, private auth: AuthService) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  onSubmit() {
    this.submitted = true;
    this.error = null;
    if (this.loginForm.invalid) return;
    const { username, password } = this.loginForm.value;
    this.auth.login(username, password).subscribe({
      next: () => this.router.navigate(['/']),
      error: err => this.error = err
    });
  }

  goToRegister() {
    this.router.navigate(['/register']);
  }
} 