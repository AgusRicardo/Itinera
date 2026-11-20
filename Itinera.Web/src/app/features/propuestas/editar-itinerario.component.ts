import { Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { Itinerario } from '../../core/models/itinerario';
import { ActividadDisponible, ActividadesService } from '../../core/services/actividades.service';
import {
  ModificarItinerarioRequest,
  PropuestasService
} from '../../core/services/propuestas.service';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';

@Component({
  selector: 'app-editar-itinerario',
  imports: [ReactiveFormsModule, RouterLink, DatePipe, MenuPrincipalComponent],
  templateUrl: './editar-itinerario.component.html'
})
export class EditarItinerarioComponent {
  private readonly ruta = inject(ActivatedRoute);
  private readonly propuestasService = inject(PropuestasService);
  private readonly constructorFormulario = inject(FormBuilder);
  private readonly actividadesService = inject(ActividadesService);

  protected readonly propuestaId = Number(this.ruta.snapshot.paramMap.get('id'));
  protected readonly itinerario = signal<Itinerario | null>(null);
  protected readonly cargando = signal(true);
  protected readonly guardando = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly guardado = signal(false);
  protected readonly actividadesDisponibles = signal<ActividadDisponible[]>([]);
  protected readonly destinoSeleccionado = signal(0);

  protected readonly formulario = this.constructorFormulario.nonNullable.group({
    fechaInicio: ['', Validators.required],
    fechaFin: ['', Validators.required]
  });

  constructor() {
    this.cargarItinerario();
    this.cargarActividades();
  }

  protected seleccionarDestino(indice: number): void {
    this.destinoSeleccionado.set(indice);
  }

  protected agregarActividad(actividad: ActividadDisponible): void {
    const actual = this.itinerario();
    if (!actual) {
      return;
    }

    const indice = this.destinoSeleccionado();
    const destinos = actual.destinos.map((destino, destinoIndice) => {
      if (destinoIndice !== indice) {
        return destino;
      }

      return {
        ...destino,
        actividades: [
          ...(destino.actividades ?? []),
          {
            actividadId: actividad.id,
            actividadNombre: actividad.nombre,
            costoFinal: actividad.costoBase,
            observaciones: '',
            orden: (destino.actividades?.length ?? 0) + 1
          }
        ]
      };
    });

    this.itinerario.set({ ...actual, destinos });
  }

  protected eliminarActividad(indiceActividad: number): void {
    const actual = this.itinerario();
    if (!actual) {
      return;
    }

    const indiceDestino = this.destinoSeleccionado();
    const destinos = actual.destinos.map((destino, destinoIndice) =>
      destinoIndice === indiceDestino
        ? { ...destino, actividades: (destino.actividades ?? []).filter((_, indice) => indice !== indiceActividad) }
        : destino
    );

    this.itinerario.set({ ...actual, destinos });
  }

  protected guardar(): void {
    const actual = this.itinerario();
    if (!actual || this.formulario.invalid) {
      this.formulario.markAllAsTouched();
      return;
    }

    this.guardando.set(true);
    this.error.set(null);

    const valores = this.formulario.getRawValue();
    const request: ModificarItinerarioRequest = {
      fechaInicio: valores.fechaInicio,
      fechaFin: valores.fechaFin,
      destinos: actual.destinos.map((destino, indice) => ({
        destinoId: destino.destinoId ?? indice + 1,
        orden: destino.orden ?? indice + 1,
        fechaLlegada: destino.fechaLlegada ?? valores.fechaInicio,
        fechaPartida: destino.fechaPartida ?? valores.fechaFin,
        actividades: (destino.actividades ?? []).map((actividad, actividadIndice) => ({
          actividadId: actividad.actividadId,
          fechaHoraInicio: actividad.fechaHoraInicio ?? `${valores.fechaInicio}T09:00:00Z`,
          costoFinal: actividad.costoFinal ?? 0,
          observaciones: actividad.observaciones ?? '',
          orden: actividad.orden ?? actividadIndice + 1
        }))
      }))
    };

    this.propuestasService.modificarItinerario(this.propuestaId, request).subscribe({
      next: respuesta => {
        this.itinerario.set(respuesta.itinerario ?? actual);
        this.guardando.set(false);
        this.guardado.set(true);
        setTimeout(() => this.guardado.set(false), 2000);
      },
      error: () => {
        this.guardando.set(false);
        this.error.set('No se pudo guardar el itinerario.');
      }
    });
  }

  private cargarItinerario(): void {
    this.propuestasService.obtenerPorId(this.propuestaId).subscribe({
      next: propuesta => {
        const itinerario = propuesta.itinerario ?? null;
        this.itinerario.set(itinerario);

        if (itinerario) {
          this.formulario.patchValue({
            fechaInicio: itinerario.fechaInicio,
            fechaFin: itinerario.fechaFin
          });
        }

        this.cargando.set(false);
      },
      error: () => {
        this.error.set('No se pudo cargar el itinerario.');
        this.cargando.set(false);
      }
    });
  }

  private cargarActividades(): void {
    this.actividadesService.obtenerTodas().subscribe({
      next: actividades => this.actividadesDisponibles.set(actividades),
      error: () => this.error.set('No se pudieron cargar las actividades.')
    });
  }
}
