import { Component, EventEmitter, HostListener, Input, Output, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-menu-acciones',
  imports: [RouterLink],
  templateUrl: './menu-acciones.component.html'
})
export class MenuAccionesComponent {
  @Input({ required: true }) rutaVer: string[] = [];
  @Input() rutaEditar: string[] | null = null;
  @Input() puedeEliminar = true;
  @Output() eliminar = new EventEmitter<void>();

  protected readonly abierto = signal(false);
  protected readonly posicion = signal({ top: '0px', left: '0px' });

  protected alternar(event: MouseEvent): void {
    event.stopPropagation();
    const elemento = event.currentTarget as HTMLElement;
    const rectangulo = elemento.getBoundingClientRect();
    this.posicion.set({
      top: `${rectangulo.bottom + 4}px`,
      left: `${Math.max(8, rectangulo.right - 144)}px`
    });
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
