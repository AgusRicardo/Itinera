import { Component, computed, forwardRef, input, signal } from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';

import { DIAS_SEMANA, armarMes, formatearFecha, sumarMeses } from '../../../core/utils/calendario';
import { fechaLocalIso } from '../../../core/utils/fechas';
import { CampoFlotante } from '../campo-flotante';

@Component({
  selector: 'app-selector-fecha',
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => SelectorFechaComponent),
    multi: true
  }],
  templateUrl: './selector-fecha.component.html'
})
export class SelectorFechaComponent extends CampoFlotante<string> {
  readonly min = input('');
  readonly max = input('');
  readonly idCampo = input('');
  readonly descripcion = input.required<string>();

  protected readonly diasSemana = DIAS_SEMANA;
  protected readonly formatear = formatearFecha;
  protected readonly mesVisible = signal('');
  protected readonly mes = computed(() => armarMes(this.mesVisible()));

  protected readonly hayMesAnterior = computed(() =>
    !this.min() || this.mesVisible() > this.min().slice(0, 7));

  protected readonly hayMesSiguiente = computed(() =>
    !this.max() || this.mesVisible() < this.max().slice(0, 7));

  protected alternar(): void {
    if (this.abierto()) {
      this.cerrar();
      return;
    }

    this.mesVisible.set((this.valor() || this.min() || fechaLocalIso(new Date())).slice(0, 7));
    this.abrir(345, 300);
  }

  protected moverMes(cantidad: number): void {
    this.mesVisible.update(mes => sumarMeses(mes, cantidad));
  }

  protected estaDeshabilitado(fecha: string): boolean {
    return (!!this.min() && fecha < this.min()) || (!!this.max() && fecha > this.max());
  }
}
