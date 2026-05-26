import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { DatePipe, NgClass } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDividerModule } from '@angular/material/divider';
import { MatChipsModule } from '@angular/material/chips';
import { IncidentsService } from '../../../shared/services/incidents.service';
import { AlertsService } from '../../../shared/services/alerts.service';
import { SignalrService } from '../../../core/services/signalr.service';
import { IncidentDto, IncidentStatus, Severity } from '../../../shared/models/incident.models';
import { AlertDto } from '../../../shared/models/alert.models';

@Component({
  selector: 'app-incident-detail',
  standalone: true,
  imports: [
    NgClass,
    DatePipe,
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatFormFieldModule,
    MatTooltipModule,
    MatDividerModule,
    MatChipsModule
  ],
  templateUrl: './incident-detail.component.html',
  styleUrl: './incident-detail.component.scss'
})
export class IncidentDetailComponent implements OnInit, OnDestroy {
  incident: IncidentDto | null = null;
  relatedAlerts: AlertDto[] = [];
  isLoading = true;
  isAnalyzing = false;
  selectedStatus: IncidentStatus | null = null;

  private incidentId!: string;
  private destroy$ = new Subject<void>();

  readonly statuses = [
    { label: 'Open', value: IncidentStatus.Open },
    { label: 'Acknowledged', value: IncidentStatus.Acknowledged },
    { label: 'Investigating', value: IncidentStatus.Investigating },
    { label: 'Resolved', value: IncidentStatus.Resolved },
    { label: 'Closed', value: IncidentStatus.Closed }
  ];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private incidentsService: IncidentsService,
    private alertsService: AlertsService,
    private signalrService: SignalrService
  ) {}

  ngOnInit(): void {
    this.incidentId = this.route.snapshot.paramMap.get('id')!;
    this.loadIncident();

    this.signalrService.joinIncidentRoom(this.incidentId);

    this.signalrService.aiAnalysisComplete$
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.isAnalyzing = false;
        this.loadIncident();
      });

    this.signalrService.incidentUpdated$
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.loadIncident());
  }

  ngOnDestroy(): void {
    this.signalrService.leaveIncidentRoom(this.incidentId);
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadIncident(): void {
    this.isLoading = true;
    this.incidentsService.getIncidentById(this.incidentId).subscribe({
      next: incident => {
        this.incident = incident;
        this.selectedStatus = incident.status;
        this.isLoading = false;
        this.loadRelatedAlerts();
      },
      error: () => { this.isLoading = false; }
    });
  }

  private loadRelatedAlerts(): void {
    this.alertsService.getAlerts().subscribe({
      next: alerts => {
        this.relatedAlerts = alerts.filter(a => a.incidentId === this.incidentId);
      }
    });
  }

  triggerAiAnalysis(): void {
    this.isAnalyzing = true;
    this.incidentsService.analyzeWithAi(this.incidentId).subscribe({
      error: () => { this.isAnalyzing = false; }
    });
  }

  updateStatus(): void {
    if (this.selectedStatus === null || this.selectedStatus === this.incident?.status) return;
    this.incidentsService.updateIncident(this.incidentId, { status: this.selectedStatus }).subscribe({
      next: () => this.loadIncident()
    });
  }

  goBack(): void {
    this.router.navigate(['/incidents']);
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

  getSeverityLabel(severity: Severity): string { return Severity[severity]; }
  getStatusLabel(status: IncidentStatus): string { return IncidentStatus[status]; }
}
