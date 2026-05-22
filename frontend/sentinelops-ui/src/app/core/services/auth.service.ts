import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { jwtDecode } from 'jwt-decode';
import { environment } from '../../../environments/environment';
import { AuthResponse, LoginRequest, RegisterRequest } from '../../shared/models/auth.models';

interface DecodedToken {
  sub: string;
  email: string;
  name: string;
  // ASP.NET encodes ClaimTypes.Role to this URI key
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': string;
  exp: number;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly tokenKey = 'sentinelops_token';
  private currentUserSubject = new BehaviorSubject<AuthResponse | null>(null);
  currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {
    this.loadUserFromStorage();
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/login`, request).pipe(
      tap(response => this.persistSession(response))
    );
  }

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/register`, request).pipe(
      tap(response => this.persistSession(response))
    );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    this.currentUserSubject.next(null);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    if (!token) return false;
    try {
      const decoded = jwtDecode<DecodedToken>(token);
      return decoded.exp > Date.now() / 1000;
    } catch {
      return false;
    }
  }

  getCurrentUser(): AuthResponse | null {
    return this.currentUserSubject.getValue();
  }

  private persistSession(response: AuthResponse): void {
    localStorage.setItem(this.tokenKey, response.token);
    this.currentUserSubject.next(response);
  }

  private loadUserFromStorage(): void {
    const token = this.getToken();
    if (!token) return;
    try {
      const decoded = jwtDecode<DecodedToken>(token);
      if (decoded.exp <= Date.now() / 1000) {
        this.logout();
        return;
      }
      const authResponse: AuthResponse = {
        token,
        email: decoded.email,
        fullName: decoded.name ?? '',
        role: decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?? '',
        expiresAt: new Date(decoded.exp * 1000)
      };
      this.currentUserSubject.next(authResponse);
    } catch {
      this.logout();
    }
  }
}
