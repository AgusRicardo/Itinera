import { Component, HostListener, inject, signal } from '@angular/core';
import { Router } from '@angular/router';

import { AutenticacionService } from '../../../core/services/autenticacion.service';

@Component({
  selector: 'app-menu-usuario',
  imports: [],
  templateUrl: './menu-usuario.component.html'
})
export class MenuUsuarioComponent {
  private readonly autenticacion = inject(AutenticacionService);
  private readonly router = inject(Router);

  protected readonly abierto = signal(false);
  protected readonly usuario = this.autenticacion.obtenerUsuario();

  protected alternar(event: MouseEvent): void {
    event.stopPropagation();
    this.abierto.update(valor => !valor);
  }

  @HostListener('document:click')
  protected cerrarAlHacerClickFuera(): void {
    this.abierto.set(false);
  }

  protected cerrarSesion(): void {
    this.autenticacion.cerrarSesion();
    void this.router.navigateByUrl('/login');
  }
}
