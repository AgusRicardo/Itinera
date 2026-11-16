import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AutenticacionService } from '../services/autenticacion.service';

export const guardPermiso: CanActivateFn = (route) => {
  const autenticacion = inject(AutenticacionService);
  const router = inject(Router);
  const permisos = (route.data['permisos'] as string[] | undefined) ?? [];

  return permisos.every(permiso => autenticacion.tienePermiso(permiso))
    ? true
    : router.createUrlTree(['/acceso-denegado']);
};
