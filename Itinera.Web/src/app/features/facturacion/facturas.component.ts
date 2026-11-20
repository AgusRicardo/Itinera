import { Component, inject, signal } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { Propuesta } from '../../core/models/propuesta';
import { Factura } from '../../core/models/factura';
import { FacturasService } from '../../core/services/facturas.service';
import { PropuestasService } from '../../core/services/propuestas.service';
import { AutenticacionService } from '../../core/services/autenticacion.service';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';

@Component({
  selector: 'app-facturas',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    CurrencyPipe,
    DatePipe,
    MenuPrincipalComponent
  ],
  templateUrl: './facturas.component.html'
})
export class FacturasComponent {
  private readonly facturasService = inject(FacturasService);
  private readonly propuestasService = inject(PropuestasService);
  private readonly autenticacion = inject(AutenticacionService);
  private readonly formularioConstructor = inject(FormBuilder);
  private readonly ruta = inject(ActivatedRoute);

  protected readonly facturas = signal<Factura[]>([]);
  protected readonly propuestasAceptadas = signal<Propuesta[]>([]);
  protected readonly cargando = signal(true);
  protected readonly modalAbierto = signal(false);
  protected readonly guardando = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly aviso = signal('');

  protected readonly formulario = this.formularioConstructor.nonNullable.group({
    propuestaId: [0, [Validators.required, Validators.min(1)]],
    fechaVencimiento: [this.fechaMinima(), Validators.required],
    detalles: this.formularioConstructor.array([this.crearDetalle()])
  });

  constructor() {
    this.cargarFacturas();
    this.cargarPropuestasAceptadas();
  }

  protected puedeCrear(): boolean {
    return this.autenticacion.tienePermiso('facturacion.crear');
  }

  protected obtenerDetalles(): FormArray {
    return this.formulario.controls.detalles;
  }

  protected abrirModal(propuestaId = 0): void {
    this.formulario.reset({
      propuestaId,
      fechaVencimiento: this.fechaMinima()
    });
    this.obtenerDetalles().clear();
    this.agregarDetalle();
    this.error.set(null);
    this.modalAbierto.set(true);
  }

  protected cerrarModal(): void {
    if (!this.guardando()) {
      this.modalAbierto.set(false);
    }
  }

  protected agregarDetalle(): void {
    this.obtenerDetalles().push(this.crearDetalle());
  }

  protected quitarDetalle(indice: number): void {
    if (this.obtenerDetalles().length > 1) {
      this.obtenerDetalles().removeAt(indice);
    }
  }

  protected guardar(): void {
    if (this.formulario.invalid) {
      this.formulario.markAllAsTouched();
      this.error.set('Completá la propuesta, el vencimiento y todos los conceptos.');
      return;
    }

    this.guardando.set(true);
    this.error.set(null);
    const request = this.formulario.getRawValue();

    this.facturasService.crear(request).subscribe({
      next: () => {
        this.guardando.set(false);
        this.modalAbierto.set(false);
        this.cargarFacturas();
        this.mostrarAviso('Factura creada correctamente');
      },
      error: respuesta => {
        this.guardando.set(false);
        this.error.set(
          respuesta.error?.detail ?? 'No se pudo generar la factura.'
        );
      }
    });
  }

  protected claseEstado(estado: string): string {
    return {
      Pendiente: 'bg-amber-50 text-amber-700',
      ParcialmentePagada: 'bg-blue-50 text-blue-700',
      Pagada: 'bg-emerald-50 text-emerald-700',
      Anulada: 'bg-red-50 text-red-700'
    }[estado] ?? 'bg-stone-100 text-stone-600';
  }

  private crearDetalle() {
    return this.formularioConstructor.nonNullable.group({
      descripcion: ['', Validators.required],
      cantidad: [1, [Validators.required, Validators.min(0.01)]],
      precioUnitario: [0, [Validators.required, Validators.min(0)]]
    });
  }

  private cargarFacturas(): void {
    this.facturasService.obtenerTodas().subscribe({
      next: facturas => {
        this.facturas.set(facturas);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set('No se pudieron cargar las facturas.');
        this.cargando.set(false);
      }
    });
  }

  private cargarPropuestasAceptadas(): void {
    this.propuestasService.obtenerTodas().subscribe({
      next: propuestas => {
        const aceptadas = propuestas.filter(propuesta => propuesta.estado === 'Aceptada');
        this.propuestasAceptadas.set(aceptadas);
        const propuestaId = Number(this.ruta.snapshot.queryParamMap.get('propuestaId'));
        if (propuestaId && aceptadas.some(propuesta => propuesta.id === propuestaId)) {
          this.abrirModal(propuestaId);
        }
      }
    });
  }

  protected fechaMinima(): string {
    return new Date().toISOString().slice(0, 10);
  }

  private mostrarAviso(mensaje: string): void {
    this.aviso.set(mensaje);
    setTimeout(() => this.aviso.set(''), 2600);
  }
}
