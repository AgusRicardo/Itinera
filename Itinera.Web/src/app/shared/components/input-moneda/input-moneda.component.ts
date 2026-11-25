import { Component, ElementRef, Input, forwardRef, signal, viewChild } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'app-input-moneda',
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => InputMonedaComponent),
    multi: true
  }],
  template: `
    <div class="flex items-center rounded-lg border bg-itinera-secondary text-sm focus-within:border-itinera-accent focus-within:ring-2 focus-within:ring-itinera-accent/30">
      @if (prefijo) {
        <span class="select-none pl-3 text-itinera-muted" aria-hidden="true">{{ prefijo }}</span>
      }
      <input
        #campo
        type="text"
        inputmode="decimal"
        autocomplete="off"
        [id]="idCampo"
        [value]="texto()"
        [disabled]="deshabilitado()"
        class="w-full min-w-0 bg-transparent px-3 py-2.5 outline-none"
        (input)="escribir($any($event))"
        (blur)="alTocar()"
      />
    </div>
  `
})
export class InputMonedaComponent implements ControlValueAccessor {
  @Input() idCampo = '';
  @Input() prefijo = 'USD';
  @Input() decimales = 2;

  private readonly campo = viewChild.required<ElementRef<HTMLInputElement>>('campo');
  private alCambiar: (valor: number | null) => void = () => undefined;

  protected readonly texto = signal('');
  protected readonly deshabilitado = signal(false);
  protected alTocar: () => void = () => undefined;

  writeValue(valor: number | null): void {
    this.texto.set(valor === null || valor === undefined
      ? ''
      : valor.toLocaleString('es-AR', { maximumFractionDigits: this.decimales }));
  }

  registerOnChange(funcion: (valor: number | null) => void): void {
    this.alCambiar = funcion;
  }

  registerOnTouched(funcion: () => void): void {
    this.alTocar = funcion;
  }

  setDisabledState(deshabilitado: boolean): void {
    this.deshabilitado.set(deshabilitado);
  }

  protected escribir(evento: InputEvent): void {
    const campo = this.campo().nativeElement;
    const posicion = campo.selectionStart ?? campo.value.length;
    let escrito = campo.value;

    if (evento.data === '.' && this.decimales) {
      escrito = `${escrito.slice(0, posicion - 1)},${escrito.slice(posicion)}`;
    } else if (evento.inputType === 'insertFromPaste' && /^\d+\.\d{1,2}$/.test(escrito.trim())) {
      escrito = escrito.replace('.', ',');
    }

    const significativos = this.limpiar(escrito.slice(0, posicion)).length;
    const [entero, ...decimales] = this.limpiar(escrito).split(',');
    const enteroSinCeros = entero.replace(/^0+(?=\d)/, '');
    const parteDecimal = decimales.join('').slice(0, this.decimales);
    const formateado = enteroSinCeros.replace(/\B(?=(\d{3})+(?!\d))/g, '.')
      + (decimales.length && this.decimales ? `,${parteDecimal}` : '');

    this.texto.set(formateado);
    campo.value = formateado;
    this.ubicarCursor(campo, significativos - (entero.length - enteroSinCeros.length));
    this.alCambiar(/\d/.test(formateado)
      ? Number(`${enteroSinCeros || '0'}.${parteDecimal || '0'}`)
      : null);
  }

  private limpiar(texto: string): string {
    return texto.replace(/[^\d,]/g, '');
  }

  private ubicarCursor(campo: HTMLInputElement, significativos: number): void {
    let posicion = 0;

    for (let restantes = significativos; restantes > 0 && posicion < campo.value.length; posicion++) {
      if (campo.value[posicion] !== '.') {
        restantes--;
      }
    }

    campo.setSelectionRange(posicion, posicion);
  }
}
