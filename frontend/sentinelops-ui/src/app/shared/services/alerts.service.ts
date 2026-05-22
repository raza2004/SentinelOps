import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AlertDto } from '../models/alert.models';

@Injectable({ providedIn: 'root' })
export class AlertsService {
  private readonly baseUrl = `${environment.apiUrl}/alerts`;

  constructor(private http: HttpClient) {}

  getAlerts(): Observable<AlertDto[]> {
    return this.http.get<AlertDto[]>(this.baseUrl);
  }

  createAlert(dto: any): Observable<any> {
    return this.http.post<any>(this.baseUrl, dto);
  }

  acknowledgeAlert(id: string): Observable<any> {
    return this.http.put<any>(`${this.baseUrl}/${id}/acknowledge`, {});
  }
}
