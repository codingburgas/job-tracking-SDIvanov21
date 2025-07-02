import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OfferService } from '../../services/offer.service';
import { ApplicationService, ApplicationStatusEnum } from '../../services/application.service';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-offer-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="auth-bg d-flex align-items-center justify-content-center min-vh-100">
      <div class="card shadow-lg p-4" style="max-width: 600px; width: 100%;">
        <div class="text-center mb-4">
          <img src="assets/logo.png" alt="Logo" width="64" height="64" class="mb-2 rounded-circle bg-light shadow-sm">
          <h2 class="mb-1">Job Offers</h2>
        </div>
        <div *ngIf="loading" class="text-center my-4">
          <div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div>
        </div>
        <div *ngIf="error" class="alert alert-danger">{{ error }}</div>
        <div *ngIf="successMessage" class="alert alert-success">{{ successMessage }}</div>
        <div *ngIf="!loading && !error && offers.length === 0" class="text-center text-muted">No offers available.</div>
        <div *ngIf="!loading && !error">
          <div *ngFor="let offer of offers" class="mb-3">
            <div class="card mb-2 shadow-sm">
              <div class="card-body">
                <h5 class="card-title">{{ offer.job }} <span class="text-muted">&#64; {{ offer.company }}</span></h5>
                <p class="card-text">{{ offer.description }}</p>
                <button *ngIf="isAdmin" class="btn btn-outline-primary btn-sm me-2" (click)="viewDetails(offer.id)">Details</button>
                <button class="btn btn-success btn-sm" [disabled]="applyLoading" (click)="apply(offer.id)">
                  <span *ngIf="applyLoading && applyingOfferId === offer.id" class="spinner-border spinner-border-sm"></span>
                  <span *ngIf="!applyLoading || applyingOfferId !== offer.id">Apply</span>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./offer-list.component.scss']
})
export class OfferListComponent implements OnInit {
  offers: any[] = [];
  loading = true;
  error: string | null = null;
  successMessage: string | null = null;
  applyLoading = false;
  applyingOfferId: number | null = null;
  isAdmin = false;

  constructor(
    private offerService: OfferService,
    private applicationService: ApplicationService,
    private authService: AuthService,
    private router: Router
  ) {
    this.isAdmin = this.authService.getUserRole() === 'Admin';
  }

  ngOnInit() {
    this.offerService.getOffers().subscribe({
      next: offers => {
        this.offers = offers;
        this.loading = false;
      },
      error: err => {
        this.error = err;
        this.loading = false;
      }
    });
  }

  viewDetails(id: number) {
    this.router.navigate(['/offers', id]);
  }

  apply(offerId: number) {
    this.applyLoading = true;
    this.applyingOfferId = offerId;
    this.successMessage = null;
    this.error = null;
    this.applicationService.applyToOffer(offerId).subscribe({
      next: () => {
        this.successMessage = 'Application submitted!';
        this.applyLoading = false;
        this.applyingOfferId = null;
      },
      error: err => {
        this.error = err;
        this.applyLoading = false;
        this.applyingOfferId = null;
      }
    });
  }
} 