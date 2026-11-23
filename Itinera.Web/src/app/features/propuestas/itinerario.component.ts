import { Component, computed, effect, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { Propuesta } from '../../core/models/propuesta';
import { AutenticacionService } from '../../core/services/autenticacion.service';
import { DestinoApi, DestinosService } from '../../core/services/destinos.service';
import { NavegacionService } from '../../core/services/navegacion.service';
import { AlternativaDia, PropuestasService } from '../../core/services/propuestas.service';
import { ToastrService } from '../../core/services/toastr.service';
import { describirDia, diasDeEstadia, fechaDe, horaDe } from '../../core/utils/fechas-itinerario';
import { MonedaPipe } from '../../shared/pipes/moneda.pipe';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';

@Component({
  selector: 'app-itinerario',
  imports: [RouterLink, MonedaPipe, DatePipe, MenuPrincipalComponent],
  templateUrl: './itinerario.component.html'
})
export class ItinerarioComponent {
  private readonly ruta = inject(ActivatedRoute);
  private readonly propuestasService = inject(PropuestasService);
  private readonly navegacion = inject(NavegacionService);
  private readonly destinosService = inject(DestinosService);
  private readonly autenticacion = inject(AutenticacionService);
  private readonly toastr = inject(ToastrService);

  protected readonly propuestaId = Number(this.ruta.snapshot.paramMap.get('id'));
  protected readonly destinoId = Number(this.ruta.snapshot.paramMap.get('destinoId'));
  protected readonly propuesta = signal<Propuesta | null>(null);
  protected readonly error = signal<string | null>(null);
  protected readonly diaAbierto = signal<string | null>(null);
  protected readonly pedido = signal('');
  protected readonly cambios = signal<AlternativaDia[]>([]);
  protected readonly sinCambios = signal(false);
  protected readonly procesando = signal(false);
  protected readonly cambiandoDestino = signal(false);
  protected readonly destinos = signal<DestinoApi[]>([]);
  protected readonly nuevoDestinoId = signal(0);
  protected readonly horaDe = horaDe;

  protected readonly destino = computed(() =>
    this.propuesta()?.destinos.find(destino => destino.id === this.destinoId) ?? null);

  protected readonly editable = computed(() =>
    !['Aceptada', 'Eliminada'].includes(this.propuesta()?.estado ?? ''));

  protected readonly dias = computed(() => {
    const destino = this.destino();
    const actividades = destino?.itinerario?.actividades ?? [];

    return destino
      ? diasDeEstadia(destino).map(fecha => ({
        fecha,
        titulo: describirDia(fecha),
        actividades: actividades.filter(actividad => fechaDe(actividad.fechaHoraInicio) === fecha)
      }))
      : [];
  });

  constructor() {
    effect(() => this.navegacion.nombreDestino.set(this.destino()?.descripcion ?? ''));

    this.propuestasService.obtenerPorId(this.propuestaId).subscribe({
      next: propuesta => {
        this.propuesta.set(propuesta);

        if (!this.destino()) {
          this.error.set('El destino no pertenece a la propuesta.');
        }
      },
      error: () => this.error.set('No se pudo cargar el itinerario.')
    });
  }

  protected puedeEditar(): boolean {
    return this.editable() && this.autenticacion.tienePermiso('itinerarios.editar');
  }

  protected puedeUsarIa(): boolean {
    return this.editable() && this.autenticacion.tienePermiso('itinerarios.generar-ia');
  }

  protected puedeCambiarDestino(): boolean {
    return this.puedeUsarIa() && this.autenticacion.tienePermiso('propuestas.editar');
  }

  protected volverAlDetalle(): void {
    this.navegacion.terminarEn(`/propuestas/${this.propuestaId}`);
  }

  protected alternarDestino(): void {
    this.cerrarDia();
    this.cambiandoDestino.update(valor => !valor);

    if (this.cambiandoDestino() && !this.destinos().length) {
      this.destinosService.obtenerTodos().subscribe(destinos => {
        const usados = this.propuesta()?.destinos.map(destino => destino.id) ?? [];
        this.destinos.set(destinos.filter(destino => !usados.includes(destino.id)));
        this.nuevoDestinoId.set(this.destinos()[0]?.id ?? 0);
      });
    }
  }

  protected alternarDia(fecha: string): void {
    const abierto = this.diaAbierto() === fecha;
    this.cerrarDia();
    this.cambiandoDestino.set(false);
    this.diaAbierto.set(abierto ? null : fecha);
  }

  protected cerrarDia(): void {
    this.diaAbierto.set(null);
    this.pedido.set('');
    this.descartarCambios();
  }

  protected descartarCambios(): void {
    this.cambios.set([]);
    this.sinCambios.set(false);
  }

  protected pedirCambio(fecha: string): void {
    const instrucciones = this.pedido().trim();
    if (!instrucciones) {
      this.toastr.advertencia('Escribí qué querés cambiar.');
      return;
    }

    this.procesando.set(true);
    this.descartarCambios();
    this.propuestasService.generarAlternativasDia(
      this.propuestaId,
      this.destinoId,
      fecha,
      instrucciones
    ).subscribe({
      next: cambios => {
        this.procesando.set(false);
        this.cambios.set(cambios);
        this.sinCambios.set(!cambios.length);
      },
      error: respuesta => {
        this.procesando.set(false);
        if (respuesta.status === 503) {
          this.toastr.error(respuesta.error?.detail ?? 'El servicio de IA no está disponible.');
        }
      }
    });
  }

  protected aplicar(cambio: AlternativaDia): void {
    this.procesando.set(true);
    this.propuestasService.aplicarAlternativaDia(
      this.propuestaId,
      this.destinoId,
      cambio
    ).subscribe({
      next: propuesta => {
        this.procesando.set(false);
        this.propuesta.set(propuesta);
        this.cerrarDia();
        this.toastr.exito('Cambio aplicado.');
      },
      error: () => this.procesando.set(false)
    });
  }
}
