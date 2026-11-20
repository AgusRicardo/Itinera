import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DestinoApi, DestinosService } from '../../core/services/destinos.service';
import { Ciudad, CiudadesService } from '../../core/services/ciudades.service';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';

@Component({ selector: 'app-destinos', imports: [ReactiveFormsModule, MenuPrincipalComponent], templateUrl: './destinos.component.html' })
export class DestinosComponent {
  private readonly servicio = inject(DestinosService);
  private readonly ciudadesServicio = inject(CiudadesService);
  private readonly fb = inject(FormBuilder);
  protected readonly destinos = signal<DestinoApi[]>([]);
  protected readonly ciudades = signal<Ciudad[]>([]);
  protected readonly editando = signal<DestinoApi | null>(null);
  protected readonly modal = signal(false);
  protected readonly cargando = signal(true);
  protected readonly guardando = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly formulario = this.fb.nonNullable.group({ ciudadId: [0, [Validators.required, Validators.min(1)]] });

  constructor() { this.cargar(); this.ciudadesServicio.obtenerTodos().subscribe(ciudades => this.ciudades.set(ciudades)); }
  protected abrir(destino?: DestinoApi): void { this.editando.set(destino ?? null); this.formulario.reset({ ciudadId: destino?.ciudadId ?? 0 }); this.error.set(null); this.modal.set(true); }
  protected cerrar(): void { if (!this.guardando()) this.modal.set(false); }
  protected guardar(): void {
    if (this.formulario.invalid) { this.formulario.markAllAsTouched(); return; }
    this.guardando.set(true); const destino = this.editando(); const ciudadId = this.formulario.controls.ciudadId.value;
    const solicitud = destino ? this.servicio.actualizar(destino.id, ciudadId) : this.servicio.crear(ciudadId);
    solicitud.subscribe({ next: () => { this.guardando.set(false); this.modal.set(false); this.cargar(); }, error: respuesta => { this.guardando.set(false); this.error.set(respuesta.error?.detail ?? 'No se pudo guardar el destino.'); } });
  }
  protected eliminar(destino: DestinoApi): void { if (!window.confirm(`¿Querés desactivar el destino de ${destino.ciudadNombre}?`)) return; this.servicio.eliminar(destino.id).subscribe({ next: () => this.cargar(), error: respuesta => this.error.set(respuesta.error?.detail ?? 'No se pudo desactivar el destino.') }); }
  private cargar(): void { this.servicio.obtenerTodos().subscribe({ next: destinos => { this.destinos.set(destinos); this.cargando.set(false); }, error: () => { this.error.set('No se pudieron cargar los destinos.'); this.cargando.set(false); } }); }
}
