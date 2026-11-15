import { Component, EventEmitter, HostListener, Input, Output, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-menu-acciones',
  imports: [RouterLink],
  templateUrl: './menu-acciones.component.html'
})
export class MenuAccionesComponent {
  @Input({ required: true }) rutaVer: string[] = [];
  @Input() puedeEliminar = true;
  @Output() eliminar = new EventEmitter<void>();

  protected readonly abierto = signal(false);

  protected alternar(event: MouseEvent): void {
    event.stopPropagation();
    this.abierto.update(valor => !valor);
  }

  @HostListener('document:click')
  protected cerrarAlHacerClickFuera(): void {
    this.abierto.set(false);
  }

  protected ejecutarEliminacion(): void {
    this.abierto.set(false);
    this.eliminar.emit();
  }
}
