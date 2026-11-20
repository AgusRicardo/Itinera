import { Component, inject, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActividadDisponible, ActividadesService } from '../../core/services/actividades.service';
import { DestinoApi, DestinosService } from '../../core/services/destinos.service';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';

@Component({ selector: 'app-actividades', imports: [ReactiveFormsModule, DecimalPipe, MenuPrincipalComponent], templateUrl: './actividades.component.html' })
export class ActividadesComponent {
  private readonly servicio = inject(ActividadesService);
  private readonly destinosServicio = inject(DestinosService);
  private readonly fb = inject(FormBuilder);
  protected readonly actividades = signal<ActividadDisponible[]>([]);
  protected readonly destinos = signal<DestinoApi[]>([]);
  protected readonly editando = signal<ActividadDisponible | null>(null);
  protected readonly modal = signal(false);
  protected readonly cargando = signal(true);
  protected readonly guardando = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly formulario = this.fb.nonNullable.group({
    nombre: ['', Validators.required], descripcion: [''], costoBase: [0, [Validators.required, Validators.min(0)]],
    duracionEstimada: [1, [Validators.required, Validators.min(1)]], destinoId: [0, [Validators.required, Validators.min(1)]]
  });

  constructor() { this.cargar(); this.destinosServicio.obtenerTodos().subscribe(destinos => this.destinos.set(destinos)); }
  protected abrir(actividad?: ActividadDisponible): void { this.editando.set(actividad ?? null); this.formulario.reset({ nombre: actividad?.nombre ?? '', descripcion: actividad?.descripcion ?? '', costoBase: actividad?.costoBase ?? 0, duracionEstimada: actividad?.duracionEstimada ?? 1, destinoId: actividad?.destinoId ?? 0 }); this.error.set(null); this.modal.set(true); }
  protected cerrar(): void { if (!this.guardando()) this.modal.set(false); }
  protected guardar(): void {
    if (this.formulario.invalid) { this.formulario.markAllAsTouched(); return; }
    this.guardando.set(true); const actividad = this.editando(); const datos = this.formulario.getRawValue();
    const solicitud = actividad ? this.servicio.actualizar(actividad.id, datos) : this.servicio.crear(datos);
    solicitud.subscribe({ next: () => { this.guardando.set(false); this.modal.set(false); this.cargar(); }, error: respuesta => { this.guardando.set(false); this.error.set(respuesta.error?.detail ?? 'No se pudo guardar la actividad.'); } });
  }
  protected eliminar(actividad: ActividadDisponible): void { if (!window.confirm(`¿Querés desactivar ${actividad.nombre}?`)) return; this.servicio.eliminar(actividad.id).subscribe({ next: () => this.cargar(), error: respuesta => this.error.set(respuesta.error?.detail ?? 'No se pudo desactivar la actividad.') }); }
  private cargar(): void { this.servicio.obtenerTodas().subscribe({ next: actividades => { this.actividades.set(actividades); this.cargando.set(false); }, error: () => { this.error.set('No se pudieron cargar las actividades.'); this.cargando.set(false); } }); }
}
