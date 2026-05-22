export enum IncidentStatus {
  Open = 0,
  Acknowledged = 1,
  Investigating = 2,
  Resolved = 3,
  Closed = 4
}

export enum Severity {
  Critical = 0,
  High = 1,
  Medium = 2,
  Low = 3
}

export interface IncidentSummaryDto {
  id: string;
  title: string;
  status: IncidentStatus;
  severity: Severity;
  assignedToName: string | null;
  slaDeadline: Date | null;
  isSlaBreach: boolean;
  createdAt: Date;
}

export interface IncidentDto extends IncidentSummaryDto {
  description: string;
  resolvedAt: Date | null;
  aiRootCause: string | null;
  aiSuggestedFix: string | null;
  aiSeverityExplanation: string | null;
  updatedAt: Date;
  commentsCount: number;
  alertsCount: number;
}

export interface CreateIncidentDto {
  title: string;
  description: string;
  severity: Severity;
  assignedToId: string | null;
  slaHours: number;
}

export interface UpdateIncidentDto {
  title?: string;
  description?: string;
  status?: IncidentStatus;
  severity?: Severity;
  assignedToId?: string;
}
