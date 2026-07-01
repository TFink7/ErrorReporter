import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ErrorApiService } from '../../core/error-api.service';
import { ApiClient, CreatedApiClient } from '../../core/models';

@Component({
  selector: 'app-clients',
  imports: [DatePipe, FormsModule],
  templateUrl: './clients.html',
  styleUrl: './clients.css',
})
export class ClientsPage implements OnInit {
  private readonly api = inject(ErrorApiService);

  newClientName = '';

  readonly clients = signal<ApiClient[] | null>(null);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly creating = signal(false);
  readonly createdClient = signal<CreatedApiClient | null>(null);
  readonly copied = signal(false);

  ngOnInit(): void {
    this.loadClients();
  }

  loadClients(): void {
    this.loading.set(true);
    this.error.set(null);

    this.api.getClients().subscribe({
      next: clients => {
        this.clients.set(clients);
        this.loading.set(false);
      },
      error: err => {
        this.error.set(
          err.status === 403 || err.status === 401
            ? 'Managing clients requires the master API key. Set it in Settings.'
            : 'Could not load clients. Managing clients requires the master API key.'
        );
        this.loading.set(false);
      },
    });
  }

  createClient(): void {
    const name = this.newClientName.trim();
    if (!name) return;

    this.creating.set(true);
    this.api.createClient(name).subscribe({
      next: created => {
        this.createdClient.set(created);
        this.copied.set(false);
        this.newClientName = '';
        this.creating.set(false);
        this.loadClients();
      },
      error: () => {
        this.creating.set(false);
        this.error.set('Could not create the client. Managing clients requires the master API key.');
      },
    });
  }

  copyKey(): void {
    const created = this.createdClient();
    if (!created) return;

    navigator.clipboard.writeText(created.apiKey).then(() => {
      this.copied.set(true);
      setTimeout(() => this.copied.set(false), 2000);
    });
  }

  dismissCreated(): void {
    this.createdClient.set(null);
  }

  revoke(client: ApiClient): void {
    if (!confirm(`Revoke '${client.name}'? Its API key will stop working immediately.`)) {
      return;
    }

    this.api.revokeClient(client.id).subscribe({
      next: () => this.loadClients(),
      error: () => this.error.set('Could not revoke the client.'),
    });
  }
}
