import { Component, OnInit, computed, inject, input, output, signal } from '@angular/core';

import { FiltrosGrilla } from '../../../core/models/pagina';
import { formatearFecha } from '../../../core/utils/calendario';
import { ClientesService } from '../../../core/services/clientes.service';
import { BuscadorOpcionesComponent, OpcionBuscable } from '../buscador-opciones/buscador-opciones.component';
import { RangoFechas, RangoFechasComponent } from '../rango-fechas/rango-fechas.component';

export interface EstadoFiltrable {
  id: number;
  descripcion: string;
}

const sinFechas: RangoFechas = { desde: '', hasta: '' };

@Component({
  selector: 'app-filtros-grilla',
  imports: [BuscadorOpcionesComponent, RangoFechasComponent],
  templateUrl: './filtros-grilla.component.html'
})
export class FiltrosGrillaComponent implements OnInit {
  private readonly clientesService = inject(ClientesService);

  readonly estados = input<EstadoFiltrable[]>([]);
  readonly etiquetaEstados = input('Estados');
  readonly etiquetaOpciones = input('Cliente');
  readonly opciones = input<OpcionBuscable[] | null>(null);
  readonly conFiltros = input(true);
  readonly conOpciones = input(true);
  readonly conFechas = input(true);
  readonly placeholder = input('Buscar');
  readonly etiquetaFechas = input('Fechas');
  readonly cambio = output<FiltrosGrilla>();

  protected readonly abierto = signal(false);
  protected readonly texto = signal('');
  protected readonly estadoIds = signal<number[]>([]);
  protected readonly opcion = signal<OpcionBuscable | null>(null);
  protected readonly fechas = signal<RangoFechas>(sinFechas);
  private readonly clientes = signal<OpcionBuscable[]>([]);
  protected readonly opcionesVisibles = computed(() => this.opciones() ?? this.clientes());

  protected readonly cantidad = computed(() =>
    this.estadoIds().length + (this.opcion() ? 1 : 0) + (this.fechas().desde ? 1 : 0));

  protected readonly etiquetas = computed(() => [
    ...this.estados()
      .filter(estado => this.estadoIds().includes(estado.id))
      .map(estado => ({ texto: estado.descripcion, quitar: () => this.alternarEstado(estado.id) })),
    ...(this.opcion()
      ? [{ texto: this.opcion()!.nombre, quitar: () => this.elegirOpcion(null) }]
      : []),
    ...(this.fechas().desde
      ? [{
        texto: `${formatearFecha(this.fechas().desde)} – ${formatearFecha(this.fechas().hasta)}`,
        quitar: () => this.elegirFechas(sinFechas)
      }]
      : [])
  ]);

  ngOnInit(): void {
    if (!this.conFiltros() || !this.conOpciones() || this.opciones()) {
      return;
    }

    this.clientesService.obtenerTodos().subscribe({
      next: clientes => this.clientes.set(clientes.map(cliente => ({
        id: cliente.id.toString(),
        nombre: `${cliente.nombre} ${cliente.apellido}`.trim()
      }))),
      error: () => this.clientes.set([])
    });
  }

  protected buscar(texto: string): void {
    this.texto.set(texto);
    this.emitir();
  }

  protected alternarEstado(id: number): void {
    this.estadoIds.update(ids => ids.includes(id) ? ids.filter(item => item !== id) : [...ids, id]);
    this.emitir();
  }

  protected elegirOpcion(opcion: OpcionBuscable | null): void {
    this.opcion.set(opcion);
    this.emitir();
  }

  protected elegirFechas(fechas: RangoFechas): void {
    this.fechas.set(fechas);
    this.emitir();
  }

  protected limpiar(): void {
    this.estadoIds.set([]);
    this.opcion.set(null);
    this.fechas.set(sinFechas);
    this.emitir();
  }

  private emitir(): void {
    this.cambio.emit({
      texto: this.texto().trim(),
      estadoIds: this.estadoIds(),
      clienteId: this.opcion()?.id ?? '',
      desde: this.comienzoDelDia(this.fechas().desde),
      hasta: this.comienzoDelDia(this.fechas().hasta, 1)
    });
  }

  private comienzoDelDia(fecha: string, diasDespues = 0): string {
    if (!fecha) {
      return '';
    }

    const [anio, mes, dia] = fecha.split('-').map(Number);

    return new Date(anio, mes - 1, dia + diasDespues).toISOString();
  }
}
