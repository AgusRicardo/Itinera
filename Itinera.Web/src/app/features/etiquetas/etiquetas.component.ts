import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { Etiqueta } from '../../core/models/etiqueta';
import { EtiquetasService } from '../../core/services/etiquetas.service';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';

@Component({
  selector: 'app-etiquetas',
  imports: [ReactiveFormsModule, MenuPrincipalComponent],
  templateUrl: './etiquetas.component.html'
})
export class EtiquetasComponent {
  private readonly etiquetasService = inject(EtiquetasService);
  private readonly formularioConstructor = inject(FormBuilder);

  protected readonly etiquetas = signal<Etiqueta[]>([]);
  protected readonly etiquetaEditando = signal<Etiqueta | null>(null);
  protected readonly modalAbierto = signal(false);
  protected readonly cargando = signal(true);
  protected readonly guardando = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly aviso = signal('');

  protected readonly formulario = this.formularioConstructor.nonNullable.group({
    nombre: ['', Validators.required]
  });

  constructor() {
    this.cargarEtiquetas();
  }

  protected abrirModal(etiqueta?: Etiqueta): void {
    this.etiquetaEditando.set(etiqueta ?? null);
    this.formulario.reset({ nombre: etiqueta?.nombre ?? '' });
    this.error.set(null);
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
    this.error.set(null);
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
        this.mostrarAviso(etiqueta ? 'Etiqueta actualizada correctamente' : 'Etiqueta creada correctamente');
      },
      error: respuesta => {
        this.guardando.set(false);
        this.error.set(respuesta.error?.detail ?? 'No se pudo guardar la etiqueta.');
      }
    });
  }

  protected desactivar(etiqueta: Etiqueta): void {
    if (!window.confirm(`¿Querés desactivar la etiqueta "${etiqueta.nombre}"?`)) {
      return;
    }

    this.etiquetasService.desactivar(etiqueta.id).subscribe({
      next: () => {
        this.cargarEtiquetas();
        this.mostrarAviso('Etiqueta desactivada');
      },
      error: respuesta => this.error.set(
        respuesta.error?.detail ?? 'No se pudo desactivar la etiqueta.'
      )
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

  private mostrarAviso(mensaje: string): void {
    this.aviso.set(mensaje);
    setTimeout(() => this.aviso.set(''), 2600);
  }
}
