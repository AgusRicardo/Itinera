import { Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { Propuesta } from '../../core/models/propuesta';
import {
  PropuestasService,
  RespuestaCliente
} from '../../core/services/propuestas.service';
import { MonedaPipe } from '../../shared/pipes/moneda.pipe';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';
import { ToastrService } from '../../core/services/toastr.service';
import { AutenticacionService } from '../../core/services/autenticacion.service';

@Component({
  selector: 'app-detalle-propuesta',
  imports: [RouterLink, MonedaPipe, DatePipe, MenuPrincipalComponent],
  templateUrl: './detalle-propuesta.component.html'
})
export class DetallePropuestaComponent {
  private readonly ruta = inject(ActivatedRoute);
  private readonly propuestasService = inject(PropuestasService);
  private readonly autenticacion = inject(AutenticacionService);
  private readonly toastr = inject(ToastrService);

  protected readonly propuesta = signal<Propuesta | null>(null);
  protected readonly cargando = signal(true);
  protected readonly error = signal<string | null>(null);
  protected readonly respuesta = signal<RespuestaCliente>('Aceptada');
  protected readonly procesando = signal(false);

  protected puedeCrearFactura(): boolean {
    return this.autenticacion.tienePermiso('facturacion.crear');
  }

  protected puedeRegistrarRespuesta(): boolean {
    return this.autenticacion.tienePermiso('propuestas.registrar-respuesta');
  }

  protected puedeEditarItinerario(): boolean {
    return this.autenticacion.tienePermiso('itinerarios.editar');
  }

  protected puedeGenerarConIa(): boolean {
    return this.autenticacion.tienePermiso('itinerarios.generar-ia');
  }

  protected editable(): boolean {
    const estado = this.propuesta()?.estado;

    return estado !== 'Aceptada' && estado !== 'Eliminada';
  }

  protected puedeEditarPropuesta(): boolean {
    return this.editable() && this.autenticacion.tienePermiso('propuestas.editar');
  }

  protected nombresDestinos(): string {
    return (this.propuesta()?.destinos ?? [])
      .map(destino => destino.descripcion.split(',')[0])
      .join(' · ');
  }

  protected cantidadListos(): number {
    return this.propuesta()?.destinos.filter(destino => !destino.faltantes.length).length ?? 0;
  }

  protected destinosPendientes(): string {
    return (this.propuesta()?.destinos ?? [])
      .filter(destino => destino.faltantes.length)
      .map(destino => destino.descripcion)
      .join(' · ');
  }

  protected porcentajePresupuesto(): number {
    const propuesta = this.propuesta();

    return propuesta ? Math.min(100, propuesta.presupuestoUsado / propuesta.presupuesto * 100) : 0;
  }

  protected puedePresentar(): boolean {
    return this.autenticacion.tienePermiso('propuestas.presentar');
  }

  constructor() {
    const id = Number(this.ruta.snapshot.paramMap.get('id'));

    this.propuestasService.obtenerPorId(id).subscribe({
      next: propuesta => {
        this.propuesta.set(propuesta);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set('No se pudo cargar la propuesta.');
        this.cargando.set(false);
      }
    });
  }

  protected presentar(): void {
    const actual = this.propuesta();
    if (!actual) {
      return;
    }

    this.procesando.set(true);
    this.error.set(null);

    this.propuestasService.presentar(actual.id).subscribe({
      next: propuesta => {
        this.propuesta.set(propuesta);
        this.procesando.set(false);
      },
      error: () => {
        this.toastr.error('No se pudo presentar la propuesta.');
        this.procesando.set(false);
      }
    });
  }

  protected registrarRespuesta(): void {
    const actual = this.propuesta();
    if (!actual) {
      return;
    }

    this.procesando.set(true);
    this.error.set(null);

    this.propuestasService.registrarRespuesta(actual.id, this.respuesta()).subscribe({
      next: propuesta => {
        this.propuesta.set(propuesta);
        this.procesando.set(false);
      },
      error: () => {
        this.toastr.error('No se pudo registrar la respuesta.');
        this.procesando.set(false);
      }
    });
  }
}
