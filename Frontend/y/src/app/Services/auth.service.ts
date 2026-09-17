import { Injectable, PLATFORM_ID, inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl = 'http://localhost:5265/Auth';

  private platformId = inject(PLATFORM_ID);
  private isBrowser = isPlatformBrowser(this.platformId);

  constructor(private http: HttpClient) {}

  login(loginData: any): Observable<any> {
    return this.http.post<any>(
      `${this.apiUrl}/login`,
      loginData
    );
  }

  register(registerData: any): Observable<any> {
    return this.http.post<any>(
      `${this.apiUrl}/register`,
      registerData
    );
  }

  getToken(): string | null {
    return this.isBrowser ? localStorage.getItem('token') : null;
  }

  isLoggedIn(): boolean {
    return this.isBrowser ? !!localStorage.getItem('token') : false;
  }

 decodeToken(token: string): any {
   try {
     const base64Url = token.split('.')[1];
     const base64 = base64Url
       .replace(/-/g, '+')
       .replace(/_/g, '/');
     const padded = base64.padEnd(
       Math.ceil(base64.length / 4) * 4,
       '='
     );

     return JSON.parse(atob(padded));
   } catch {
     return null;
   }
 }

 getRole(): string | null {
   const token = this.getToken();

   if (!token) {
     return null;
   }

   const payload = this.decodeToken(token);

   if (!payload) {
     return null;
   }

   return (
     payload.role ??
     payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ??
     null
   );
 }

  logout(): void {
    if (this.isBrowser) localStorage.removeItem('token');
  }
}