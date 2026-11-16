import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

export const interceptorToken: HttpInterceptorFn = (solicitud, siguiente) => {
  const router = inject(Router);
  const token = localStorage.getItem('itinera_token');

  if (!token) {
    return siguiente(solicitud);
  }

  return siguiente(
    solicitud.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    })
  ).pipe(
    catchError(error => {
      if (error instanceof HttpErrorResponse && error.status === 403) {
        void router.navigateByUrl('/acceso-denegado');
      }

      if (error instanceof HttpErrorResponse && error.status === 401) {
        localStorage.removeItem('itinera_token');
        localStorage.removeItem('itinera_usuario');
        void router.navigateByUrl('/login');
      }

      return throwError(() => error);
    })
  );
};
