import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Ciudad, CiudadesService } from '../../core/services/ciudades.service';
import { Pais, PaisesService } from '../../core/services/paises.service';
import { GrillaPaginada } from '../../core/utils/grilla-paginada';
import { PaginadorComponent } from '../../shared/components/paginador/paginador.component';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';

@Component({ selector: 'app-ciudades', imports: [ReactiveFormsModule, MenuPrincipalComponent, PaginadorComponent], templateUrl: './ciudades.component.html' })
export class CiudadesComponent {
  private readonly servicio = inject(CiudadesService);
  private readonly paisesServicio = inject(PaisesService);
  private readonly fb = inject(FormBuilder);
  protected readonly grilla = new GrillaPaginada<Ciudad>(
    consulta => this.servicio.obtenerPagina(consulta), 'No se pudieron cargar las ciudades.');
  protected readonly paises = signal<Pais[]>([]);
  protected readonly editando = signal<Ciudad | null>(null);
  protected readonly modal = signal(false);
  protected readonly guardando = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly formulario = this.fb.nonNullable.group({ nombre: ['', Validators.required], paisId: [0, [Validators.required, Validators.min(1)]] });

  constructor() { this.paisesServicio.obtenerTodos().subscribe(paises => this.paises.set(paises)); }
  protected abrir(ciudad?: Ciudad): void { this.editando.set(ciudad ?? null); this.formulario.reset({ nombre: ciudad?.nombre ?? '', paisId: ciudad?.paisId ?? 0 }); this.error.set(null); this.modal.set(true); }
  protected cerrar(): void { if (!this.guardando()) this.modal.set(false); }
  protected guardar(): void {
    if (this.formulario.invalid) { this.formulario.markAllAsTouched(); return; }
    this.guardando.set(true); const ciudad = this.editando(); const datos = this.formulario.getRawValue();
    const solicitud = ciudad ? this.servicio.actualizar(ciudad.id, datos.nombre.trim(), datos.paisId) : this.servicio.crear(datos.nombre.trim(), datos.paisId);
    solicitud.subscribe({ next: () => { this.guardando.set(false); this.modal.set(false); this.grilla.cargar(); }, error: respuesta => { this.guardando.set(false); this.error.set(respuesta.error?.detail ?? 'No se pudo guardar la ciudad.'); } });
  }
  protected eliminar(ciudad: Ciudad): void { if (!window.confirm(`¿Querés desactivar ${ciudad.nombre}?`)) return; this.servicio.eliminar(ciudad.id).subscribe({ next: () => this.grilla.cargar(), error: respuesta => this.error.set(respuesta.error?.detail ?? 'No se pudo desactivar la ciudad.') }); }
}
