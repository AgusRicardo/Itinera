import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AutenticacionService } from '../services/autenticacion.service';
import { ToastrService } from '../services/toastr.service';
import { catchError, throwError } from 'rxjs';

export const interceptorToken: HttpInterceptorFn = (solicitud, siguiente) => {
  const router = inject(Router);
  const toastr = inject(ToastrService);
  const autenticacion = inject(AutenticacionService);
  const token = autenticacion.obtenerToken();
  const solicitudFinal = token
    ? solicitud.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : solicitud;

  return siguiente(solicitudFinal).pipe(
    catchError(error => {
      if (error instanceof HttpErrorResponse && error.status === 403) {
        toastr.error('No tenés permisos para realizar esta operación.');
        void router.navigateByUrl('/acceso-denegado');
      }

      if (
        error instanceof HttpErrorResponse
        && [400, 409, 422, 500].includes(error.status)
      ) {
        toastr.error(
          error.error?.detail
          ?? error.error?.title
          ?? 'No se pudo completar la operación.'
        );
      }

      if (error instanceof HttpErrorResponse && error.status === 401) {
        autenticacion.cerrarSesion();
        void router.navigateByUrl('/login');
      }

      return throwError(() => error);
    })
  );
};
