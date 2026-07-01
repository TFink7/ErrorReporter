import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ErrorApiService } from '../../core/error-api.service';
import { ErrorReport } from '../../core/models';

@Component({
  selector: 'app-error-detail',
  imports: [DatePipe, RouterLink],
  templateUrl: './error-detail.html',
  styleUrl: './error-detail.css',
})
export class ErrorDetailPage implements OnInit {
  private readonly api = inject(ErrorApiService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly report = signal<ErrorReport | null>(null);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly deleting = signal(false);

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.api.getError(id).subscribe({
      next: report => {
        this.report.set(report);
        this.loading.set(false);
      },
      error: err => {
        this.error.set(
          err.status === 404
            ? 'This error report no longer exists.'
            : err.status === 401
              ? 'Your API key was rejected. Check it in Settings.'
              : 'Could not load the error report.'
        );
        this.loading.set(false);
      },
    });
  }

  delete(): void {
    const report = this.report();
    if (!report || !confirm(`Delete error #${report.id}? This cannot be undone.`)) {
      return;
    }

    this.deleting.set(true);
    this.api.deleteError(report.id).subscribe({
      next: () => this.router.navigate(['/errors']),
      error: () => {
        this.deleting.set(false);
        this.error.set('Could not delete the error report.');
      },
    });
  }
}
