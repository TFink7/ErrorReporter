import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ErrorApiService } from '../../core/error-api.service';
import { ErrorSummary } from '../../core/models';

@Component({
  selector: 'app-dashboard',
  imports: [DatePipe, RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class DashboardPage implements OnInit {
  private readonly api = inject(ErrorApiService);

  readonly summary = signal<ErrorSummary[] | null>(null);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  readonly totalErrors = computed(() =>
    (this.summary() ?? []).reduce((sum, s) => sum + s.count, 0)
  );

  readonly serviceCount = computed(() => (this.summary() ?? []).length);

  readonly lastOccurrence = computed(() => {
    const dates = (this.summary() ?? [])
      .map(s => s.mostRecentOccurrence)
      .filter((d): d is string => d !== null);
    return dates.length ? dates.reduce((a, b) => (a > b ? a : b)) : null;
  });

  ngOnInit(): void {
    this.api.getSummary().subscribe({
      next: summary => {
        this.summary.set(summary);
        this.loading.set(false);
      },
      error: err => {
        this.error.set(
          err.status === 401
            ? 'Your API key was rejected. Check it in Settings.'
            : 'Could not load the summary. Is the API running?'
        );
        this.loading.set(false);
      },
    });
  }
}
