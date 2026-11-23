import { Component, ElementRef, computed, inject, input, output, signal } from '@angular/core';

export interface RangoFechas {
  desde: string;
  hasta: string;
}

@Component({
  selector: 'app-rango-fechas',
  host: {
    class: 'relative block',
    '(document:mousedown)': 'cerrarSiEsAfuera($event)',
    '(keydown.escape)': 'cerrar()'
  },
  templateUrl: './rango-fechas.component.html'
})
export class RangoFechasComponent {
  private readonly elemento = inject<ElementRef<HTMLElement>>(ElementRef);

  readonly desde = input('');
  readonly hasta = input('');
  readonly min = input('');
  readonly max = input('');
  readonly idCampo = input('');
  readonly descripcion = input('Fechas');
  readonly alineadoDerecha = input(false);
  readonly cambio = output<RangoFechas>();

  protected readonly abierto = signal(false);
  protected readonly inicioElegido = signal<string | null>(null);
  protected readonly diaSenalado = signal<string | null>(null);
  protected readonly mesVisible = signal('');
  protected readonly diasSemana = ['Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sá', 'Do'];

  protected readonly resumen = computed(() =>
    this.desde() && this.hasta()
      ? `${this.formatear(this.desde())} – ${this.formatear(this.hasta())}`
      : '');

  private readonly rango = computed<RangoFechas>(() => {
    const inicio = this.inicioElegido();

    if (!inicio) {
      return { desde: this.desde(), hasta: this.hasta() };
    }

    const senalado = this.diaSenalado();

    return { desde: inicio, hasta: senalado && senalado > inicio ? senalado : inicio };
  });

  protected readonly noches = computed(() => {
    const { desde, hasta } = this.rango();

    return desde && hasta
      ? Math.round((Date.parse(hasta) - Date.parse(desde)) / 86_400_000)
      : 0;
  });

  protected readonly meses = computed(() =>
    [0, 1].map(desplazamiento => this.armarMes(this.sumarMeses(this.mesVisible(), desplazamiento))));

  protected readonly hayMesAnterior = computed(() =>
    !this.min() || this.mesVisible() > this.min().slice(0, 7));

  protected readonly hayMesSiguiente = computed(() =>
    !this.max() || this.sumarMeses(this.mesVisible(), 1) <= this.max().slice(0, 7));

  protected alternar(): void {
    if (this.abierto()) {
      this.cerrar();
      return;
    }

    const hoy = new Date();
    const mesActual = `${hoy.getFullYear()}-${String(hoy.getMonth() + 1).padStart(2, '0')}`;
    this.mesVisible.set((this.desde() || this.min()).slice(0, 7) || mesActual);
    this.abierto.set(true);
  }

  protected cerrar(): void {
    this.abierto.set(false);
    this.inicioElegido.set(null);
    this.diaSenalado.set(null);
  }

  protected cerrarSiEsAfuera(evento: Event): void {
    if (this.abierto() && !this.elemento.nativeElement.contains(evento.target as Node)) {
      this.cerrar();
    }
  }

  protected moverMes(cantidad: number): void {
    this.mesVisible.update(mes => this.sumarMeses(mes, cantidad));
  }

  protected elegir(fecha: string): void {
    const inicio = this.inicioElegido();

    if (!inicio || fecha < inicio) {
      this.inicioElegido.set(fecha);
      this.diaSenalado.set(null);
      return;
    }

    this.cambio.emit({ desde: inicio, hasta: fecha });
    this.cerrar();
  }

  protected estaDeshabilitado(fecha: string): boolean {
    return (!!this.min() && fecha < this.min()) || (!!this.max() && fecha > this.max());
  }

  protected esExtremo(fecha: string): boolean {
    const { desde, hasta } = this.rango();

    return fecha === desde || fecha === hasta;
  }

  protected estaEnRango(fecha: string): boolean {
    const { desde, hasta } = this.rango();

    return !!desde && !!hasta && fecha >= desde && fecha <= hasta;
  }

  protected formatear(fecha: string): string {
    return fecha.split('-').reverse().join('/');
  }

  private armarMes(mes: string) {
    const [anio, numero] = mes.split('-').map(Number);
    const primero = new Date(Date.UTC(anio, numero - 1, 1));
    const cantidad = new Date(Date.UTC(anio, numero, 0)).getUTCDate();
    const titulo = primero.toLocaleDateString('es-AR', {
      month: 'long',
      year: 'numeric',
      timeZone: 'UTC'
    });

    return {
      clave: mes,
      titulo: titulo.charAt(0).toUpperCase() + titulo.slice(1),
      huecos: Array.from({ length: (primero.getUTCDay() + 6) % 7 }, (_, indice) => indice),
      dias: Array.from({ length: cantidad }, (_, indice) =>
        `${mes}-${String(indice + 1).padStart(2, '0')}`)
    };
  }

  private sumarMeses(mes: string, cantidad: number): string {
    const [anio, numero] = mes.split('-').map(Number);

    return new Date(Date.UTC(anio, numero - 1 + cantidad, 1)).toISOString().slice(0, 7);
  }
}
