import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Pais, PaisesService } from '../../core/services/paises.service';
import { GrillaPaginada } from '../../core/utils/grilla-paginada';
import { PaginadorComponent } from '../../shared/components/paginador/paginador.component';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';

@Component({
  selector: 'app-paises',
  imports: [ReactiveFormsModule, MenuPrincipalComponent, PaginadorComponent],
  templateUrl: './paises.component.html'
})
export class PaisesComponent {
  private readonly servicio = inject(PaisesService);
  private readonly constructorFormulario = inject(FormBuilder);
  protected readonly grilla = new GrillaPaginada<Pais>(
    consulta => this.servicio.obtenerPagina(consulta), 'No se pudieron cargar los países.');
  protected readonly editando = signal<Pais | null>(null);
  protected readonly modal = signal(false);
  protected readonly guardando = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly formulario = this.constructorFormulario.nonNullable.group({ nombre: ['', Validators.required] });

  protected abrir(pais?: Pais): void {
    this.editando.set(pais ?? null);
    this.formulario.reset({ nombre: pais?.nombre ?? '' });
    this.error.set(null);
    this.modal.set(true);
  }

  protected cerrar(): void { if (!this.guardando()) this.modal.set(false); }

  protected guardar(): void {
    if (this.formulario.invalid || !this.formulario.controls.nombre.value.trim()) {
      this.formulario.markAllAsTouched();
      return;
    }
    this.guardando.set(true);
    const pais = this.editando();
    const nombre = this.formulario.controls.nombre.value.trim();
    const solicitud = pais ? this.servicio.actualizar(pais.id, nombre) : this.servicio.crear(nombre);
    solicitud.subscribe({
      next: () => { this.guardando.set(false); this.modal.set(false); this.grilla.cargar(); },
      error: respuesta => { this.guardando.set(false); this.error.set(respuesta.error?.detail ?? 'No se pudo guardar el país.'); }
    });
  }

  protected eliminar(pais: Pais): void {
    if (!window.confirm(`¿Querés desactivar ${pais.nombre}?`)) return;
    this.servicio.eliminar(pais.id).subscribe({ next: () => this.grilla.cargar(), error: respuesta => this.error.set(respuesta.error?.detail ?? 'No se pudo desactivar el país.') });
  }
}
