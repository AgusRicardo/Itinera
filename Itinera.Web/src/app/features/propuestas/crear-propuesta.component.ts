import { DatePipe } from '@angular/common';
import { Component, Input, OnInit, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { forkJoin, map } from 'rxjs';

import { OpcionPropuesta } from '../../core/models/opciones-propuesta';
import { ClientesService } from '../../core/services/clientes.service';
import { DestinosService } from '../../core/services/destinos.service';
import { EmpleadosService } from '../../core/services/empleados.service';
import { Propuesta } from '../../core/models/propuesta';
import { NavegacionService } from '../../core/services/navegacion.service';
import { EstadiaRequest, PropuestasService } from '../../core/services/propuestas.service';
import { AutenticacionService } from '../../core/services/autenticacion.service';
import { Etiqueta } from '../../core/models/etiqueta';
import { EtiquetasService } from '../../core/services/etiquetas.service';
import { ActividadesService } from '../../core/services/actividades.service';
import {
  BuscadorOpcionesComponent,
  OpcionBuscable
} from '../../shared/components/buscador-opciones/buscador-opciones.component';
import { InputMonedaComponent } from '../../shared/components/input-moneda/input-moneda.component';
import { RangoFechas, RangoFechasComponent } from '../../shared/components/rango-fechas/rango-fechas.component';
import { MonedaPipe } from '../../shared/pipes/moneda.pipe';
import { ConfirmacionService } from '../../core/services/confirmacion.service';
import { ToastrService } from '../../core/services/toastr.service';
import { SelectorComponent } from '../../shared/components/selector/selector.component';

@Component({
  selector: 'app-crear-propuesta',
  imports: [
    ReactiveFormsModule,
    DatePipe,
    BuscadorOpcionesComponent,
    InputMonedaComponent,
    RangoFechasComponent,
    MonedaPipe,
    SelectorComponent
  ],
  templateUrl: './crear-propuesta.component.html'
})
export class CrearPropuestaComponent implements OnInit {
  private readonly confirmacion = inject(ConfirmacionService);
  private readonly toastr = inject(ToastrService);
  @Input() propuesta: Propuesta | null = null;
  private readonly constructorFormulario = inject(FormBuilder);
  private readonly propuestasService = inject(PropuestasService);
  private readonly clientesService = inject(ClientesService);
  private readonly empleadosService = inject(EmpleadosService);
  private readonly destinosService = inject(DestinosService);
  private readonly navegacion = inject(NavegacionService);
  private readonly autenticacion = inject(AutenticacionService);
  private readonly etiquetasService = inject(EtiquetasService);
  private readonly actividadesService = inject(ActividadesService);

  protected readonly cargando = signal(false);
  protected readonly estadias = signal<EstadiaRequest[]>([]);
  protected readonly destinosSeleccionados = computed(() =>
    this.estadias().map(estadia => estadia.destinoId));

  protected readonly clientes = signal<OpcionPropuesta[]>([]);
  protected readonly empleados = signal<OpcionPropuesta[]>([]);
  protected readonly destinos = signal<OpcionPropuesta[]>([]);
  protected readonly etiquetas = signal<Pick<Etiqueta, 'id' | 'nombre'>[]>([]);
  protected readonly etiquetasSeleccionadas = signal<number[]>([]);
  protected readonly actividades = signal<Propuesta['actividades']>([]);
  protected readonly actividadesSeleccionadas = signal<number[]>([]);

  protected readonly formulario = this.constructorFormulario.nonNullable.group({
    clienteId: [0, [Validators.required, Validators.min(1)]],
    empleadoId: [0, [Validators.required, Validators.min(1)]],
    presupuesto: [5000, [Validators.required, Validators.min(1)]],
    cantidadViajeros: [1, [Validators.required, Validators.min(1)]],
    fechaInicio: ['', Validators.required],
    fechaFin: ['', Validators.required],
    preferencias: [''],
    intensidadViaje: ['Media' as 'Baja' | 'Media' | 'Alta', Validators.required]
  });

  protected readonly valores = toSignal(
    this.formulario.valueChanges.pipe(map(() => this.formulario.getRawValue())),
    { initialValue: this.formulario.getRawValue() }
  );

  protected readonly intensidades = [
    { valor: 'Baja', descripcion: 'Descansos amplios' },
    { valor: 'Media', descripcion: 'Ritmo equilibrado' },
    { valor: 'Alta', descripcion: 'Aprovecha el día' }
  ];

  protected readonly faltantes = computed(() => {
    const valores = this.valores();

    return [
      { texto: 'cliente', falta: valores.clienteId < 1 },
      { texto: 'empleado responsable', falta: valores.empleadoId < 1 },
      { texto: 'presupuesto', falta: !(valores.presupuesto >= 1) },
      { texto: 'viajeros', falta: !(valores.cantidadViajeros >= 1) },
      { texto: 'fechas del viaje', falta: !valores.fechaInicio || !valores.fechaFin },
      { texto: 'al menos un destino', falta: !this.estadias().length },
      {
        texto: 'fechas de cada destino',
        falta: this.estadias().some(estadia => !estadia.fechaLlegada || !estadia.fechaPartida)
      }
    ].filter(item => item.falta).map(item => item.texto);
  });

  protected readonly opcionesDestinos = computed<OpcionBuscable[]>(() =>
    this.conLosUsados(this.destinos(), this.propuesta?.destinos
      .map(destino => ({ id: destino.id, nombre: destino.descripcion })))
      .filter(destino => !this.destinosSeleccionados().includes(destino.id))
      .map(destino => ({ id: destino.id.toString(), nombre: destino.nombre })));

  protected readonly opcionesActividades = computed<OpcionBuscable[]>(() =>
    this.actividades()
      .filter(actividad =>
        this.destinosSeleccionados().includes(actividad.destinoId)
        && !this.actividadesSeleccionadas().includes(actividad.id))
      .map(actividad => ({
        id: actividad.id.toString(),
        nombre: `${actividad.nombre} (${this.nombreDestino(actividad.destinoId)})`
      })));

  protected readonly actividadesElegidas = computed(() =>
    this.actividades().filter(actividad =>
      this.actividadesSeleccionadas().includes(actividad.id)));

  protected readonly nombreCliente = computed(() =>
    this.clientes().find(cliente => cliente.id === this.valores().clienteId)?.nombre ?? '');

  ngOnInit(): void {
    if (this.propuesta) {
      this.cargarPropuesta(this.propuesta);
    }

    this.cargarOpciones();
  }

  private cargarPropuesta(propuesta: Propuesta): void {
    this.formulario.patchValue({
      clienteId: propuesta.clienteId,
      empleadoId: propuesta.empleadoId,
      presupuesto: propuesta.presupuesto,
      cantidadViajeros: propuesta.cantidadViajeros,
      fechaInicio: propuesta.fechaInicio,
      fechaFin: propuesta.fechaFin,
      preferencias: propuesta.preferencias,
      intensidadViaje: propuesta.intensidadViaje
    });
    this.formulario.controls.empleadoId.disable();
    this.formulario.controls.clienteId.disable();
    this.clientes.set([{ id: propuesta.clienteId, nombre: propuesta.clienteNombre }]);
    this.empleados.set([{ id: propuesta.empleadoId, nombre: propuesta.empleadoNombre }]);
    this.estadias.set(propuesta.destinos.map(destino => ({
      destinoId: destino.id,
      fechaLlegada: destino.fechaLlegada,
      fechaPartida: destino.fechaPartida
    })));
    this.actividadesSeleccionadas.set(propuesta.actividades.map(actividad => actividad.id));
    this.etiquetasSeleccionadas.set(propuesta.etiquetas.map(etiqueta => etiqueta.id));
  }

  protected nombreDestino(id: number): string {
    return this.destinos().find(destino => destino.id === id)?.nombre
      ?? this.propuesta?.destinos.find(destino => destino.id === id)?.descripcion
      ?? 'Destino';
  }

  protected cambiarEstadia(id: number, rango: RangoFechas): void {
    this.estadias.update(estadias => estadias.map(estadia =>
      estadia.destinoId === id
        ? { ...estadia, fechaLlegada: rango.desde, fechaPartida: rango.hasta }
        : estadia));
  }

  protected cambiarViaje(rango: RangoFechas): void {
    this.formulario.patchValue({ fechaInicio: rango.desde, fechaFin: rango.hasta });
    this.cambiaronFechas();
  }

  protected cambiarViajeros(cantidad: number): void {
    const { cantidadViajeros } = this.formulario.controls;
    cantidadViajeros.setValue(Math.max(1, Number(cantidadViajeros.value) + cantidad));
  }

  protected escribirViajeros(campo: HTMLInputElement): void {
    const cantidad = Math.min(9999, Number(campo.value.replace(/\D/g, '')));
    campo.value = cantidad ? cantidad.toString() : '';
    this.formulario.controls.cantidadViajeros.setValue(cantidad);
  }

  private cambiaronFechas(): void {
    if (!this.propuesta) {
      this.repartirDias();
    }
  }

  protected repartirDias(): void {
    const { fechaInicio, fechaFin } = this.formulario.getRawValue();
    if (!fechaInicio || !fechaFin || fechaFin < fechaInicio) {
      return;
    }

    const milisegundosPorDia = 86_400_000;
    const inicio = Date.parse(`${fechaInicio}T00:00:00Z`);
    const dias = (Date.parse(`${fechaFin}T00:00:00Z`) - inicio) / milisegundosPorDia;
    const cantidad = this.estadias().length;
    const fecha = (posicion: number): string =>
      new Date(inicio + Math.floor(dias * posicion / cantidad) * milisegundosPorDia)
        .toISOString()
        .slice(0, 10);

    this.estadias.update(estadias => estadias.map((estadia, indice) => ({
      ...estadia,
      fechaLlegada: fecha(indice),
      fechaPartida: fecha(indice + 1)
    })));
  }

  /** Suma los que la propuesta ya usa aunque estén desactivados en el catálogo. */
  private conLosUsados<T extends { id: number }>(disponibles: T[], usados: T[] = []): T[] {
    return [
      ...disponibles,
      ...usados.filter(usado => disponibles.every(disponible => disponible.id !== usado.id))
    ];
  }

  private cargarOpciones(): void {
    forkJoin({
      clientes: this.clientesService.obtenerTodos(),
      empleados: this.empleadosService.obtenerTodos(),
      destinos: this.destinosService.obtenerTodos(),
      etiquetas: this.etiquetasService.obtenerTodas(),
      actividades: this.actividadesService.obtenerTodas()
    }).subscribe({
      next: respuesta => {
        this.destinos.set(
          respuesta.destinos.map(destino => ({
            id: destino.id,
            nombre: `${destino.ciudadNombre}, ${destino.paisNombre}`
          }))
        );
        this.etiquetas.set(this.conLosUsados(respuesta.etiquetas, this.propuesta?.etiquetas));
        this.actividades.set(this.conLosUsados(respuesta.actividades, this.propuesta?.actividades));

        if (this.propuesta) {
          return;
        }

        this.clientes.set(respuesta.clientes.map(cliente => ({
          id: cliente.id,
          nombre: `${cliente.nombre} ${cliente.apellido}`.trim()
        })));
        this.empleados.set(respuesta.empleados.map(empleado => ({
          id: empleado.id,
          nombre: `${empleado.nombre} ${empleado.apellido}`.trim()
        })));

        const usuario = this.autenticacion.obtenerUsuario();
        const empleadoActual = respuesta.empleados.find(empleado =>
          empleado.email.toLowerCase() === usuario?.email.toLowerCase());

        if (empleadoActual) {
          this.formulario.controls.empleadoId.setValue(empleadoActual.id);
        } else if (respuesta.empleados.length) {
          this.formulario.controls.empleadoId.setValue(respuesta.empleados[0].id);
        }
      },
      error: () => this.toastr.error('No se pudieron cargar los datos de la propuesta.')
    });
  }

  protected agregarDestino(opcion: OpcionBuscable): void {
    this.alternarDestino(Number(opcion.id));
  }

  protected async alternarDestino(id: number): Promise<void> {
    const { fechaInicio, fechaFin } = this.formulario.getRawValue();

    if (this.destinosSeleccionados().includes(id)) {
      const tieneItinerario = this.propuesta?.destinos
        .some(destino => destino.id === id && destino.itinerario);

      if (tieneItinerario && !await this.confirmacion.confirmar({
        titulo: `¿Quitar ${this.nombreDestino(id)}?`,
        mensaje: 'Al guardar la propuesta se elimina su itinerario.',
        accion: 'Quitar',
        peligro: true
      })) {
        return;
      }

      this.estadias.update(estadias => estadias.filter(estadia => estadia.destinoId !== id));
    } else {
      const ultima = this.estadias().at(-1);
      this.estadias.update(estadias => [...estadias, {
        destinoId: id,
        fechaLlegada: ultima?.fechaPartida || fechaInicio,
        fechaPartida: fechaFin
      }]);
    }

    this.cambiaronFechas();
    this.actividadesSeleccionadas.update(actividades =>
      actividades.filter(actividadId =>
        this.actividades().some(actividad =>
          actividad.id === actividadId && this.destinosSeleccionados().includes(actividad.destinoId))));
  }

  protected alternarEtiqueta(id: number): void {
    const seleccionadas = this.etiquetasSeleccionadas();
    this.etiquetasSeleccionadas.set(
      seleccionadas.includes(id)
        ? seleccionadas.filter(etiquetaId => etiquetaId !== id)
        : [...seleccionadas, id]
    );
  }

  protected etiquetaSeleccionada(id: number): boolean {
    return this.etiquetasSeleccionadas().includes(id);
  }

  protected agregarActividad(opcion: OpcionBuscable): void {
    const id = Number(opcion.id);
    this.actividadesSeleccionadas.update(actividades =>
      actividades.includes(id) ? actividades : [...actividades, id]);
  }

  protected quitarActividad(id: number): void {
    this.actividadesSeleccionadas.update(actividades =>
      actividades.filter(actividadId => actividadId !== id));
  }

  protected crear(): void {
    if (
      this.formulario.invalid
      || this.estadias().length === 0
      || this.estadias().some(estadia => !estadia.fechaLlegada || !estadia.fechaPartida)
    ) {
      this.formulario.markAllAsTouched();
      this.toastr.advertencia(
        'Completá los campos obligatorios y las fechas de al menos un destino.');
      return;
    }

    this.cargando.set(true);

    const { clienteId, empleadoId, ...configuracion } = this.formulario.getRawValue();
    const request = {
      ...configuracion,
      destinos: this.estadias(),
      actividadIds: this.actividadesSeleccionadas(),
      etiquetaIds: this.etiquetasSeleccionadas()
    };
    const guardado$ = this.propuesta
      ? this.propuestasService.actualizar(this.propuesta.id, request)
      : this.propuestasService.crear({ ...request, clienteId, empleadoId });

    guardado$.subscribe({
      next: propuesta => {
        this.cargando.set(false);
        this.toastr.exito(this.propuesta ? 'Propuesta actualizada.' : 'Propuesta creada.');
        this.navegacion.terminarEn(`/propuestas/${propuesta.id}`);
      },
      error: () => this.cargando.set(false)
    });
  }

  protected salir(): void {
    this.navegacion.volver(this.propuesta ? `/propuestas/${this.propuesta.id}` : '/propuestas');
  }
}
