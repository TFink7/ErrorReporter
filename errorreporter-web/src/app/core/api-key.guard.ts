import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { ApiKeyService } from './api-key.service';

export const apiKeyGuard: CanActivateFn = () => {
  const apiKey = inject(ApiKeyService);
  const router = inject(Router);

  return apiKey.hasKey() ? true : router.createUrlTree(['/settings']);
};
