import { Component, computed, forwardRef, input, signal } from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';

import { sinTildes } from '../../../core/utils/texto';
import { CampoFlotante } from '../campo-flotante';

export interface OpcionSelector {
  id: number;
  nombre: string;
}

const altoMaximo = 288;
const opcionesSinBuscador = 8;

@Component({
  selector: 'app-selector',
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => SelectorComponent),
    multi: true
  }],
  templateUrl: './selector.component.html'
})
export class SelectorComponent extends CampoFlotante<number> {
  readonly opciones = input.required<OpcionSelector[]>();
  readonly placeholder = input('Seleccioná una opción');
  readonly descripcion = input.required<string>();
  readonly idCampo = input('');

  protected readonly busqueda = signal('');
  protected readonly conBuscador = computed(() => this.opciones().length > opcionesSinBuscador);
  protected readonly seleccionada = computed(() =>
    this.opciones().find(opcion => opcion.id === this.valor())?.nombre ?? '');

  protected readonly visibles = computed(() => {
    const texto = sinTildes(this.busqueda().trim());

    return this.opciones().filter(opcion => sinTildes(opcion.nombre).includes(texto));
  });

  protected alternar(): void {
    if (this.abierto()) {
      this.cerrar();
      return;
    }

    this.busqueda.set('');
    this.abrir(Math.min(altoMaximo, this.opciones().length * 36 + (this.conBuscador() ? 60 : 14)));
    setTimeout(() => {
      const elegida = this.opciones().findIndex(opcion => opcion.id === this.valor());
      this.enfocables()[this.conBuscador() ? 0 : Math.max(0, elegida)]?.focus();
    });
  }

  protected mover(evento: Event, pasos: number): void {
    evento.preventDefault();
    const campos = this.enfocables();
    const actual = campos.indexOf(document.activeElement as HTMLElement);
    campos[Math.max(0, Math.min(campos.length - 1, actual + pasos))]?.focus();
  }

  private enfocables(): HTMLElement[] {
    return Array.from(this.elemento.nativeElement
      .querySelectorAll<HTMLElement>('[data-panel] input, [data-panel] button'));
  }
}
