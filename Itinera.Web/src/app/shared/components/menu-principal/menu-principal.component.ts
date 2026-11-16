import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';

import { AutenticacionService } from '../../../core/services/autenticacion.service';

@Component({
  selector: 'app-menu-principal',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './menu-principal.component.html'
})
export class MenuPrincipalComponent {
  protected readonly autenticacion = inject(AutenticacionService);
  private readonly router = inject(Router);

  protected readonly contraido = signal(false);
  protected readonly usuario = this.autenticacion.obtenerUsuario();

  protected alternar(): void {
    this.contraido.update(valor => !valor);
  }

  protected cerrarSesion(): void {
    this.autenticacion.cerrarSesion();
    void this.router.navigateByUrl('/login');
  }

  protected irInicio(): void {
    const ruta = this.autenticacion.tienePermiso('plataforma.empresas.gestionar')
      ? '/administracion/empresas'
      : this.autenticacion.tienePermiso('propuestas.ver')
        ? '/propuestas'
        : this.autenticacion.tienePermiso('clientes.ver')
          ? '/clientes'
          : '/acceso-denegado';

    void this.router.navigateByUrl(ruta);
  }
}
