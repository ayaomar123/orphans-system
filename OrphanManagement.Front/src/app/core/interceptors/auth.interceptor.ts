import { inject } from '@angular/core';
import {
  HttpErrorResponse,
  HttpInterceptorFn,
  HttpRequest,
  HttpHandlerFn
} from '@angular/common/http';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

import { AuthService } from '../services/auth.service';

function shouldAttachToken(req: HttpRequest<unknown>) {
  // In this project we use a dev proxy and call the API via relative `/api/...` URLs.
  return req.url.startsWith('/api');
}

export const authInterceptor: HttpInterceptorFn = (req: HttpRequest<unknown>, next: HttpHandlerFn) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const token = auth.token();

  const authReq = token && shouldAttachToken(req) ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : req;

  if (token && shouldAttachToken(req)) {
    console.log('Attaching token to request:', req.url);
  }

  return next(authReq).pipe(
    catchError((err: unknown) => {
      if (err instanceof HttpErrorResponse && err.status === 401) {
        console.warn('Received 401 Unauthorized response for:', req.url);
        console.log('Logging out due to 401 error');
        auth.logout();
        router.navigateByUrl('/login');
      }

      return throwError(() => err);
    })
  );
};
