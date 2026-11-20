import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { Cargo, CargosService } from '../../core/services/cargos.service';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';

@Component({
  selector: 'app-cargos',
  imports: [ReactiveFormsModule, MenuPrincipalComponent],
  templateUrl: './cargos.component.html'
})
export class CargosComponent {
  private readonly cargosService = inject(CargosService);
  private readonly constructorFormulario = inject(FormBuilder);

  protected readonly cargos = signal<Cargo[]>([]);
  protected readonly cargando = signal(true);
  protected readonly modalAbierto = signal(false);
  protected readonly cargoEnEdicion = signal<Cargo | null>(null);
  protected readonly guardando = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly aviso = signal('');

  protected readonly formulario = this.constructorFormulario.nonNullable.group({
    descripcion: ['', Validators.required]
  });

  constructor() {
    this.cargarCargos();
  }

  protected abrirModal(cargo?: Cargo): void {
    this.cargoEnEdicion.set(cargo ?? null);
    this.formulario.reset({ descripcion: cargo?.descripcion ?? '' });
    this.error.set(null);
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
    this.error.set(null);
    const cargo = this.cargoEnEdicion();
    const solicitud = cargo
      ? this.cargosService.actualizar(cargo.id, descripcion)
      : this.cargosService.crear(descripcion);

    solicitud.subscribe({
      next: () => {
        this.guardando.set(false);
        this.modalAbierto.set(false);
        this.cargarCargos();
        this.mostrarAviso(cargo ? 'Cargo actualizado correctamente' : 'Cargo creado correctamente');
      },
      error: respuesta => {
        this.guardando.set(false);
        this.error.set(respuesta.error?.detail ?? 'No se pudo guardar el cargo.');
      }
    });
  }

  protected eliminar(cargo: Cargo): void {
    if (!window.confirm(`¿Querés eliminar el cargo "${cargo.descripcion}"?`)) {
      return;
    }

    this.cargosService.eliminar(cargo.id).subscribe({
      next: () => {
        this.cargarCargos();
        this.mostrarAviso('Cargo eliminado correctamente');
      },
      error: respuesta => this.error.set(
        respuesta.error?.detail ?? 'No se pudo eliminar el cargo.'
      )
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

  private mostrarAviso(mensaje: string): void {
    this.aviso.set(mensaje);
    setTimeout(() => this.aviso.set(''), 2600);
  }
}
