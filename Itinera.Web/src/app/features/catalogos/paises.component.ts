import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { Pais, PaisesService } from '../../core/services/paises.service';
import { ConfirmacionService } from '../../core/services/confirmacion.service';
import { ToastrService } from '../../core/services/toastr.service';
import { GrillaPaginada } from '../../core/utils/grilla-paginada';
import { ColumnaOrdenableComponent } from '../../shared/components/columna-ordenable/columna-ordenable.component';
import { EncabezadoComponent } from '../../shared/components/encabezado/encabezado.component';
import { FiltrosGrillaComponent } from '../../shared/components/filtros-grilla/filtros-grilla.component';
import { AccionMenu, MenuAccionesComponent } from '../../shared/components/menu-acciones/menu-acciones.component';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';
import { ModalComponent } from '../../shared/components/modal/modal.component';
import { TablaComponent } from '../../shared/components/tabla/tabla.component';

@Component({
  selector: 'app-paises',
  imports: [
    ReactiveFormsModule,
    ColumnaOrdenableComponent,
    EncabezadoComponent,
    FiltrosGrillaComponent,
    MenuAccionesComponent,
    MenuPrincipalComponent,
    ModalComponent,
    TablaComponent
  ],
  templateUrl: './paises.component.html'
})
export class PaisesComponent {
  private readonly servicio = inject(PaisesService);
  private readonly confirmacion = inject(ConfirmacionService);
  private readonly toastr = inject(ToastrService);
  private readonly constructorFormulario = inject(FormBuilder);

  protected readonly grilla = new GrillaPaginada<Pais>(
    consulta => this.servicio.obtenerPagina(consulta),
    'No se pudieron cargar los países.');
  protected readonly editando = signal<Pais | null>(null);
  protected readonly modal = signal(false);
  protected readonly guardando = signal(false);

  protected readonly formulario = this.constructorFormulario.nonNullable.group({
    nombre: ['', Validators.required]
  });

  protected acciones(pais: Pais): AccionMenu[] {
    return [
      { texto: 'Editar', ejecutar: () => this.abrir(pais) },
      { texto: 'Eliminar', peligro: true, ejecutar: () => this.eliminar(pais) }
    ];
  }

  protected abrir(pais?: Pais): void {
    this.editando.set(pais ?? null);
    this.formulario.reset({ nombre: pais?.nombre ?? '' });
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
    const pais = this.editando();
    const nombre = this.formulario.controls.nombre.value.trim();
    const solicitud = pais
      ? this.servicio.actualizar(pais.id, nombre)
      : this.servicio.crear(nombre);

    solicitud.subscribe({
      next: () => {
        this.guardando.set(false);
        this.modal.set(false);
        this.grilla.cargar();
        this.toastr.exito(pais ? 'País actualizado.' : 'País creado.');
      },
      error: () => this.guardando.set(false)
    });
  }

  private async eliminar(pais: Pais): Promise<void> {
    const confirmado = await this.confirmacion.confirmar({
      titulo: `¿Eliminar ${pais.nombre}?`,
      mensaje: 'Las propuestas que ya lo usan no cambian.',
      accion: 'Eliminar',
      peligro: true
    });

    if (!confirmado) {
      return;
    }

    this.servicio.eliminar(pais.id).subscribe(() => {
      this.grilla.cargar();
      this.toastr.exito('País eliminado.');
    });
  }
}
