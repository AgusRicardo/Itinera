import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { Ciudad, CiudadesService } from '../../core/services/ciudades.service';
import { Pais, PaisesService } from '../../core/services/paises.service';
import { ConfirmacionService } from '../../core/services/confirmacion.service';
import { ToastrService } from '../../core/services/toastr.service';
import { GrillaPaginada } from '../../core/utils/grilla-paginada';
import { textoRequerido } from '../../core/utils/validadores';
import { ColumnaOrdenableComponent } from '../../shared/components/columna-ordenable/columna-ordenable.component';
import { EncabezadoComponent } from '../../shared/components/encabezado/encabezado.component';
import { FiltrosGrillaComponent } from '../../shared/components/filtros-grilla/filtros-grilla.component';
import { AccionMenu, MenuAccionesComponent } from '../../shared/components/menu-acciones/menu-acciones.component';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';
import { ModalComponent } from '../../shared/components/modal/modal.component';
import { OpcionSelector, SelectorComponent } from '../../shared/components/selector/selector.component';
import { TablaComponent } from '../../shared/components/tabla/tabla.component';

@Component({
  selector: 'app-ciudades',
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
  templateUrl: './ciudades.component.html'
})
export class CiudadesComponent {
  private readonly servicio = inject(CiudadesService);
  private readonly paisesServicio = inject(PaisesService);
  private readonly confirmacion = inject(ConfirmacionService);
  private readonly toastr = inject(ToastrService);
  private readonly constructorFormulario = inject(FormBuilder);

  protected readonly grilla = new GrillaPaginada<Ciudad>(
    consulta => this.servicio.obtenerPagina(consulta),
    'No se pudieron cargar las ciudades.');
  private readonly paises = signal<Pais[]>([]);
  protected readonly editando = signal<Ciudad | null>(null);
  protected readonly modal = signal(false);
  protected readonly guardando = signal(false);

  protected readonly formulario = this.constructorFormulario.nonNullable.group({
    nombre: ['', textoRequerido],
    paisId: [0, [Validators.required, Validators.min(1)]]
  });

  protected readonly opcionesPaises = computed<OpcionSelector[]>(() =>
    this.paises().map(pais => ({ id: pais.id, nombre: pais.nombre })));

  constructor() {
    this.paisesServicio.obtenerTodos().subscribe(paises => this.paises.set(paises));
  }

  protected acciones(ciudad: Ciudad): AccionMenu[] {
    return [
      { texto: 'Editar', ejecutar: () => this.abrir(ciudad) },
      { texto: 'Eliminar', peligro: true, ejecutar: () => this.eliminar(ciudad) }
    ];
  }

  protected abrir(ciudad?: Ciudad): void {
    this.editando.set(ciudad ?? null);
    this.formulario.reset({ nombre: ciudad?.nombre ?? '', paisId: ciudad?.paisId ?? 0 });
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
    const ciudad = this.editando();
    const { nombre, paisId } = this.formulario.getRawValue();
    const solicitud = ciudad
      ? this.servicio.actualizar(ciudad.id, nombre.trim(), paisId)
      : this.servicio.crear(nombre.trim(), paisId);

    solicitud.subscribe({
      next: () => {
        this.guardando.set(false);
        this.modal.set(false);
        this.grilla.cargar();
        this.toastr.exito(ciudad ? 'Ciudad actualizada.' : 'Ciudad creada.');
      },
      error: () => this.guardando.set(false)
    });
  }

  private async eliminar(ciudad: Ciudad): Promise<void> {
    const confirmado = await this.confirmacion.confirmar({
      titulo: `¿Eliminar ${ciudad.nombre}?`,
      mensaje: 'También dejan de aparecer su destino y sus actividades. Las propuestas que ya la usan no cambian.',
      accion: 'Eliminar',
      peligro: true
    });

    if (!confirmado) {
      return;
    }

    this.servicio.eliminar(ciudad.id).subscribe(() => {
      this.grilla.cargar();
      this.toastr.exito('Ciudad eliminada.');
    });
  }
}
