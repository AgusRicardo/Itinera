import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';

import { Cargo, CargosService } from '../../core/services/cargos.service';
import { AccionMenu, MenuAccionesComponent } from '../../shared/components/menu-acciones/menu-acciones.component';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';
import { ConfirmacionService } from '../../core/services/confirmacion.service';
import { ToastrService } from '../../core/services/toastr.service';
import { textoRequerido } from '../../core/utils/validadores';
import { EncabezadoComponent } from '../../shared/components/encabezado/encabezado.component';

@Component({
  selector: 'app-cargos',
  imports: [
    ReactiveFormsModule,
    MenuAccionesComponent,
    MenuPrincipalComponent,
    EncabezadoComponent
  ],
  templateUrl: './cargos.component.html'
})
export class CargosComponent {
  private readonly confirmacion = inject(ConfirmacionService);
  private readonly toastr = inject(ToastrService);
  private readonly cargosService = inject(CargosService);
  private readonly constructorFormulario = inject(FormBuilder);

  protected readonly cargos = signal<Cargo[]>([]);
  protected readonly cargando = signal(true);
  protected readonly modalAbierto = signal(false);
  protected readonly cargoEnEdicion = signal<Cargo | null>(null);
  protected readonly guardando = signal(false);
  protected readonly error = signal<string | null>(null);

  protected readonly formulario = this.constructorFormulario.nonNullable.group({
    descripcion: ['', textoRequerido]
  });

  constructor() {
    this.cargarCargos();
  }

  protected acciones(cargo: Cargo): AccionMenu[] {
    return [
      { texto: 'Editar', ejecutar: () => this.abrirModal(cargo) },
      { texto: 'Eliminar', peligro: true, ejecutar: () => this.eliminar(cargo) }
    ];
  }

  protected abrirModal(cargo?: Cargo): void {
    this.cargoEnEdicion.set(cargo ?? null);
    this.formulario.reset({ descripcion: cargo?.descripcion ?? '' });
    this.modalAbierto.set(true);
  }

  protected cerrarModal(): void {
    if (!this.guardando()) {
      this.modalAbierto.set(false);
    }
  }

  protected guardar(): void {
    if (this.formulario.invalid) {
      this.formulario.markAllAsTouched();
      return;
    }

    const descripcion = this.formulario.controls.descripcion.value.trim();
    if (!descripcion) {
      this.formulario.controls.descripcion.setValue('');
      this.formulario.controls.descripcion.markAsTouched();
      return;
    }

    this.guardando.set(true);
    const cargo = this.cargoEnEdicion();
    const solicitud = cargo
      ? this.cargosService.actualizar(cargo.id, descripcion)
      : this.cargosService.crear(descripcion);

    solicitud.subscribe({
      next: () => {
        this.guardando.set(false);
        this.modalAbierto.set(false);
        this.cargarCargos();
        this.toastr.exito(cargo ? 'Cargo actualizado.' : 'Cargo creado.');
      },
      error: () => this.guardando.set(false)
    });
  }

  private async eliminar(cargo: Cargo): Promise<void> {
    const confirmado = await this.confirmacion.confirmar({
      titulo: `¿Eliminar el cargo ${cargo.descripcion}?`,
      mensaje: 'Quienes ya lo tienen lo conservan.',
      accion: 'Eliminar',
      peligro: true
    });

    if (!confirmado) {
      return;
    }

    this.cargosService.eliminar(cargo.id).subscribe({
      next: () => {
        this.cargarCargos();
        this.toastr.exito('Cargo eliminado.');
      }
    });
  }

  private cargarCargos(): void {
    this.cargosService.obtenerTodos().subscribe({
      next: cargos => {
        this.cargos.set(cargos);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set('No se pudieron cargar los cargos.');
        this.cargando.set(false);
      }
    });
  }
}
