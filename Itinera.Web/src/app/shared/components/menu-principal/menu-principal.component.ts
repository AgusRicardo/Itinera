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

  protected administracionPlataforma(): boolean {
    return this.autenticacion.tienePermiso('plataforma.empresas.gestionar')
      || this.puedeUsarUsuarios()
      || this.puedeUsarRoles()
      || this.puedeUsarGrupos();
  }

  protected puedeUsarUsuarios(): boolean {
    return this.autenticacion.tienePermiso('plataforma.usuarios.gestionar')
      && this.autenticacion.tienePermiso('seguridad.gestionar');
  }

  protected puedeUsarRoles(): boolean {
    return this.autenticacion.tienePermiso('plataforma.roles.gestionar')
      && this.autenticacion.tienePermiso('seguridad.gestionar');
  }

  protected puedeUsarGrupos(): boolean {
    return this.autenticacion.tienePermiso('plataforma.grupos.gestionar')
      && this.autenticacion.tienePermiso('seguridad.gestionar');
  }

  protected operacion(): boolean {
    return this.autenticacion.tienePermiso('propuestas.ver')
      || this.autenticacion.tienePermiso('clientes.ver')
      || this.autenticacion.tienePermiso('facturacion.ver')
      || this.autenticacion.tienePermiso('etiquetas.gestionar')
      || this.autenticacion.tienePermiso('catalogos.operativos.gestionar');
  }

  protected irInicio(): void {
    const ruta = this.autenticacion.tienePermiso('plataforma.empresas.gestionar')
      ? '/administracion/empresas'
      : this.autenticacion.tienePermiso('propuestas.ver')
        ? '/propuestas'
        : this.autenticacion.tienePermiso('clientes.ver')
          ? '/clientes'
          : this.autenticacion.tienePermiso('catalogos.gestionar')
            ? '/administracion/cargos'
            : '/acceso-denegado';

    void this.router.navigateByUrl(ruta);
  }
}
