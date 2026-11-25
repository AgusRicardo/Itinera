import { Component, computed, effect, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { Propuesta } from '../../core/models/propuesta';
import { DestinosService } from '../../core/services/destinos.service';
import { NavegacionService } from '../../core/services/navegacion.service';
import { AlternativaItinerario, PropuestasService } from '../../core/services/propuestas.service';
import { ToastrService } from '../../core/services/toastr.service';
import { describirDia, diasDeEstadia, fechaDe, horaDe } from '../../core/utils/fechas-itinerario';
import { MonedaPipe } from '../../shared/pipes/moneda.pipe';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';
import { ConfirmacionService } from '../../core/services/confirmacion.service';

@Component({
  selector: 'app-generar-itinerario',
  imports: [
    RouterLink,
    MonedaPipe,
    DatePipe,
    MenuPrincipalComponent
  ],
  templateUrl: './generar-itinerario.component.html'
})
export class GenerarItinerarioComponent {
  private readonly ruta = inject(ActivatedRoute);
  private readonly propuestasService = inject(PropuestasService);
  private readonly navegacion = inject(NavegacionService);
  private readonly destinosService = inject(DestinosService);
  private readonly confirmacion = inject(ConfirmacionService);
  private readonly toastr = inject(ToastrService);

  protected readonly propuestaId = Number(this.ruta.snapshot.paramMap.get('id'));
  protected readonly destinoId = Number(this.ruta.snapshot.paramMap.get('destinoId'));
  protected readonly nuevoDestinoId =
    Number(this.ruta.snapshot.queryParamMap.get('nuevoDestino')) || null;
  protected readonly propuesta = signal<Propuesta | null>(null);
  protected readonly nombreNuevoDestino = signal('');
  protected readonly preferencias = signal('');
  protected readonly alternativas = signal<AlternativaItinerario[]>([]);
  protected readonly elegida = signal(0);
  protected readonly abiertas = signal<number[]>([0]);
  protected readonly generando = signal(false);
  protected readonly guardando = signal(false);
  protected readonly sinResultados = signal(false);
  protected readonly errorIa = signal<string | null>(null);
  protected readonly error = signal<string | null>(null);
  protected readonly horaDe = horaDe;

  protected readonly destino = computed(() =>
    this.propuesta()?.destinos.find(destino => destino.id === this.destinoId) ?? null);

  protected readonly nombreDestino = computed(() =>
    this.nuevoDestinoId ? this.nombreNuevoDestino() : this.destino()?.descripcion ?? '');

  protected readonly pedidas = computed(() =>
    this.nuevoDestinoId
      ? []
      : (this.propuesta()?.actividades ?? [])
        .filter(actividad => actividad.destinoId === this.destinoId));

  protected readonly disponible = computed(() =>
    (this.propuesta()?.presupuestoDisponible ?? 0) + (this.destino()?.itinerario?.costo ?? 0));

  constructor() {
    effect(() => this.navegacion.nombreDestino.set(this.destino()?.descripcion ?? ''));

    this.propuestasService.obtenerPorId(this.propuestaId).subscribe({
      next: propuesta => {
        this.propuesta.set(propuesta);
        this.preferencias.set(propuesta.preferencias);

        if (!this.destino()) {
          this.error.set('El destino no pertenece a la propuesta.');
        }
      },
      error: () => this.error.set('No se pudo cargar la propuesta.')
    });

    if (this.nuevoDestinoId) {
      this.destinosService.obtenerTodos().subscribe(destinos => {
        const destino = destinos.find(item => item.id === this.nuevoDestinoId);
        this.nombreNuevoDestino.set(
          destino ? `${destino.ciudadNombre}, ${destino.paisNombre}` : 'el nuevo destino');
      });
    }
  }

  protected diasDe(alternativa: AlternativaItinerario) {
    const destino = this.destino();

    return destino
      ? diasDeEstadia(destino).map((fecha, indice) => ({
        fecha,
        numero: indice + 1,
        nombre: describirDia(fecha).split(' ')[0].replace(',', ''),
        actividades: alternativa.actividades
          .filter(actividad => fechaDe(actividad.fechaHoraInicio) === fecha)
      }))
      : [];
  }

  protected cantidadDiasLibres(alternativa: AlternativaItinerario): number {
    return this.diasDe(alternativa).filter(dia => !dia.actividades.length).length;
  }

  protected cantidadRecomendadas(alternativa: AlternativaItinerario): number {
    return alternativa.actividades.filter(actividad => !actividad.pedidaPorCliente).length;
  }

  protected alternar(indice: number): void {
    this.abiertas.update(abiertas => abiertas.includes(indice)
      ? abiertas.filter(abierta => abierta !== indice)
      : [...abiertas, indice]);
  }

  protected generar(): void {
    this.generando.set(true);
    this.alternativas.set([]);
    this.sinResultados.set(false);
    this.errorIa.set(null);

    const generacion$ = this.nuevoDestinoId
      ? this.propuestasService.generarCambioDestino(
        this.propuestaId, this.destinoId, this.nuevoDestinoId, this.preferencias())
      : this.propuestasService.generarAlternativas(
        this.propuestaId, this.destinoId, this.preferencias());

    generacion$.subscribe({
      next: alternativas => {
        this.generando.set(false);
        this.alternativas.set(alternativas);
        this.elegida.set(0);
        this.abiertas.set([0]);
        this.sinResultados.set(!alternativas.length);
      },
      error: respuesta => {
        this.generando.set(false);
        this.errorIa.set(respuesta.error?.detail ?? 'No se pudieron generar alternativas.');
      }
    });
  }

  protected async usar(): Promise<void> {
    const alternativa = this.alternativas()[this.elegida()];
    const destino = this.destino();
    if (!alternativa || !destino) {
      return;
    }

    const confirmado = !(this.nuevoDestinoId || destino.itinerario)
      || await this.confirmacion.confirmar(this.nuevoDestinoId
        ? {
          titulo: '¿Cambiar el destino?',
          mensaje: `Se reemplaza ${destino.descripcion} por ${this.nombreDestino()} en la propuesta.`,
          accion: 'Cambiar'
        }
        : {
          titulo: '¿Reemplazar el itinerario?',
          mensaje: `Se pierde el itinerario actual de ${destino.descripcion}.`,
          accion: 'Reemplazar'
        });

    if (!confirmado) {
      return;
    }

    this.guardando.set(true);
    const guardado$ = this.nuevoDestinoId
      ? this.propuestasService.cambiarDestino(
        this.propuestaId, this.destinoId, this.nuevoDestinoId, alternativa)
      : this.propuestasService.aplicarAlternativa(this.propuestaId, this.destinoId, alternativa);

    guardado$.subscribe({
      next: () => {
        this.toastr.exito('Itinerario guardado.');
        this.navegacion.terminarEn(
          `/propuestas/${this.propuestaId}/destinos/${this.nuevoDestinoId ?? this.destinoId}`);
      },
      error: () => this.guardando.set(false)
    });
  }
}
