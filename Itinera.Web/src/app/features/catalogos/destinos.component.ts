import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { Ciudad, CiudadesService } from '../../core/services/ciudades.service';
import { DestinoApi, DestinosService } from '../../core/services/destinos.service';
import { ConfirmacionService } from '../../core/services/confirmacion.service';
import { ToastrService } from '../../core/services/toastr.service';
import { GrillaPaginada } from '../../core/utils/grilla-paginada';
import { ColumnaOrdenableComponent } from '../../shared/components/columna-ordenable/columna-ordenable.component';
import { EncabezadoComponent } from '../../shared/components/encabezado/encabezado.component';
import { FiltrosGrillaComponent } from '../../shared/components/filtros-grilla/filtros-grilla.component';
import { AccionMenu, MenuAccionesComponent } from '../../shared/components/menu-acciones/menu-acciones.component';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';
import { ModalComponent } from '../../shared/components/modal/modal.component';
import { OpcionSelector, SelectorComponent } from '../../shared/components/selector/selector.component';
import { TablaComponent } from '../../shared/components/tabla/tabla.component';

@Component({
  selector: 'app-destinos',
  imports: [
    ReactiveFormsModule,
    ColumnaOrdenableComponent,
    EncabezadoComponent,
    FiltrosGrillaComponent,
    MenuAccionesComponent,
    MenuPrincipalComponent,
    ModalComponent,
    SelectorComponent,
    TablaComponent
  ],
  templateUrl: './destinos.component.html'
})
export class DestinosComponent {
  private readonly servicio = inject(DestinosService);
  private readonly ciudadesServicio = inject(CiudadesService);
  private readonly confirmacion = inject(ConfirmacionService);
  private readonly toastr = inject(ToastrService);
  private readonly constructorFormulario = inject(FormBuilder);

  protected readonly grilla = new GrillaPaginada<DestinoApi>(
    consulta => this.servicio.obtenerPagina(consulta),
    'No se pudieron cargar los destinos.');
  private readonly ciudades = signal<Ciudad[]>([]);
  protected readonly editando = signal<DestinoApi | null>(null);
  protected readonly modal = signal(false);
  protected readonly guardando = signal(false);

  protected readonly formulario = this.constructorFormulario.nonNullable.group({
    ciudadId: [0, [Validators.required, Validators.min(1)]]
  });

  protected readonly opcionesCiudades = computed<OpcionSelector[]>(() =>
    this.ciudades().map(ciudad => ({ id: ciudad.id, nombre: `${ciudad.nombre}, ${ciudad.paisNombre}` })));

  constructor() {
    this.ciudadesServicio.obtenerTodos().subscribe(ciudades => this.ciudades.set(ciudades));
  }

  protected acciones(destino: DestinoApi): AccionMenu[] {
    return [
      { texto: 'Editar', ejecutar: () => this.abrir(destino) },
      { texto: 'Eliminar', peligro: true, ejecutar: () => this.eliminar(destino) }
    ];
  }

  protected abrir(destino?: DestinoApi): void {
    this.editando.set(destino ?? null);
    this.formulario.reset({ ciudadId: destino?.ciudadId ?? 0 });
    this.modal.set(true);
  }

  protected cerrar(): void {
    if (!this.guardando()) {
      this.modal.set(false);
    }
  }

  protected guardar(): void {
    if (this.formulario.invalid) {
      this.formulario.markAllAsTouched();
      return;
    }

    this.guardando.set(true);
    const destino = this.editando();
    const { ciudadId } = this.formulario.getRawValue();
    const solicitud = destino
      ? this.servicio.actualizar(destino.id, ciudadId)
      : this.servicio.crear(ciudadId);

    solicitud.subscribe({
      next: () => {
        this.guardando.set(false);
        this.modal.set(false);
        this.grilla.cargar();
        this.toastr.exito(destino ? 'Destino actualizado.' : 'Destino creado.');
      },
      error: () => this.guardando.set(false)
    });
  }

  private async eliminar(destino: DestinoApi): Promise<void> {
    const confirmado = await this.confirmacion.confirmar({
      titulo: `¿Eliminar el destino ${destino.ciudadNombre}?`,
      mensaje: 'Las propuestas que ya lo usan no cambian.',
      accion: 'Eliminar',
      peligro: true
    });

    if (!confirmado) {
      return;
    }

    this.servicio.eliminar(destino.id).subscribe(() => {
      this.grilla.cargar();
      this.toastr.exito('Destino eliminado.');
    });
  }
}
