import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';

import { MigasComponent } from '../migas/migas.component';
import { AutenticacionService } from '../../../core/services/autenticacion.service';
import { MenuService } from '../../../core/services/menu.service';
import { TooltipDirective } from '../tooltip/tooltip.directive';

@Component({
  selector: 'app-menu-principal',
  imports: [
    RouterLink,
    RouterLinkActive,
    MigasComponent,
    TooltipDirective
  ],
  templateUrl: './menu-principal.component.html'
})
export class MenuPrincipalComponent {
  protected readonly autenticacion = inject(AutenticacionService);
  private readonly menu = inject(MenuService);
  private readonly router = inject(Router);

  protected readonly contraido = this.menu.contraido;
  protected readonly usuario = this.autenticacion.obtenerUsuario();
  protected readonly empresa = this.usuario?.empresaRazonSocial
    || (this.autenticacion.tienePermiso('plataforma.empresas.gestionar') ? 'Plataforma' : '');

  protected alternar(): void {
    this.menu.alternar();
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
