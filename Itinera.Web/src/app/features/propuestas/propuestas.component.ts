import { Component, computed, inject, signal } from '@angular/core';
import { CurrencyPipe, DatePipe, DecimalPipe } from '@angular/common';

import { PropuestasService } from '../../core/services/propuestas.service';
import { Propuesta } from '../../core/models/propuesta';
import {
  EstadoPropuesta,
  EstadosPropuestasService
} from '../../core/services/estados-propuestas.service';
import { AutenticacionService } from '../../core/services/autenticacion.service';
import { MenuAccionesComponent } from '../../shared/components/menu-acciones/menu-acciones.component';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';
import { CrearPropuestaComponent } from './crear-propuesta.component';

@Component({
  selector: 'app-propuestas',
  imports: [
    CurrencyPipe,
    DatePipe,
    DecimalPipe,
    MenuAccionesComponent,
    MenuPrincipalComponent,
    CrearPropuestaComponent
  ],
  templateUrl: './propuestas.component.html'
})
export class PropuestasComponent {
  private readonly propuestasService = inject(PropuestasService);
  private readonly estadosService = inject(EstadosPropuestasService);
  private readonly autenticacion = inject(AutenticacionService);
  protected readonly propuestas = signal<Propuesta[]>([]);
  protected readonly cargando = signal(true);
  protected readonly error = signal<string | null>(null);
  protected readonly busqueda = signal('');
  protected readonly estado = signal('');
  protected readonly estados = signal<EstadoPropuesta[]>([]);
  protected readonly eliminando = signal<number | null>(null);
  protected readonly nuevaPropuestaAbierta = signal(false);

  protected readonly propuestasFiltradas = computed(() => {
    const texto = this.busqueda().trim().toLowerCase();
    const estado = this.estado();

    return this.propuestas().filter(propuesta => {
      const coincideCliente = propuesta.clienteNombre.toLowerCase().includes(texto);
      const coincideEstado = !estado || propuesta.estado === estado;

      return coincideCliente && coincideEstado;
    });
  });

  protected readonly cantidadBorradores = computed(() =>
    this.propuestas().filter(propuesta => propuesta.estado === 'Borrador').length
  );

  protected readonly cantidadPresentadas = computed(() =>
    this.propuestas().filter(propuesta => propuesta.estado === 'Presentada').length
  );

  protected iniciales(nombre: string): string {
    return nombre
      .split(' ')
      .map(parte => parte[0])
      .slice(0, 2)
      .join('');
  }

  constructor() {
    this.cargarPropuestas();
    this.cargarEstados();
  }

  protected cantidadPorEstado(descripcion: string): number {
    return this.propuestas().filter(propuesta => propuesta.estado === descripcion).length;
  }

  protected eliminar(propuesta: Propuesta): void {
    const confirmar = window.confirm(
      `¿Querés eliminar la propuesta #${propuesta.id} de ${propuesta.clienteNombre}?`
    );

    if (!confirmar) {
      return;
    }

    this.eliminando.set(propuesta.id);
    this.propuestasService.eliminar(propuesta.id).subscribe({
      next: () => {
        this.cargarPropuestas();
        this.eliminando.set(null);
      },
      error: () => {
        this.error.set('No se pudo eliminar la propuesta.');
        this.eliminando.set(null);
      }
    });
  }

  protected puedeCrear(): boolean {
    return this.autenticacion.tienePermiso('propuestas.crear');
  }

  protected puedeEliminar(): boolean {
    return this.autenticacion.tienePermiso('propuestas.eliminar');
  }

  protected abrirNuevaPropuesta(): void {
    this.nuevaPropuestaAbierta.set(true);
  }

  protected cerrarNuevaPropuesta(): void {
    this.nuevaPropuestaAbierta.set(false);
  }

  private cargarPropuestas(): void {
    this.propuestasService.obtenerTodas().subscribe({
      next: propuestas => {
        this.propuestas.set(propuestas);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set('No se pudieron cargar las propuestas.');
        this.cargando.set(false);
      }
    });
  }

  private cargarEstados(): void {
    this.estadosService.obtenerTodos().subscribe({
      next: estados => this.estados.set(estados),
      error: () => this.error.set('No se pudieron cargar los estados de propuestas.')
    });
  }
}
