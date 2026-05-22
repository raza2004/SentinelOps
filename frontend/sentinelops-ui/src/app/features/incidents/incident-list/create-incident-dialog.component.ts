import { Component } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { IncidentsService } from '../../../shared/services/incidents.service';
import { Severity } from '../../../shared/models/incident.models';

@Component({
  selector: 'app-create-incident-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatProgressSpinnerModule
  ],
  template: `
    <h2 mat-dialog-title>New Incident</h2>
    <mat-dialog-content>
      <form [formGroup]="form" class="dialog-form">
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Title</mat-label>
          <input matInput formControlName="title" placeholder="Brief incident title" />
          @if (form.get('title')?.hasError('required') && form.get('title')?.touched) {
            <mat-error>Title is required.</mat-error>
          }
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Description</mat-label>
          <textarea matInput formControlName="description" rows="4"
            placeholder="Describe what happened..."></textarea>
          @if (form.get('description')?.hasError('required') && form.get('description')?.touched) {
            <mat-error>Description is required.</mat-error>
          }
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Severity</mat-label>
          <mat-select formControlName="severity">
            @for (s of severities; track s.value) {
              <mat-option [value]="s.value">{{ s.label }}</mat-option>
            }
          </mat-select>
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>SLA Hours</mat-label>
          <input matInput type="number" formControlName="slaHours" min="1" max="168" />
        </mat-form-field>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button (click)="onCancel()">Cancel</button>
      <button mat-raised-button color="primary"
        [disabled]="form.invalid || isSubmitting"
        (click)="onSubmit()">
        @if (isSubmitting) { <mat-spinner diameter="18" /> } @else { Create }
      </button>
    </mat-dialog-actions>

    <style>
      .dialog-form { display: flex; flex-direction: column; gap: 4px; min-width: 420px; }
      .full-width { width: 100%; }
    </style>
  `
})
export class CreateIncidentDialogComponent {
  form = this.fb.group({
    title: ['', [Validators.required, Validators.maxLength(500)]],
    description: ['', [Validators.required, Validators.maxLength(2000)]],
    severity: [Severity.Medium, Validators.required],
    slaHours: [4, [Validators.required, Validators.min(1), Validators.max(168)]]
  });

  severities = [
    { label: 'Critical', value: Severity.Critical },
    { label: 'High', value: Severity.High },
    { label: 'Medium', value: Severity.Medium },
    { label: 'Low', value: Severity.Low }
  ];

  isSubmitting = false;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<CreateIncidentDialogComponent>,
    private incidentsService: IncidentsService
  ) {}

  onSubmit(): void {
    if (this.form.invalid) return;
    this.isSubmitting = true;
    const { title, description, severity, slaHours } = this.form.value;
    this.incidentsService.createIncident({
      title: title!,
      description: description!,
      severity: severity!,
      assignedToId: null,
      slaHours: slaHours!
    }).subscribe({
      next: () => this.dialogRef.close(true),
      error: () => { this.isSubmitting = false; }
    });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
