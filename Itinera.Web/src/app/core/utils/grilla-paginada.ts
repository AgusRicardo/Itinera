import { signal } from '@angular/core';
import { Observable, Subscription } from 'rxjs';

import { ConsultaPagina, FiltrosGrilla, Pagina } from '../models/pagina';

const minimoLetras = 3;

export class GrillaPaginada<T> {
  private consulta?: Subscription;
  private espera?: ReturnType<typeof setTimeout>;
  private filtros: FiltrosGrilla = {};

  readonly elementos = signal<T[]>([]);
  readonly total = signal(0);
  readonly pagina = signal(1);
  readonly tamanoPagina = signal(10);
  readonly orden = signal<string | null>(null);
  readonly descendente = signal(false);
  readonly cargando = signal(true);
  readonly error = signal<string | null>(null);

  constructor(
    private readonly obtener: (consulta: ConsultaPagina) => Observable<Pagina<T>>,
    private readonly mensajeError: string
  ) {
    this.cargar();
  }

  cargar(): void {
    clearTimeout(this.espera);
    this.consulta?.unsubscribe();
    this.consulta = this.obtener({
      ...this.filtros,
      ...(this.orden() ? { orden: this.orden()!, descendente: this.descendente() } : {}),
      pagina: this.pagina(),
      tamanoPagina: this.tamanoPagina()
    }).subscribe({
      next: respuesta => {
        this.elementos.set(respuesta.elementos);
        this.total.set(respuesta.total);
        this.pagina.set(respuesta.numeroPagina);
        this.error.set(null);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set(this.mensajeError);
        this.cargando.set(false);
      }
    });
  }

  irA(pagina: number): void {
    this.pagina.set(pagina);
    this.cargar();
  }

  cambiarTamano(tamanoPagina: number): void {
    this.tamanoPagina.set(tamanoPagina);
    this.irA(1);
  }

  ordenarPor(campo: string): void {
    const yaOrdenada = this.orden() === campo;

    if (yaOrdenada && this.descendente()) {
      this.orden.set(null);
      this.descendente.set(false);
    } else {
      this.orden.set(campo);
      this.descendente.set(yaOrdenada);
    }

    this.irA(1);
  }

  filtrar(filtros: FiltrosGrilla): void {
    const nuevos = Object.fromEntries(
      Object.entries(filtros).filter(([nombre, valor]) =>
        valor !== ''
        && !(Array.isArray(valor) && !valor.length)
        && !(nombre === 'texto' && String(valor).length < minimoLetras)));

    if (JSON.stringify(nuevos) === JSON.stringify(this.filtros)) {
      return;
    }

    this.filtros = nuevos;
    this.pagina.set(1);
    clearTimeout(this.espera);
    this.espera = setTimeout(() => this.cargar(), 400);
  }
}
