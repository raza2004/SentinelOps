import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatChipsModule } from '@angular/material/chips';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { FormsModule } from '@angular/forms';
import { DatePipe, NgClass } from '@angular/common';
import { IncidentsService } from '../../../shared/services/incidents.service';
import {
  IncidentSummaryDto,
  IncidentStatus,
  Severity
} from '../../../shared/models/incident.models';
import { CreateIncidentDialogComponent } from './create-incident-dialog.component';

@Component({
  selector: 'app-incident-list',
  standalone: true,
  imports: [
    NgClass,
    DatePipe,
    FormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatDialogModule,
    MatChipsModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatFormFieldModule,
    MatProgressSpinnerModule,
    MatTooltipModule
  ],
  templateUrl: './incident-list.component.html',
  styleUrl: './incident-list.component.scss'
})
export class IncidentListComponent implements OnInit, AfterViewInit {
  displayedColumns = ['title', 'severity', 'status', 'assignedTo', 'slaDeadline', 'actions'];
  dataSource = new MatTableDataSource<IncidentSummaryDto>();

  isLoading = true;
  statusFilter: IncidentStatus | null = null;
  severityFilter: Severity | null = null;

  readonly statuses = [
    { label: 'Open', value: IncidentStatus.Open },
    { label: 'Acknowledged', value: IncidentStatus.Acknowledged },
    { label: 'Investigating', value: IncidentStatus.Investigating },
    { label: 'Resolved', value: IncidentStatus.Resolved },
    { label: 'Closed', value: IncidentStatus.Closed }
  ];

  readonly severities = [
    { label: 'Critical', value: Severity.Critical },
    { label: 'High', value: Severity.High },
    { label: 'Medium', value: Severity.Medium },
    { label: 'Low', value: Severity.Low }
  ];

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private incidentsService: IncidentsService,
    private router: Router,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadIncidents();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  loadIncidents(): void {
    this.isLoading = true;
    this.incidentsService.getIncidents(
      this.statusFilter ?? undefined,
      this.severityFilter ?? undefined
    ).subscribe({
      next: incidents => {
        this.dataSource.data = incidents;
        this.isLoading = false;
      },
      error: () => { this.isLoading = false; }
    });
  }

  onFilterChange(): void {
    this.loadIncidents();
  }

  viewIncident(id: string): void {
    this.router.navigate(['/incidents', id]);
  }

  deleteIncident(id: string, event: Event): void {
    event.stopPropagation();
    if (!confirm('Delete this incident? This action cannot be undone.')) return;
    this.incidentsService.deleteIncident(id).subscribe({
      next: () => this.loadIncidents()
    });
  }

  openCreateDialog(): void {
    const ref = this.dialog.open(CreateIncidentDialogComponent, {
      width: '500px',
      disableClose: true
    });
    ref.afterClosed().subscribe(created => {
      if (created) this.loadIncidents();
    });
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
