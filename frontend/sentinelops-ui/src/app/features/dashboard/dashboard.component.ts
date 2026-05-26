import { Component, OnInit, OnDestroy, NgZone } from '@angular/core';
import { forkJoin, Subject, takeUntil, catchError, of } from 'rxjs';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { DatePipe, NgClass } from '@angular/common';
import { IncidentsService } from '../../shared/services/incidents.service';
import { AlertsService } from '../../shared/services/alerts.service';
import { SignalrService } from '../../core/services/signalr.service';
import { AuthService } from '../../core/services/auth.service';
import { IncidentSummaryDto, IncidentStatus, Severity } from '../../shared/models/incident.models';
import { AlertDto, DashboardStatsDto } from '../../shared/models/alert.models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    NgClass,
    DatePipe,
    MatCardModule,
    MatIconModule,
    MatChipsModule,
    MatProgressBarModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit, OnDestroy {
  stats: DashboardStatsDto = {
    totalIncidents: 0,
    openIncidents: 0,
    criticalIncidents: 0,
    resolvedToday: 0,
    activeAlerts: 0,
    slaBreaches: 0,
    averageResolutionHours: 0
  };

  recentIncidents: IncidentSummaryDto[] = [];
  activeAlerts: AlertDto[] = [];
  isLoading = true;

  readonly IncidentStatus = IncidentStatus;
  readonly Severity = Severity;

  private destroy$ = new Subject<void>();

  constructor(
    private incidentsService: IncidentsService,
    private alertsService: AlertsService,
    private signalrService: SignalrService,
    private authService: AuthService,
    private zone: NgZone
  ) {}

  ngOnInit(): void {
    this.loadData();
    this.startRealTime();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.signalrService.stopConnection();
  }

  private loadData(): void {
    forkJoin({
      incidents: this.incidentsService.getIncidents().pipe(catchError(() => of([] as IncidentSummaryDto[]))),
      alerts: this.alertsService.getAlerts().pipe(catchError(() => of([] as AlertDto[])))
    })
      .pipe(takeUntil(this.destroy$))
      .subscribe(({ incidents, alerts }) => {
        this.zone.run(() => {
          this.recentIncidents = incidents;
          this.activeAlerts = alerts;
          this.isLoading = false;
          this.calculateStats();
        });
      });
  }

  private calculateStats(): void {
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    this.stats = {
      totalIncidents: this.recentIncidents.length,
      openIncidents: this.recentIncidents.filter(i => i.status === IncidentStatus.Open).length,
      criticalIncidents: this.recentIncidents.filter(i => i.severity === Severity.Critical).length,
      resolvedToday: this.recentIncidents.filter(
        i => i.status === IncidentStatus.Resolved &&
             new Date(i.createdAt) >= today
      ).length,
      activeAlerts: this.activeAlerts.length,
      slaBreaches: this.recentIncidents.filter(i => i.isSlaBreach).length,
      averageResolutionHours: 0
    };
  }

  private startRealTime(): void {
    const token = this.authService.getToken();
    if (!token) return;

    this.signalrService.startConnection(token)
      .then(() => {
        this.signalrService.incidentCreated$
          .pipe(takeUntil(this.destroy$))
          .subscribe(() => this.zone.run(() => this.loadData()));

        this.signalrService.alertFired$
          .pipe(takeUntil(this.destroy$))
          .subscribe(() => this.zone.run(() => this.loadData()));
      })
      .catch(err => console.warn('SignalR connection failed:', err));
  }

  getSeverityClass(severity: Severity): string {
    return {
      [Severity.Critical]: 'chip-critical',
      [Severity.High]: 'chip-high',
      [Severity.Medium]: 'chip-medium',
      [Severity.Low]: 'chip-low'
    }[severity] ?? '';
  }

  getStatusClass(status: IncidentStatus): string {
    return {
      [IncidentStatus.Open]: 'chip-open',
      [IncidentStatus.Acknowledged]: 'chip-acknowledged',
      [IncidentStatus.Investigating]: 'chip-investigating',
      [IncidentStatus.Resolved]: 'chip-resolved',
      [IncidentStatus.Closed]: 'chip-closed'
    }[status] ?? '';
  }

  getSeverityLabel(severity: Severity): string {
    return Severity[severity];
  }

  getStatusLabel(status: IncidentStatus): string {
    return IncidentStatus[status];
  }

  get recentFive(): IncidentSummaryDto[] {
    return this.recentIncidents.slice(0, 5);
  }
}
