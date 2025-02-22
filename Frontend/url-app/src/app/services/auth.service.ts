import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private tokenKey = 'jwtToken';
  private roleKey = 'userRole';
  private emailKey = 'userEmail';
  private fNameKey = 'firstName';
  private lNameKey = 'lastName';

  login(email: string, password: string) {
    return this.http.post<{ user: any; token: string }>(
      'https://localhost:7266/api/auth/login',
      { email, password }
    ).pipe(
      tap(response => {
        localStorage.setItem(this.tokenKey, response.token);
        localStorage.setItem(this.roleKey, response.user.role);
        localStorage.setItem(this.emailKey, response.user.email);
        localStorage.setItem(this.fNameKey, response.user.firstName);
        localStorage.setItem(this.lNameKey, response.user.lastName);
      })
    );
  }

  register(data: { firstName: string; lastName: string; email: string; password: string }) {
    return this.http.post(
      'https://localhost:7266/api/auth/register',
      data
    );
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  getUserRole(): string | null {
    return localStorage.getItem(this.roleKey);
  }

  isAdmin(): boolean {
    return this.getUserRole() === 'Admin';
  }

  getUserEmail(): string | null {
    return localStorage.getItem(this.emailKey);
  }

  getFullName(): string {
    const fName = localStorage.getItem(this.fNameKey) || '';
    const lName = localStorage.getItem(this.lNameKey) || '';
    return (fName && lName) ? `${fName} ${lName}` : fName || lName;
  }

  logout() {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.roleKey);
    localStorage.removeItem(this.emailKey);
    localStorage.removeItem(this.fNameKey);
    localStorage.removeItem(this.lNameKey);
  }
}
