import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, throwError } from 'rxjs';

/**
 * Catches HTTP errors and surfaces a MatSnackBar notification.
 * Prefers the RFC 7807 ProblemDetails `detail`/`title` returned by the API.
 */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const snackBar = inject(MatSnackBar);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let message = 'An unexpected error occurred.';

      if (error.error && typeof error.error === 'object') {
        message = error.error.detail || error.error.title || message;
      } else if (error.status === 0) {
        message = 'Unable to reach the server. Is the API running?';
      } else if (typeof error.error === 'string' && error.error.length) {
        message = error.error;
      }

      snackBar.open(message, 'Dismiss', { duration: 5000, panelClass: 'error-snackbar' });
      return throwError(() => error);
    })
  );
};
