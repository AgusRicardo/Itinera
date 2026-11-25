import { Component, input, model, signal } from '@angular/core';

import { CampoFlotante } from '../campo-flotante';

const dosDigitos = (cantidad: number, paso: number): string[] =>
  Array.from({ length: cantidad }, (_, indice) => String(indice * paso).padStart(2, '0'));

@Component({
  selector: 'app-selector-hora',
  templateUrl: './selector-hora.component.html'
})
export class SelectorHoraComponent extends CampoFlotante<string> {
  readonly hora = model.required<string>();
  readonly descripcion = input.required<string>();

  protected readonly horas = dosDigitos(24, 1);
  protected readonly minutos = signal<string[]>([]);

  protected alternar(): void {
    if (this.abierto()) {
      this.cerrar();
      return;
    }

    const opciones = dosDigitos(12, 5);
    const minuto = this.hora().slice(3);
    this.minutos.set(opciones.includes(minuto) ? opciones : [...opciones, minuto].sort());
    this.abrir(232, 152);
    setTimeout(() => this.elemento.nativeElement
      .querySelectorAll<HTMLElement>('[aria-pressed="true"]')
      .forEach(boton => boton.parentElement!.scrollTop = boton.offsetTop - 90));
  }

  protected elegirHora(hora: string): void {
    this.hora.set(`${hora}:${this.hora().slice(3)}`);
  }

  protected elegirMinuto(minuto: string): void {
    this.hora.set(`${this.hora().slice(0, 2)}:${minuto}`);
    this.cerrar();
  }
}
