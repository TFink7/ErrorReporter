import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ErrorApiService } from '../../core/error-api.service';
import { ErrorReport, PagedResult } from '../../core/models';

@Component({
  selector: 'app-error-list',
  imports: [DatePipe, FormsModule],
  templateUrl: './error-list.html',
  styleUrl: './error-list.css',
})
export class ErrorListPage implements OnInit {
  private readonly api = inject(ErrorApiService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  service = '';
  from = '';
  to = '';
  pageSize = 20;

  readonly result = signal<PagedResult<ErrorReport> | null>(null);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    this.service = this.route.snapshot.queryParamMap.get('service') ?? '';
    this.load(1);
  }

  load(page: number): void {
    this.loading.set(true);
    this.error.set(null);

    this.api
      .getErrors({
        service: this.service.trim() || undefined,
        from: this.from || undefined,
        to: this.to || undefined,
        page,
        pageSize: this.pageSize,
      })
      .subscribe({
        next: result => {
          this.result.set(result);
          this.loading.set(false);
        },
        error: err => {
          this.error.set(
            err.status === 401
              ? 'Your API key was rejected. Check it in Settings.'
              : 'Could not load errors. Is the API running?'
          );
          this.loading.set(false);
        },
      });
  }

  applyFilters(): void {
    this.load(1);
  }

  clearFilters(): void {
    this.service = '';
    this.from = '';
    this.to = '';
    this.load(1);
  }

  get hasFilters(): boolean {
    return !!(this.service.trim() || this.from || this.to);
  }

  open(report: ErrorReport): void {
    this.router.navigate(['/errors', report.id]);
  }
}
