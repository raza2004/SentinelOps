import { Severity } from './incident.models';

export enum AlertStatus {
  Active = 0,
  Acknowledged = 1,
  Resolved = 2,
  Suppressed = 3
}

export interface AlertDto {
  id: string;
  title: string;
  message: string;
  source: string;
  status: AlertStatus;
  severity: Severity;
  metricValue: number | null;
  threshold: number | null;
  incidentId: string | null;
  createdAt: Date;
}

export interface DashboardStatsDto {
  totalIncidents: number;
  openIncidents: number;
  criticalIncidents: number;
  resolvedToday: number;
  activeAlerts: number;
  slaBreaches: number;
  averageResolutionHours: number;
}
