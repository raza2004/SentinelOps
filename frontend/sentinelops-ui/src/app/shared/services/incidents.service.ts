import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CreateIncidentDto,
  IncidentDto,
  IncidentStatus,
  IncidentSummaryDto,
  Severity,
  UpdateIncidentDto
} from '../models/incident.models';

@Injectable({ providedIn: 'root' })
export class IncidentsService {
  private readonly baseUrl = `${environment.apiUrl}/incidents`;

  constructor(private http: HttpClient) {}

  getIncidents(status?: IncidentStatus, severity?: Severity): Observable<IncidentSummaryDto[]> {
    let params = new HttpParams();
    if (status !== undefined) params = params.set('status', status.toString());
    if (severity !== undefined) params = params.set('severity', severity.toString());
    return this.http.get<IncidentSummaryDto[]>(this.baseUrl, { params });
  }

  getIncidentById(id: string): Observable<IncidentDto> {
    return this.http.get<IncidentDto>(`${this.baseUrl}/${id}`);
  }

  createIncident(dto: CreateIncidentDto): Observable<string> {
    return this.http.post<string>(this.baseUrl, dto);
  }

  updateIncident(id: string, dto: UpdateIncidentDto): Observable<boolean> {
    return this.http.put<boolean>(`${this.baseUrl}/${id}`, dto);
  }

  deleteIncident(id: string): Observable<boolean> {
    return this.http.delete<boolean>(`${this.baseUrl}/${id}`);
  }

  analyzeWithAi(id: string): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/${id}/analyze`, {});
  }
}
