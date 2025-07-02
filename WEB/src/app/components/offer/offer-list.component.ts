import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OfferService } from '../../services/offer.service';
import { ApplicationService, ApplicationStatusEnum } from '../../services/application.service';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { FormBuilder } from '@angular/forms';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

@Component({
  selector: 'app-offer-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  template: `
    <div class="auth-bg d-flex align-items-center justify-content-center min-vh-100">
      <div class="card shadow-lg p-4" style="max-width: 600px; width: 100%;">
        <div class="text-center mb-4">
          <img src="assets/logo.png" alt="Logo" width="64" height="64" class="mb-2 rounded-circle bg-light shadow-sm">
          <h2 class="mb-1">Job Offers</h2>
        </div>
        <div *ngIf="isAdmin" class="mb-3 text-end">
          <button class="btn btn-primary" (click)="showCreateModal = true">Create Offer</button>
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
                <button *ngIf="!isAdmin && offer.status === 1" class="btn btn-secondary btn-sm" disabled>Inactive</button>
                <button *ngIf="!isAdmin && offer.status === 0 && !appliedOfferIds.has(offer.id)" class="btn btn-success btn-sm" [disabled]="applyLoading" (click)="apply(offer.id)">
                  <span *ngIf="applyLoading && applyingOfferId === offer.id" class="spinner-border spinner-border-sm"></span>
                  <span *ngIf="!applyLoading || applyingOfferId !== offer.id">Apply</span>
                </button>
                <button *ngIf="!isAdmin && offer.status === 0 && appliedOfferIds.has(offer.id)" class="btn btn-secondary btn-sm" disabled>Applied</button>
              </div>
            </div>
          </div>
        </div>
        <!-- Create Offer Modal -->
        <div *ngIf="showCreateModal" class="modal fade show d-block" tabindex="-1" style="background:rgba(0,0,0,0.5);">
          <div class="modal-dialog">
            <div class="modal-content">
              <div class="modal-header">
                <h5 class="modal-title">Create Offer</h5>
                <button type="button" class="btn-close" (click)="showCreateModal = false"></button>
              </div>
              <form (ngSubmit)="createOffer()" [formGroup]="createOfferForm">
                <div class="modal-body">
                  <div class="mb-3">
                    <label class="form-label">Job</label>
                    <input formControlName="job" class="form-control" required />
                  </div>
                  <div class="mb-3">
                    <label class="form-label">Company</label>
                    <input formControlName="company" class="form-control" required />
                  </div>
                  <div class="mb-3">
                    <label class="form-label">Description</label>
                    <textarea formControlName="description" class="form-control" rows="3" required></textarea>
                  </div>
                  <div class="mb-3">
                    <label class="form-label">Status</label>
                    <select formControlName="status" class="form-select">
                      <option [value]="0">ACTIVE</option>
                      <option [value]="1">INACTIVE</option>
                    </select>
                  </div>
                </div>
                <div class="modal-footer">
                  <button type="button" class="btn btn-secondary" (click)="showCreateModal = false">Cancel</button>
                  <button type="submit" class="btn btn-primary" [disabled]="createOfferForm.invalid">Create</button>
                </div>
              </form>
            </div>
          </div>
        </div>
        <!-- End Modal -->
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
  userId: number | null = null;
  appliedOfferIds: Set<number> = new Set();
  showCreateModal = false;
  createOfferForm = this.fb.group({
    job: [''],
    company: [''],
    description: [''],
    status: [0]
  });

  constructor(
    private offerService: OfferService,
    private applicationService: ApplicationService,
    private authService: AuthService,
    private router: Router,
    private fb: FormBuilder
  ) {
    this.isAdmin = this.authService.getUserRole() === 'Admin';
  }

  ngOnInit() {
    this.userId = this.authService.getUserId();
    this.offerService.getOffers().subscribe({
      next: (offers: any[]) => {
        this.offers = offers;
        this.loading = false;
        if (this.userId) {
          this.applicationService.getUserApplications(this.userId).subscribe({
            next: (offerIds: number[]) => {
              this.appliedOfferIds = new Set(offerIds);
            },
            error: (err: any) => {
              // Optionally handle error
            }
          });
        }
      },
      error: (err: any) => {
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
        this.appliedOfferIds.add(offerId);
      },
      error: err => {
        this.error = err;
        this.applyLoading = false;
        this.applyingOfferId = null;
      }
    });
  }

  createOffer() {
    if (this.createOfferForm.invalid) return;
    const formValue = this.createOfferForm.value;
    const offer = {
      job: formValue.job,
      company: formValue.company,
      description: formValue.description,
      status: formValue.status
    };
    this.offerService.createOffer(offer).subscribe({
      next: (newOffer: any) => {
        this.offers.push(newOffer);
        this.showCreateModal = false;
        this.createOfferForm.reset({ job: '', company: '', description: '', status: 0 });
      },
      error: (err: any) => {
        this.error = err;
      }
    });
  }
} 