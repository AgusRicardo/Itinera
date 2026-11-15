import { HttpInterceptorFn } from '@angular/common/http';

export const interceptorToken: HttpInterceptorFn = (solicitud, siguiente) => {
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
  );
};
