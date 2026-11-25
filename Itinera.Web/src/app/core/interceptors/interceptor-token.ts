import { HttpContextToken, HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AutenticacionService } from '../services/autenticacion.service';
import { ToastrService } from '../services/toastr.service';
import { catchError, throwError } from 'rxjs';

/** Para los pedidos cuya pantalla ya muestra el error. */
export const SIN_AVISO = new HttpContextToken(() => false);

const mensajeDe = (error: HttpErrorResponse): string => {
  const validaciones = Object.values<string[]>(error.error?.errors ?? {}).flat();

  return error.error?.detail ?? validaciones[0] ?? 'No se pudo completar la operación.';
};

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
        && ![401, 403].includes(error.status)
        && solicitud.method !== 'GET'
        && !solicitud.context.get(SIN_AVISO)
      ) {
        toastr.error(mensajeDe(error));
      }

      if (error instanceof HttpErrorResponse && error.status === 401) {
        autenticacion.cerrarSesion();
        void router.navigateByUrl('/login');
      }

      return throwError(() => error);
    })
  );
};
