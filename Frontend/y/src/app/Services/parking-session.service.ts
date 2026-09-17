import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ParkingSessionService {
  private apiUrl = 'http://localhost:5265/ParkingSession';

  constructor(private http: HttpClient) {}

  getAll(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}`);
  }

  getById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  bookSlot(vehicleId: number, spaceId: number): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/book-slot`, { vehicleId, spaceId });
  }

  occupySlot(sessionId: number): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/occupy-slot/${sessionId}`, {});
  }

  exitVehicle(sessionId: number): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/exit-vehicle/${sessionId}`, {});
  }
}
