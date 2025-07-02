import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { OfferService } from '../../services/offer.service';
import { ApplicationService } from '../../services/application.service';
import { AuthService } from '../../services/auth.service';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-offer-detail',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  template: `
    <div class="auth-bg d-flex align-items-center justify-content-center min-vh-100">
      <div class="card shadow-lg p-4" style="max-width: 700px; width: 100%;">
        <div class="text-center mb-4">
          <img src="assets/logo.png" alt="Logo" width="64" height="64" class="mb-2 rounded-circle bg-light shadow-sm">
          <h2 class="mb-1">Offer Details</h2>
        </div>
        <div *ngIf="loading" class="text-center my-4">
          <div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div>
        </div>
        <div *ngIf="error" class="alert alert-danger">{{ error }}</div>
        <div *ngIf="offer">
          <h4>{{ offer.job }} <span class="text-muted">&#64; {{ offer.company }}</span></h4>
          <p>{{ offer.description }}</p>
          <div class="mb-2"><strong>Status:</strong>
            <ng-container *ngIf="isAdmin; else statusText">
              <select class="form-select d-inline w-auto" [(ngModel)]="offer.status" (change)="changeOfferStatus()">
                <option *ngFor="let opt of offerStatusOptions" [value]="opt.value">{{ opt.label }}</option>
              </select>
            </ng-container>
            <ng-template #statusText>{{ offerStatusMap[offer.status] || offer.status }}</ng-template>
          </div>
          <div class="mb-2"><strong>Created On:</strong> {{ offer.createdOn | date:'medium' }}</div>
          <div class="mb-2"><strong>Created By:</strong> {{ offer.createdBy }}</div>
          <div *ngIf="isAdmin">
            <h5 class="mt-4">Applications</h5>
            <div *ngIf="applications.length === 0" class="text-muted">No applications yet.</div>
            <div *ngFor="let app of applications" class="card mb-2">
              <div class="card-body d-flex flex-column flex-md-row align-items-md-center justify-content-between">
                <div>
                  <div><strong>Applicant:</strong> {{ app.applicantUsername }}</div>
                  <div><strong>Status:</strong> {{ statusMap[app.status] || app.status }}</div>
                </div>
                <div class="mt-2 mt-md-0">
                  <select class="form-select" [(ngModel)]="app.status" (change)="changeStatus(app)">
                    <option *ngFor="let opt of statusOptions" [value]="opt.value">{{ opt.label }}</option>
                  </select>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./offer-detail.component.scss']
})
export class OfferDetailComponent implements OnInit {
  offer: any = null;
  applications: any[] = [];
  loading = true;
  error: string | null = null;
  isAdmin = false;
  offerId: number | null = null;
  statusMap: any = {
    0: 'SUBMITTED',
    1: 'ACCEPTED',
    2: 'REJECTED'
  };
  statusOptions = [
    { value: 0, label: 'SUBMITTED' },
    { value: 1, label: 'ACCEPTED' },
    { value: 2, label: 'REJECTED' }
  ];
  offerStatusMap: any = {
    0: 'ACTIVE',
    1: 'INACTIVE'
  };
  offerStatusOptions = [
    { value: 0, label: 'ACTIVE' },
    { value: 1, label: 'INACTIVE' }
  ];

  constructor(
    private route: ActivatedRoute,
    private offerService: OfferService,
    private applicationService: ApplicationService,
    private authService: AuthService
  ) {
    this.isAdmin = this.authService.getUserRole() === 'Admin';
  }

  ngOnInit() {
    this.offerId = Number(this.route.snapshot.paramMap.get('id'));
    if (!this.offerId) {
      this.error = 'Invalid offer ID.';
      this.loading = false;
      return;
    }
    this.offerService.getOfferById(this.offerId).subscribe({
      next: offer => {
        this.offer = offer;
        if (this.isAdmin) {
          this.applicationService.getApplicationsForOffer(this.offerId!).subscribe({
            next: apps => {
              this.applications = apps;
              this.loading = false;
            },
            error: err => {
              this.error = err;
              this.loading = false;
            }
          });
        } else {
          this.loading = false;
        }
      },
      error: err => {
        this.error = err;
        this.loading = false;
      }
    });
  }

  changeStatus(app: any) {
    this.applicationService.updateApplicationStatus(app.id, Number(app.status)).subscribe();
  }

  changeOfferStatus() {
    if (this.offer && this.offer.id != null) {
      this.offerService.updateOfferStatus(this.offer.id, Number(this.offer.status)).subscribe();
    }
  }
} 