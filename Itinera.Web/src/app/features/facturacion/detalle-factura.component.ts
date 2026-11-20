import { Component, inject, signal } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { Factura } from '../../core/models/factura';
import { AutenticacionService } from '../../core/services/autenticacion.service';
import { FacturasService } from '../../core/services/facturas.service';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';

@Component({
  selector: 'app-detalle-factura',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    CurrencyPipe,
    DatePipe,
    MenuPrincipalComponent
  ],
  templateUrl: './detalle-factura.component.html'
})
export class DetalleFacturaComponent {
  private readonly ruta = inject(ActivatedRoute);
  private readonly facturasService = inject(FacturasService);
  private readonly autenticacion = inject(AutenticacionService);
  private readonly formularioConstructor = inject(FormBuilder);

  protected readonly factura = signal<Factura | null>(null);
  protected readonly cargando = signal(true);
  protected readonly modalPagoAbierto = signal(false);
  protected readonly procesando = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly aviso = signal('');

  protected readonly formularioPago = this.formularioConstructor.nonNullable.group({
    importe: [0, [Validators.required, Validators.min(0.01)]],
    fecha: [this.fechaHoy(), Validators.required],
    medioPago: ['', [Validators.required, Validators.maxLength(100)]]
  });

  constructor() {
    this.cargarFactura();
  }

  protected puedeRegistrarPago(): boolean {
    return this.autenticacion.tienePermiso('facturacion.registrar-pago');
  }

  protected puedeAnular(): boolean {
    return this.autenticacion.tienePermiso('facturacion.anular');
  }

  protected puedePagar(): boolean {
    const estado = this.factura()?.estado;
    return estado === 'Pendiente' || estado === 'ParcialmentePagada';
  }

  protected abrirModalPago(): void {
    this.formularioPago.reset({
      importe: 0,
      fecha: this.fechaHoy(),
      medioPago: ''
    });
    this.error.set(null);
    this.modalPagoAbierto.set(true);
  }

  protected cerrarModalPago(): void {
    if (!this.procesando()) {
      this.modalPagoAbierto.set(false);
    }
  }

  protected registrarPago(): void {
    const actual = this.factura();
    if (!actual || this.formularioPago.invalid) {
      this.formularioPago.markAllAsTouched();
      return;
    }

    this.procesando.set(true);
    this.error.set(null);
    this.facturasService.registrarPago(actual.id, this.formularioPago.getRawValue()).subscribe({
      next: factura => {
        this.factura.set(factura);
        this.procesando.set(false);
        this.modalPagoAbierto.set(false);
        this.mostrarAviso('Pago registrado correctamente');
      },
      error: respuesta => {
        this.procesando.set(false);
        this.error.set(respuesta.error?.detail ?? 'No se pudo registrar el pago.');
      }
    });
  }

  protected anular(): void {
    const actual = this.factura();
    if (!actual || !window.confirm(`¿Querés anular la factura ${actual.numero}?`)) {
      return;
    }

    this.procesando.set(true);
    this.error.set(null);
    this.facturasService.anular(actual.id).subscribe({
      next: () => {
        this.factura.update(factura => factura
          ? { ...factura, estado: 'Anulada' }
          : factura);
        this.procesando.set(false);
        this.mostrarAviso('Factura anulada correctamente');
      },
      error: respuesta => {
        this.procesando.set(false);
        this.error.set(respuesta.error?.detail ?? 'No se pudo anular la factura.');
      }
    });
  }

  protected descargarPdf(): void {
    const actual = this.factura();
    if (!actual) {
      return;
    }

    this.facturasService.descargarPdf(actual.id).subscribe({
      next: contenido => {
        const url = URL.createObjectURL(contenido);
        const enlace = document.createElement('a');
        enlace.href = url;
        enlace.download = `${actual.numero}.pdf`;
        enlace.click();
        URL.revokeObjectURL(url);
      },
      error: () => this.error.set('No se pudo descargar el PDF.')
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

  private cargarFactura(): void {
    const id = Number(this.ruta.snapshot.paramMap.get('id'));
    this.facturasService.obtenerPorId(id).subscribe({
      next: factura => {
        this.factura.set(factura);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set('No se pudo cargar la factura.');
        this.cargando.set(false);
      }
    });
  }

  protected fechaHoy(): string {
    return new Date().toISOString().slice(0, 10);
  }

  private mostrarAviso(mensaje: string): void {
    this.aviso.set(mensaje);
    setTimeout(() => this.aviso.set(''), 2600);
  }
}
