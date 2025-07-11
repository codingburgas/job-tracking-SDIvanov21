import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

export enum ApplicationStatusEnum {
  SUBMITTED = 'SUBMITTED',
  ACCEPTED = 'ACCEPTED',
  REJECTED = 'REJECTED'
}

@Injectable({ providedIn: 'root' })
export class ApplicationService {
  private apiUrl = 'http://localhost:5230/api/Application';

  constructor(private http: HttpClient) {}

  applyToOffer(offerId: number): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}`, { offerId }).pipe(
      catchError(err => throwError(() => err.error?.error || 'Failed to apply'))
    );
  }

  getUserApplications(userId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/for-user/${userId}`).pipe(
      catchError(err => throwError(() => err.error?.error || 'Failed to fetch applications'))
    );
  }

  getApplicationsForOffer(offerId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/for-offer/${offerId}`);
  }

  updateApplicationStatus(id: number, status: number): Observable<any> {
    return this.http.patch<any>(`${this.apiUrl}/${id}/status`, { status }).pipe(
      catchError(err => throwError(() => err.error?.error || 'Failed to update status'))
    );
  }
} 