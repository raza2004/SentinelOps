import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatCardModule } from '@angular/material/card';
import { DatePipe, DecimalPipe, NgClass } from '@angular/common';
import { AlertsService } from '../../shared/services/alerts.service';
import { AlertDto, AlertStatus } from '../../shared/models/alert.models';
import { Severity } from '../../shared/models/incident.models';

@Component({
  selector: 'app-alerts',
  standalone: true,
  imports: [
    NgClass,
    DatePipe,
    DecimalPipe,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatCardModule
  ],
  templateUrl: './alerts.component.html',
  styleUrl: './alerts.component.scss'
})
export class AlertsComponent implements OnInit, AfterViewInit {
  displayedColumns = ['title', 'source', 'severity', 'status', 'metric', 'createdAt', 'actions'];
  dataSource = new MatTableDataSource<AlertDto>();
  isLoading = true;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private alertsService: AlertsService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadAlerts();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  loadAlerts(): void {
    this.isLoading = true;
    this.alertsService.getAlerts().subscribe({
      next: alerts => {
        this.dataSource.data = alerts;
        this.isLoading = false;
      },
      error: () => { this.isLoading = false; }
    });
  }

  acknowledgeAlert(id: string, event: Event): void {
    event.stopPropagation();
    this.alertsService.acknowledgeAlert(id).subscribe({
      next: () => this.loadAlerts()
    });
  }

  viewLinkedIncident(incidentId: string | null): void {
    if (incidentId) this.router.navigate(['/incidents', incidentId]);
  }

  getSeverityClass(severity: Severity): string {
    return {
      [Severity.Critical]: 'chip-critical',
      [Severity.High]: 'chip-high',
      [Severity.Medium]: 'chip-medium',
      [Severity.Low]: 'chip-low'
    }[severity] ?? '';
  }

  getStatusClass(status: AlertStatus): string {
    return {
      [AlertStatus.Active]: 'chip-active',
      [AlertStatus.Acknowledged]: 'chip-acknowledged',
      [AlertStatus.Resolved]: 'chip-resolved',
      [AlertStatus.Suppressed]: 'chip-suppressed'
    }[status] ?? '';
  }

  getSeverityLabel(severity: Severity): string { return Severity[severity]; }
  getStatusLabel(status: AlertStatus): string { return AlertStatus[status]; }
}
