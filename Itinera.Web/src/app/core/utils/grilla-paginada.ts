import { signal } from '@angular/core';
import { Observable, Subscription } from 'rxjs';

import { ConsultaPagina, Pagina } from '../models/pagina';

export class GrillaPaginada<T> {
  private consulta?: Subscription;
  private espera?: ReturnType<typeof setTimeout>;
  private filtros: Record<string, string | number> = {};

  readonly elementos = signal<T[]>([]);
  readonly total = signal(0);
  readonly pagina = signal(1);
  readonly tamanoPagina = signal(10);
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

  filtrar(filtros: Record<string, string | number>): void {
    this.filtros = Object.fromEntries(
      Object.entries(filtros).filter(([, valor]) => valor !== ''));
    this.pagina.set(1);
    clearTimeout(this.espera);
    this.espera = setTimeout(() => this.cargar(), 300);
  }
}
