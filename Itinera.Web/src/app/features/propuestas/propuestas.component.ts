import { Component, inject, signal } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';

import { PropuestasService, ResumenPropuestas } from '../../core/services/propuestas.service';
import { Propuesta } from '../../core/models/propuesta';
import {
  EstadoPropuesta,
  EstadosPropuestasService
} from '../../core/services/estados-propuestas.service';
import { AutenticacionService } from '../../core/services/autenticacion.service';
import { MenuAccionesComponent } from '../../shared/components/menu-acciones/menu-acciones.component';
import { MonedaPipe } from '../../shared/pipes/moneda.pipe';
import { GrillaPaginada } from '../../core/utils/grilla-paginada';
import { PaginadorComponent } from '../../shared/components/paginador/paginador.component';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';

@Component({
  selector: 'app-propuestas',
  imports: [
    MonedaPipe,
    DatePipe,
    DecimalPipe,
    MenuAccionesComponent,
    MenuPrincipalComponent,
    PaginadorComponent,
    RouterLink
  ],
  templateUrl: './propuestas.component.html'
})
export class PropuestasComponent {
  private readonly propuestasService = inject(PropuestasService);
  private readonly estadosService = inject(EstadosPropuestasService);
  private readonly autenticacion = inject(AutenticacionService);
  protected readonly grilla = new GrillaPaginada<Propuesta>(
    consulta => this.propuestasService.obtenerPagina(consulta),
    'No se pudieron cargar las propuestas.');
  protected readonly resumen = signal<ResumenPropuestas>({
    total: 0,
    borradores: 0,
    presentadas: 0,
    aceptadas: 0
  });
  protected readonly error = signal<string | null>(null);
  protected readonly busqueda = signal('');
  protected readonly estado = signal('');
  protected readonly estados = signal<EstadoPropuesta[]>([]);
  protected readonly eliminando = signal<number | null>(null);

  protected rutaEditar(propuesta: Propuesta): string[] | null {
    return this.autenticacion.tienePermiso('propuestas.editar')
      && !['Aceptada', 'Eliminada'].includes(propuesta.estado)
      ? ['/propuestas', propuesta.id.toString(), 'editar']
      : null;
  }

  constructor() {
    this.cargarResumen();
    this.cargarEstados();
  }

  protected filtrar(): void {
    this.grilla.filtrar({ texto: this.busqueda().trim(), estadoId: this.estado() });
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
        this.grilla.cargar();
        this.cargarResumen();
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

  private cargarResumen(): void {
    this.propuestasService.obtenerResumen().subscribe(resumen => this.resumen.set(resumen));
  }

  private cargarEstados(): void {
    this.estadosService.obtenerTodos().subscribe({
      next: estados => this.estados.set(estados),
      error: () => this.error.set('No se pudieron cargar los estados de propuestas.')
    });
  }
}
