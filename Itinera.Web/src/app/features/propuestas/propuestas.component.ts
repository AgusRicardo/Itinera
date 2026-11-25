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
import { AccionMenu, MenuAccionesComponent } from '../../shared/components/menu-acciones/menu-acciones.component';
import { MonedaPipe } from '../../shared/pipes/moneda.pipe';
import { GrillaPaginada } from '../../core/utils/grilla-paginada';
import { ColumnaOrdenableComponent } from '../../shared/components/columna-ordenable/columna-ordenable.component';
import { FiltrosGrillaComponent } from '../../shared/components/filtros-grilla/filtros-grilla.component';
import { EstadoComponent } from '../../shared/components/estado/estado.component';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';
import { ConfirmacionService } from '../../core/services/confirmacion.service';
import { ToastrService } from '../../core/services/toastr.service';
import { EncabezadoComponent } from '../../shared/components/encabezado/encabezado.component';
import { TablaComponent } from '../../shared/components/tabla/tabla.component';

@Component({
  selector: 'app-propuestas',
  imports: [
    MonedaPipe,
    DatePipe,
    DecimalPipe,
    MenuAccionesComponent,
    ColumnaOrdenableComponent,
    FiltrosGrillaComponent,
    EstadoComponent,
    MenuPrincipalComponent,
    RouterLink,
    EncabezadoComponent,
    TablaComponent
  ],
  templateUrl: './propuestas.component.html'
})
export class PropuestasComponent {
  private readonly confirmacion = inject(ConfirmacionService);
  private readonly toastr = inject(ToastrService);
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
  protected readonly estados = signal<EstadoPropuesta[]>([]);
  protected readonly eliminando = signal<number | null>(null);

  protected acciones(propuesta: Propuesta): AccionMenu[] {
    const ruta = ['/propuestas', propuesta.id.toString()];
    const acciones: AccionMenu[] = [{ texto: 'Ver detalle', ruta }];

    if (this.autenticacion.tienePermiso('propuestas.editar')
      && !['Aceptada', 'Eliminada'].includes(propuesta.estado)) {
      acciones.push({ texto: 'Editar', ruta: [...ruta, 'editar'] });
    }

    if (this.autenticacion.tienePermiso('propuestas.eliminar')) {
      acciones.push({
        texto: 'Eliminar',
        peligro: true,
        deshabilitada: this.eliminando() === propuesta.id,
        ejecutar: () => this.eliminar(propuesta)
      });
    }

    return acciones;
  }

  constructor() {
    this.cargarResumen();
    this.cargarEstados();
  }

  private async eliminar(propuesta: Propuesta): Promise<void> {
    const confirmado = await this.confirmacion.confirmar({
      titulo: `¿Eliminar la propuesta #${propuesta.id}?`,
      mensaje: `Es de ${propuesta.clienteNombre}.`,
      accion: 'Eliminar',
      peligro: true
    });

    if (!confirmado) {
      return;
    }

    this.eliminando.set(propuesta.id);
    this.propuestasService.eliminar(propuesta.id).subscribe({
      next: () => {
        this.grilla.cargar();
        this.cargarResumen();
        this.eliminando.set(null);
        this.toastr.exito('Propuesta eliminada.');
      },
      error: () => this.eliminando.set(null)
    });
  }

  protected puedeCrear(): boolean {
    return this.autenticacion.tienePermiso('propuestas.crear');
  }

  private cargarResumen(): void {
    this.propuestasService.obtenerResumen().subscribe(resumen => this.resumen.set(resumen));
  }

  private cargarEstados(): void {
    this.estadosService.obtenerTodos().subscribe(estados =>
      this.estados.set(estados.filter(estado => estado.descripcion !== 'Eliminada')));
  }
}
