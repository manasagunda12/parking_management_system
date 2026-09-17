import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class InvoiceService {
  private apiUrl = 'http://localhost:5265/Invoice';

  constructor(private http: HttpClient) {}

  getAll(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}`);
  }

  getMyInvoices(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/my`);
  }

  getById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  generate(paymentId: number): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/generate/${paymentId}`, {});
  }
}
