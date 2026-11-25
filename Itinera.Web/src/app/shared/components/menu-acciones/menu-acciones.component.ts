import { Component, ElementRef, inject, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

export interface AccionMenu {
  texto: string;
  ruta?: string[];
  ejecutar?: () => void;
  peligro?: boolean;
  deshabilitada?: boolean;
}

@Component({
  selector: 'app-menu-acciones',
  imports: [RouterLink],
  host: {
    class: 'relative inline-block',
    '(document:click)': 'cerrarSiEsAfuera($event)',
    '(document:keydown.escape)': 'abierto.set(false)',
    '(window:scroll)': 'abierto.set(false)'
  },
  templateUrl: './menu-acciones.component.html'
})
export class MenuAccionesComponent {
  private readonly elemento = inject<ElementRef<HTMLElement>>(ElementRef);

  readonly acciones = input.required<AccionMenu[]>();

  protected readonly abierto = signal(false);
  protected readonly posicion = signal({ top: '0px', left: '0px' });

  protected alternar(event: MouseEvent): void {
    const rectangulo = (event.currentTarget as HTMLElement).getBoundingClientRect();
    const alto = this.acciones().length * 36 + 14;
    const entraAbajo = rectangulo.bottom + 4 + alto <= window.innerHeight;
    this.posicion.set({
      top: `${entraAbajo ? rectangulo.bottom + 4 : rectangulo.top - 4 - alto}px`,
      left: `${Math.max(8, rectangulo.right - 160)}px`
    });
    this.abierto.update(valor => !valor);
  }

  protected cerrarSiEsAfuera(evento: Event): void {
    if (!this.elemento.nativeElement.contains(evento.target as Node)) {
      this.abierto.set(false);
    }
  }

  protected ejecutar(accion: AccionMenu): void {
    this.abierto.set(false);
    accion.ejecutar?.();
  }
}
