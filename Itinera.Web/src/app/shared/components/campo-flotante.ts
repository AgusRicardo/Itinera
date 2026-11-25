import { Directive, ElementRef, inject, signal } from '@angular/core';
import { ControlValueAccessor } from '@angular/forms';

/** Campo propio de formulario que abre un panel flotante sobre el resto de la pantalla. */
@Directive({
  host: {
    class: 'relative block',
    '(document:mousedown)': 'cerrarSiEsAfuera($event)',
    '(window:wheel)': 'cerrarSiEsAfuera($event)',
    '(window:resize)': 'cerrar()',
    '(keydown.escape)': 'escapar($event)',
    '(focusout)': 'cerrarSiSaleElFoco($event)'
  }
})
export abstract class CampoFlotante<T> implements ControlValueAccessor {
  protected readonly elemento = inject<ElementRef<HTMLElement>>(ElementRef);

  protected readonly abierto = signal(false);
  protected readonly deshabilitado = signal(false);
  protected readonly posicion = signal({ top: '0px', left: '0px', width: '0px' });
  protected readonly valor = signal<T | null>(null);
  protected alCambiar: (valor: T) => void = () => undefined;
  protected alTocar: () => void = () => undefined;

  writeValue(valor: T | null): void {
    this.valor.set(valor);
  }

  registerOnChange(funcion: (valor: T) => void): void {
    this.alCambiar = funcion;
  }

  registerOnTouched(funcion: () => void): void {
    this.alTocar = funcion;
  }

  setDisabledState(deshabilitado: boolean): void {
    this.deshabilitado.set(deshabilitado);
  }

  protected abrir(alto: number, anchoMinimo = 0): void {
    const campo = this.elemento.nativeElement.getBoundingClientRect();
    const ancho = Math.max(campo.width, anchoMinimo);
    const entraAbajo = campo.bottom + 4 + alto <= window.innerHeight;
    const arriba = entraAbajo ? campo.bottom + 4 : campo.top - 4 - alto;
    this.posicion.set({
      top: `${Math.max(8, Math.min(arriba, window.innerHeight - alto - 8))}px`,
      left: `${Math.max(8, Math.min(campo.left, window.innerWidth - ancho - 8))}px`,
      width: `${ancho}px`
    });
    this.abierto.set(true);
  }

  protected cerrar(): void {
    if (this.abierto()) {
      this.abierto.set(false);
      this.alTocar();
    }
  }

  protected cerrarSiEsAfuera(evento: Event): void {
    if (!this.elemento.nativeElement.contains(evento.target as Node)) {
      this.cerrar();
    }
  }

  protected cerrarSiSaleElFoco(evento: FocusEvent): void {
    if (evento.relatedTarget && !this.elemento.nativeElement.contains(evento.relatedTarget as Node)) {
      this.cerrar();
    }
  }

  protected escapar(evento: Event): void {
    if (this.abierto()) {
      evento.stopPropagation();
      this.cerrar();
    }
  }

  protected elegirValor(valor: T): void {
    this.valor.set(valor);
    this.alCambiar(valor);
    this.cerrar();
    this.elemento.nativeElement.querySelector('button')?.focus();
  }
}
