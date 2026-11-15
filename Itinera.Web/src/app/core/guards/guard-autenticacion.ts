import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AutenticacionService } from '../services/autenticacion.service';

export const guardAutenticacion: CanActivateFn = () => {
  const autenticacion = inject(AutenticacionService);
  const router = inject(Router);

  return autenticacion.estaAutenticado()
    ? true
    : router.createUrlTree(['/login']);
};
