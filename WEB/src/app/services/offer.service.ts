import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class OfferService {
  private apiUrl = 'http://localhost:5230/api/Offer';

  constructor(private http: HttpClient) {}

  getOffers(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}`).pipe(
      catchError(err => throwError(() => err.error?.error || 'Failed to load offers'))
    );
  }

  getOfferById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  updateOfferStatus(id: number, status: number): Observable<any> {
    return this.http.patch<any>(`${this.apiUrl}/${id}/status`, { status });
  }

  createOffer(offer: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}`, offer).pipe(
      catchError(err => throwError(() => err.error?.error || 'Failed to create offer'))
    );
  }
} 