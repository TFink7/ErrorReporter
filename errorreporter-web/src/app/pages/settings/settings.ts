import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiKeyService } from '../../core/api-key.service';

@Component({
  selector: 'app-settings',
  imports: [FormsModule],
  templateUrl: './settings.html',
  styleUrl: './settings.css',
})
export class SettingsPage {
  protected readonly apiKey = inject(ApiKeyService);
  private readonly router = inject(Router);

  keyInput = '';
  readonly saved = signal(false);

  save(): void {
    const key = this.keyInput.trim();
    if (!key) return;

    this.apiKey.setKey(key);
    this.keyInput = '';
    this.saved.set(true);
    setTimeout(() => {
      this.saved.set(false);
      this.router.navigate(['/dashboard']);
    }, 800);
  }

  clear(): void {
    if (!confirm('Remove the stored API key?')) return;
    this.apiKey.clear();
  }

  get maskedKey(): string {
    const key = this.apiKey.key();
    if (key.length <= 8) return '••••••••';
    return `${key.slice(0, 4)}…${key.slice(-4)}`;
  }
}
