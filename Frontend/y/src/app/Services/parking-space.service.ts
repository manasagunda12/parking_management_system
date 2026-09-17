import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ParkingSpaceService {
  private apiUrl = 'http://localhost:5265/ParkingSpace';

  constructor(private http: HttpClient) {}

  createSpaces(lotId: number, totalSlots: number, numberOfFloors: number): Observable<any> {
    const params = new HttpParams()
      .set('lotId', lotId)
      .set('totalSlots', totalSlots)
      .set('numberOfFloors', numberOfFloors);
    return this.http.post<any>(`${this.apiUrl}/create-spaces`, {}, { params });
  }

  getByLotId(lotId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/lot/${lotId}`);
  }

  getAvailableSpaces(lotId: number, vehicleType: string): Observable<any[]> {
    const params = new HttpParams().set('vehicleType', vehicleType);
    return this.http.get<any[]>(`${this.apiUrl}/lot/${lotId}/available`, { params });
  }

  getById(spaceId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${spaceId}`);
  }

  updateStatus(spaceId: number, status: string): Observable<any> {
    return this.http.patch<any>(`${this.apiUrl}/${spaceId}/status`, JSON.stringify(status), {
      headers: { 'Content-Type': 'application/json' }
    });
  }
}
