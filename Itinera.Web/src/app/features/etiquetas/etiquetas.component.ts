import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { Etiqueta } from '../../core/models/etiqueta';
import { EtiquetasService } from '../../core/services/etiquetas.service';
import { AccionMenu, MenuAccionesComponent } from '../../shared/components/menu-acciones/menu-acciones.component';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';
import { ConfirmacionService } from '../../core/services/confirmacion.service';
import { ToastrService } from '../../core/services/toastr.service';
import { EncabezadoComponent } from '../../shared/components/encabezado/encabezado.component';
import { AutenticacionService } from '../../core/services/autenticacion.service';

@Component({
  selector: 'app-etiquetas',
  imports: [
    ReactiveFormsModule,
    MenuAccionesComponent,
    MenuPrincipalComponent,
    EncabezadoComponent
  ],
  templateUrl: './etiquetas.component.html'
})
export class EtiquetasComponent {
  private readonly confirmacion = inject(ConfirmacionService);
  private readonly toastr = inject(ToastrService);
  private readonly etiquetasService = inject(EtiquetasService);
  private readonly formularioConstructor = inject(FormBuilder);

  protected readonly etiquetas = signal<Etiqueta[]>([]);
  protected readonly etiquetaEditando = signal<Etiqueta | null>(null);
  protected readonly modalAbierto = signal(false);
  protected readonly cargando = signal(true);
  protected readonly guardando = signal(false);
  protected readonly error = signal<string | null>(null);

  protected readonly puedeGestionar = inject(AutenticacionService).tienePermiso('etiquetas.gestionar');

  protected readonly formulario = this.formularioConstructor.nonNullable.group({
    nombre: ['', [Validators.required, Validators.maxLength(80)]]
  });

  constructor() {
    this.cargarEtiquetas();
  }

  protected acciones(etiqueta: Etiqueta): AccionMenu[] {
    return [
      { texto: 'Editar', ejecutar: () => this.abrirModal(etiqueta) },
      { texto: 'Eliminar', peligro: true, ejecutar: () => this.eliminar(etiqueta) }
    ];
  }

  protected abrirModal(etiqueta?: Etiqueta): void {
    this.etiquetaEditando.set(etiqueta ?? null);
    this.formulario.reset({ nombre: etiqueta?.nombre ?? '' });
    this.modalAbierto.set(true);
  }

  protected cerrarModal(): void {
    if (!this.guardando()) {
      this.modalAbierto.set(false);
    }
  }

  protected guardar(): void {
    if (this.formulario.invalid || !this.formulario.controls.nombre.value.trim()) {
      this.formulario.markAllAsTouched();
      return;
    }

    this.guardando.set(true);
    const etiqueta = this.etiquetaEditando();
    const nombre = this.formulario.controls.nombre.value.trim();
    const solicitud = etiqueta
      ? this.etiquetasService.actualizar(etiqueta.id, nombre)
      : this.etiquetasService.crear(nombre);

    solicitud.subscribe({
      next: () => {
        this.guardando.set(false);
        this.modalAbierto.set(false);
        this.cargarEtiquetas();
        this.toastr.exito(etiqueta ? 'Etiqueta actualizada.' : 'Etiqueta creada.');
      },
      error: () => this.guardando.set(false)
    });
  }

  private async eliminar(etiqueta: Etiqueta): Promise<void> {
    const confirmado = await this.confirmacion.confirmar({
      titulo: `¿Eliminar la etiqueta ${etiqueta.nombre}?`,
      mensaje: 'Las propuestas que ya la usan no cambian.',
      accion: 'Eliminar',
      peligro: true
    });

    if (!confirmado) {
      return;
    }

    this.etiquetasService.eliminar(etiqueta.id).subscribe({
      next: () => {
        this.cargarEtiquetas();
        this.toastr.exito('Etiqueta eliminada.');
      }
    });
  }

  private cargarEtiquetas(): void {
    this.etiquetasService.obtenerTodas().subscribe({
      next: etiquetas => {
        this.etiquetas.set(etiquetas);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set('No se pudieron cargar las etiquetas.');
        this.cargando.set(false);
      }
    });
  }
}
