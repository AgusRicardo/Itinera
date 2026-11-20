import { Component, inject, signal } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { Propuesta } from '../../core/models/propuesta';
import {
  PropuestasService,
  RespuestaCliente
} from '../../core/services/propuestas.service';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';
import { AutenticacionService } from '../../core/services/autenticacion.service';

@Component({
  selector: 'app-detalle-propuesta',
  imports: [RouterLink, CurrencyPipe, DatePipe, MenuPrincipalComponent],
  templateUrl: './detalle-propuesta.component.html'
})
export class DetallePropuestaComponent {
  private readonly ruta = inject(ActivatedRoute);
  private readonly propuestasService = inject(PropuestasService);
  private readonly autenticacion = inject(AutenticacionService);

  protected readonly propuesta = signal<Propuesta | null>(null);
  protected readonly cargando = signal(true);
  protected readonly error = signal<string | null>(null);
  protected readonly respuesta = signal<RespuestaCliente>('Aceptada');
  protected readonly procesando = signal(false);

  protected puedeCrearFactura(): boolean {
    return this.autenticacion.tienePermiso('facturacion.crear');
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
        this.error.set('No se pudo presentar la propuesta.');
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
        this.error.set('No se pudo registrar la respuesta.');
        this.procesando.set(false);
      }
    });
  }
}
