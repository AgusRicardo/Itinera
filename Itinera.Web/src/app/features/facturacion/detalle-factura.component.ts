import { Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

import { Factura } from '../../core/models/factura';
import { AutenticacionService } from '../../core/services/autenticacion.service';
import { FacturasService } from '../../core/services/facturas.service';
import { ToastrService } from '../../core/services/toastr.service';
import { fechaLocalIso } from '../../core/utils/fechas';
import { InputMonedaComponent } from '../../shared/components/input-moneda/input-moneda.component';
import { MonedaPipe } from '../../shared/pipes/moneda.pipe';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';
import { ConfirmacionService } from '../../core/services/confirmacion.service';
import { textoRequerido } from '../../core/utils/validadores';
import { EstadoComponent } from '../../shared/components/estado/estado.component';
import { SelectorFechaComponent } from '../../shared/components/selector-fecha/selector-fecha.component';

@Component({
  selector: 'app-detalle-factura',
  imports: [
    ReactiveFormsModule,
    InputMonedaComponent,
    MonedaPipe,
    DatePipe,
    MenuPrincipalComponent,
    EstadoComponent,
    SelectorFechaComponent
  ],
  templateUrl: './detalle-factura.component.html'
})
export class DetalleFacturaComponent {
  private readonly ruta = inject(ActivatedRoute);
  private readonly facturasService = inject(FacturasService);
  private readonly autenticacion = inject(AutenticacionService);
  private readonly formularioConstructor = inject(FormBuilder);
  private readonly confirmacion = inject(ConfirmacionService);
  private readonly toastr = inject(ToastrService);

  protected readonly factura = signal<Factura | null>(null);
  protected readonly cargando = signal(true);
  protected readonly modalPagoAbierto = signal(false);
  protected readonly procesando = signal(false);
  protected readonly error = signal<string | null>(null);

  protected readonly formularioPago = this.formularioConstructor.nonNullable.group({
    importe: [0, [Validators.required, Validators.min(0.01)]],
    fecha: [this.fechaHoy(), Validators.required],
    medioPago: ['', [textoRequerido, Validators.maxLength(100)]]
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
    this.facturasService.registrarPago(actual.id, this.formularioPago.getRawValue()).subscribe({
      next: factura => {
        this.factura.set(factura);
        this.procesando.set(false);
        this.modalPagoAbierto.set(false);
        this.toastr.exito('Pago registrado.');
      },
      error: () => this.procesando.set(false)
    });
  }

  protected async anular(): Promise<void> {
    const actual = this.factura();
    if (!actual) {
      return;
    }

    const confirmado = await this.confirmacion.confirmar({
      titulo: `¿Anular la factura ${actual.numero}?`,
      mensaje: 'No se puede deshacer.',
      accion: 'Anular',
      peligro: true
    });

    if (!confirmado) {
      return;
    }

    this.procesando.set(true);
    this.facturasService.anular(actual.id).subscribe({
      next: () => {
        this.factura.set({ ...actual, estado: 'Anulada' });
        this.procesando.set(false);
        this.toastr.exito('Factura anulada.');
      },
      error: () => this.procesando.set(false)
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
      error: () => this.toastr.error('No se pudo descargar el PDF.')
    });
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
    return fechaLocalIso(new Date());
  }
}
