import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ReservationService {
  private apiUrl = 'http://localhost:5265/Reservation';

  constructor(private http: HttpClient) {}

  getAll(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}`);
  }

  getMyReservations(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/my`);
  }

  getById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  create(dto: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}`, dto);
  }

  cancel(id: number): Observable<any> {
    return this.http.patch<any>(`${this.apiUrl}/${id}/cancel`, {});
  }

  checkIn(id: number): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/${id}/checkin`, {});
  }
}
