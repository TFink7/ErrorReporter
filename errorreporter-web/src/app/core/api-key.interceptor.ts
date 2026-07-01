import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { ApiKeyService } from './api-key.service';

export const apiKeyInterceptor: HttpInterceptorFn = (req, next) => {
  const key = inject(ApiKeyService).key();

  if (key) {
    req = req.clone({ setHeaders: { 'X-Api-Key': key } });
  }

  return next(req);
};
