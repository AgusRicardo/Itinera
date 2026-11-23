import { Component, computed, effect, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { forkJoin } from 'rxjs';

import { Propuesta } from '../../core/models/propuesta';
import { ActividadDisponible, ActividadesService } from '../../core/services/actividades.service';
import { NavegacionService } from '../../core/services/navegacion.service';
import { PropuestasService } from '../../core/services/propuestas.service';
import { ToastrService } from '../../core/services/toastr.service';
import {
  describirDia,
  diasDeEstadia,
  fechaDe,
  horaDe,
  minutosOcupados
} from '../../core/utils/fechas-itinerario';
import { MonedaPipe } from '../../shared/pipes/moneda.pipe';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';

interface ActividadArmada {
  actividadId: number;
  nombre: string;
  duracion: number;
  costo: number;
  fecha: string;
  hora: string;
  observaciones: string;
}

@Component({
  selector: 'app-armar-itinerario',
  imports: [MonedaPipe, DatePipe, MenuPrincipalComponent],
  templateUrl: './armar-itinerario.component.html'
})
export class ArmarItinerarioComponent {
  private readonly ruta = inject(ActivatedRoute);
  private readonly propuestasService = inject(PropuestasService);
  private readonly navegacion = inject(NavegacionService);
  private readonly actividadesService = inject(ActividadesService);
  private readonly toastr = inject(ToastrService);

  protected readonly propuestaId = Number(this.ruta.snapshot.paramMap.get('id'));
  protected readonly destinoId = Number(this.ruta.snapshot.paramMap.get('destinoId'));
  protected readonly propuesta = signal<Propuesta | null>(null);
  protected readonly catalogo = signal<ActividadDisponible[]>([]);
  protected readonly actividades = signal<ActividadArmada[]>([]);
  protected readonly diaAbierto = signal<string | null>(null);
  protected readonly busqueda = signal('');
  protected readonly horaNueva = signal('10:00');
  protected readonly guardando = signal(false);
  protected readonly error = signal<string | null>(null);

  protected readonly destino = computed(() =>
    this.propuesta()?.destinos.find(destino => destino.id === this.destinoId) ?? null);

  protected readonly costoItinerario = computed(() =>
    this.actividades().reduce((total, actividad) => total + actividad.costo, 0));

  protected readonly otrosDestinos = computed(() =>
    (this.propuesta()?.destinos ?? [])
      .filter(destino => destino.id !== this.destinoId && destino.itinerario));

  protected readonly disponible = computed(() =>
    (this.propuesta()?.presupuesto ?? 0)
    - this.costoItinerario()
    - this.otrosDestinos().reduce((total, destino) => total + (destino.itinerario?.costo ?? 0), 0));

  protected readonly porcentajeUsado = computed(() => {
    const presupuesto = this.propuesta()?.presupuesto ?? 0;

    return presupuesto ? Math.min(100, (presupuesto - this.disponible()) / presupuesto * 100) : 0;
  });

  protected readonly pedidas = computed(() =>
    (this.propuesta()?.actividades ?? [])
      .filter(actividad => actividad.destinoId === this.destinoId)
      .map(actividad => ({
        id: actividad.id,
        nombre: actividad.nombre,
        incluida: this.actividades().some(item => item.actividadId === actividad.id)
      })));

  protected readonly dias = computed(() => {
    const destino = this.destino();
    const limites = this.propuesta()?.limites;
    if (!destino || !limites) {
      return [];
    }

    return diasDeEstadia(destino).map(fecha => {
      const actividades = this.deLaFecha(fecha);
      const minutos = minutosOcupados(
        actividades.map(actividad => actividad.duracion),
        fecha === destino.fechaLlegada,
        limites);

      return {
        fecha,
        titulo: describirDia(fecha),
        actividades,
        minutos,
        sobrecargado: minutos > limites.minutosDisponibles
          || actividades.length > limites.maxActividadesPorDia
      };
    });
  });

  protected readonly fueraDeFecha = computed(() => {
    const destino = this.destino();

    return destino
      ? this.actividades().filter(actividad =>
        actividad.fecha < destino.fechaLlegada || actividad.fecha > destino.fechaPartida)
      : [];
  });

  protected readonly opciones = computed(() => {
    const texto = this.busqueda().trim().toLowerCase();

    return this.catalogo().filter(actividad =>
      actividad.nombre.toLowerCase().includes(texto)
      && !this.actividades().some(item => item.actividadId === actividad.id));
  });

  constructor() {
    effect(() => this.navegacion.nombreDestino.set(this.destino()?.descripcion ?? ''));

    forkJoin({
      propuesta: this.propuestasService.obtenerPorId(this.propuestaId),
      actividades: this.actividadesService.obtenerTodas()
    }).subscribe({
      next: ({ propuesta, actividades }) => {
        this.propuesta.set(propuesta);
        this.catalogo.set(actividades.filter(actividad => actividad.destinoId === this.destinoId));
        this.actividades.set((this.destino()?.itinerario?.actividades ?? []).map(actividad => ({
          actividadId: actividad.actividadId,
          nombre: actividad.nombre,
          duracion: actividad.duracionEstimada,
          costo: actividad.costoFinal,
          fecha: fechaDe(actividad.fechaHoraInicio),
          hora: horaDe(actividad.fechaHoraInicio),
          observaciones: actividad.observaciones
        })));

        if (!this.destino()) {
          this.error.set('El destino no pertenece a la propuesta.');
        }

        this.diaAbierto.set(this.ruta.snapshot.queryParamMap.get('dia'));
      },
      error: () => this.error.set('No se pudo cargar el itinerario.')
    });
  }

  protected esPedida(actividadId: number): boolean {
    return this.pedidas().some(pedida => pedida.id === actividadId);
  }

  protected alternarDia(fecha: string): void {
    this.diaAbierto.update(actual => actual === fecha ? null : fecha);
    this.busqueda.set('');
  }

  protected agregar(actividad: ActividadDisponible, fecha: string): void {
    this.actividades.update(actividades => [...actividades, {
      actividadId: actividad.id,
      nombre: actividad.nombre,
      duracion: actividad.duracionEstimada,
      costo: actividad.costoBase * (this.propuesta()?.cantidadViajeros ?? 1),
      fecha,
      hora: this.horaNueva() || '10:00',
      observaciones: ''
    }]);
    this.diaAbierto.set(null);
    this.busqueda.set('');
  }

  protected cambiarHora(actividadId: number, hora: string): void {
    if (!hora) {
      return;
    }

    this.actividades.update(actividades => actividades.map(actividad =>
      actividad.actividadId === actividadId ? { ...actividad, hora } : actividad));
  }

  protected quitar(actividadId: number): void {
    this.actividades.update(actividades =>
      actividades.filter(actividad => actividad.actividadId !== actividadId));
  }

  protected cancelar(): void {
    this.navegacion.volver(`/propuestas/${this.propuestaId}`);
  }

  protected guardar(): void {
    this.guardando.set(true);
    this.propuestasService.guardarItinerario(this.propuestaId, this.destinoId, {
      actividades: this.actividades().map(actividad => ({
        actividadId: actividad.actividadId,
        fechaHoraInicio: `${actividad.fecha}T${actividad.hora}:00Z`,
        observaciones: actividad.observaciones
      }))
    }).subscribe({
      next: () => {
        this.toastr.exito('Itinerario guardado.');
        this.navegacion.terminarEn(`/propuestas/${this.propuestaId}/destinos/${this.destinoId}`);
      },
      error: () => this.guardando.set(false)
    });
  }

  private deLaFecha(fecha: string): ActividadArmada[] {
    return this.actividades()
      .filter(actividad => actividad.fecha === fecha)
      .sort((una, otra) => una.hora.localeCompare(otra.hora));
  }
}
