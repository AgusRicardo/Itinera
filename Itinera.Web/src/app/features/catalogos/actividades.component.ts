import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { FiltrosGrilla } from '../../core/models/pagina';
import { ActividadDisponible, ActividadesService } from '../../core/services/actividades.service';
import { DestinoApi, DestinosService } from '../../core/services/destinos.service';
import { ConfirmacionService } from '../../core/services/confirmacion.service';
import { ToastrService } from '../../core/services/toastr.service';
import { GrillaPaginada } from '../../core/utils/grilla-paginada';
import { ColumnaOrdenableComponent } from '../../shared/components/columna-ordenable/columna-ordenable.component';
import { EncabezadoComponent } from '../../shared/components/encabezado/encabezado.component';
import { FiltrosGrillaComponent } from '../../shared/components/filtros-grilla/filtros-grilla.component';
import { InputMonedaComponent } from '../../shared/components/input-moneda/input-moneda.component';
import { AccionMenu, MenuAccionesComponent } from '../../shared/components/menu-acciones/menu-acciones.component';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';
import { ModalComponent } from '../../shared/components/modal/modal.component';
import { OpcionSelector, SelectorComponent } from '../../shared/components/selector/selector.component';
import { TablaComponent } from '../../shared/components/tabla/tabla.component';
import { MonedaPipe } from '../../shared/pipes/moneda.pipe';

@Component({
  selector: 'app-actividades',
  imports: [
    ReactiveFormsModule,
    ColumnaOrdenableComponent,
    EncabezadoComponent,
    FiltrosGrillaComponent,
    InputMonedaComponent,
    MenuAccionesComponent,
    MenuPrincipalComponent,
    ModalComponent,
    SelectorComponent,
    TablaComponent,
    MonedaPipe
  ],
  templateUrl: './actividades.component.html'
})
export class ActividadesComponent {
  private readonly servicio = inject(ActividadesService);
  private readonly destinosServicio = inject(DestinosService);
  private readonly confirmacion = inject(ConfirmacionService);
  private readonly toastr = inject(ToastrService);
  private readonly constructorFormulario = inject(FormBuilder);

  protected readonly grilla = new GrillaPaginada<ActividadDisponible>(
    consulta => this.servicio.obtenerPagina(consulta),
    'No se pudieron cargar las actividades.');
  private readonly destinos = signal<DestinoApi[]>([]);
  protected readonly editando = signal<ActividadDisponible | null>(null);
  protected readonly modal = signal(false);
  protected readonly guardando = signal(false);

  protected readonly formulario = this.constructorFormulario.nonNullable.group({
    nombre: ['', Validators.required],
    descripcion: [''],
    costoBase: [0, [Validators.required, Validators.min(0)]],
    duracionEstimada: [1, [Validators.required, Validators.min(1)]],
    destinoId: [0, [Validators.required, Validators.min(1)]]
  });

  protected readonly opcionesDestinos = computed<OpcionSelector[]>(() =>
    this.destinos().map(destino => ({ id: destino.id, nombre: `${destino.ciudadNombre}, ${destino.paisNombre}` })));

  protected readonly opcionesFiltro = computed(() =>
    this.opcionesDestinos().map(opcion => ({ ...opcion, id: opcion.id.toString() })));

  protected readonly origenes = [
    { id: 1, descripcion: 'Catálogo' },
    { id: 2, descripcion: 'Generadas por IA' }
  ];

  constructor() {
    this.destinosServicio.obtenerTodos().subscribe(destinos => this.destinos.set(destinos));
  }

  protected filtrar(filtros: FiltrosGrilla): void {
    const origenes = filtros['estadoIds'] as number[];

    this.grilla.filtrar({
      texto: filtros['texto'],
      destinoId: filtros['clienteId'],
      generadoPorIA: origenes.length === 1 ? origenes[0] === 2 : ''
    });
  }

  protected acciones(actividad: ActividadDisponible): AccionMenu[] {
    return [
      { texto: 'Editar', ejecutar: () => this.abrir(actividad) },
      { texto: 'Eliminar', peligro: true, ejecutar: () => this.eliminar(actividad) }
    ];
  }

  protected abrir(actividad?: ActividadDisponible): void {
    this.editando.set(actividad ?? null);
    this.formulario.reset({ 
      nombre: actividad?.nombre ?? '',
      descripcion: actividad?.descripcion ?? '',
      costoBase: actividad?.costoBase ?? 0,
      duracionEstimada: actividad?.duracionEstimada ?? 1,
      destinoId: actividad?.destinoId ?? 0
    });
    this.modal.set(true);
  }

  protected cerrar(): void {
    if (!this.guardando()) {
      this.modal.set(false);
    }
  }

  protected guardar(): void {
    if (this.formulario.invalid || !this.formulario.controls.nombre.value.trim()) {
      this.formulario.markAllAsTouched();
      return;
    }

    this.guardando.set(true);
    const actividad = this.editando();
    const datos = this.formulario.getRawValue();
    const solicitud = actividad
      ? this.servicio.actualizar(actividad.id, datos)
      : this.servicio.crear(datos);

    solicitud.subscribe({
      next: () => {
        this.guardando.set(false);
        this.modal.set(false);
        this.grilla.cargar();
        this.toastr.exito(actividad ? 'Actividad actualizada.' : 'Actividad creada.');
      },
      error: () => this.guardando.set(false)
    });
  }

  private async eliminar(actividad: ActividadDisponible): Promise<void> {
    const confirmado = await this.confirmacion.confirmar({
      titulo: `¿Eliminar ${actividad.nombre}?`,
      mensaje: 'Las propuestas que ya la usan no cambian.',
      accion: 'Eliminar',
      peligro: true
    });

    if (!confirmado) {
      return;
    }

    this.servicio.eliminar(actividad.id).subscribe(() => {
      this.grilla.cargar();
      this.toastr.exito('Actividad eliminada.');
    });
  }
}
