import { Injectable, computed, signal } from '@angular/core';

const STORAGE_KEY = 'errorreporter.apiKey';

@Injectable({ providedIn: 'root' })
export class ApiKeyService {
  private readonly _key = signal<string>(localStorage.getItem(STORAGE_KEY) ?? '');

  readonly key = this._key.asReadonly();
  readonly hasKey = computed(() => this._key().length > 0);

  setKey(value: string): void {
    const key = value.trim();
    this._key.set(key);

    if (key) {
      localStorage.setItem(STORAGE_KEY, key);
    } else {
      localStorage.removeItem(STORAGE_KEY);
    }
  }

  clear(): void {
    this.setKey('');
  }
}
