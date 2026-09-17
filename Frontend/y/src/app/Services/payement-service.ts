import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PayementService {
  private apiUrl = 'http://localhost:5265/Payment';

  constructor(private http: HttpClient) {}

  getAll(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}`);
  }

  getMyPayments(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/my`);
  }

  getById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  create(dto: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}`, dto);
  }

  pay(paymentId: number, paymentMethod: string): Observable<any> {
    const params = new HttpParams().set('paymentMethod', paymentMethod);
    return this.http.put<any>(`${this.apiUrl}/${paymentId}/pay`, {}, { params });
  }
}
