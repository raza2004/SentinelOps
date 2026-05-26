import { Injectable, NgZone } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class SignalrService {
  private hubConnection!: HubConnection;

  incidentCreated$ = new Subject<any>();
  incidentUpdated$ = new Subject<any>();
  alertFired$ = new Subject<any>();
  aiAnalysisComplete$ = new Subject<any>();

  constructor(private zone: NgZone) {}

  startConnection(token: string): Promise<void> {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(environment.hubUrl, { accessTokenFactory: () => token })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('IncidentCreated', data =>
      this.zone.run(() => this.incidentCreated$.next(data)));
    this.hubConnection.on('IncidentUpdated', data =>
      this.zone.run(() => this.incidentUpdated$.next(data)));
    this.hubConnection.on('AlertFired', data =>
      this.zone.run(() => this.alertFired$.next(data)));
    this.hubConnection.on('AiAnalysisComplete', data =>
      this.zone.run(() => this.aiAnalysisComplete$.next(data)));

    return this.hubConnection.start();
  }

  stopConnection(): Promise<void> {
    return this.hubConnection?.stop() ?? Promise.resolve();
  }

  joinIncidentRoom(incidentId: string): void {
    this.hubConnection?.invoke('JoinIncidentRoom', incidentId).catch(console.error);
  }

  leaveIncidentRoom(incidentId: string): void {
    this.hubConnection?.invoke('LeaveIncidentRoom', incidentId).catch(console.error);
  }
}
