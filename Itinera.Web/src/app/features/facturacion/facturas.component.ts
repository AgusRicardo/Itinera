import { Component, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { Propuesta } from '../../core/models/propuesta';
import { Factura } from '../../core/models/factura';
import { Concepto, ConceptosService } from '../../core/services/conceptos.service';
import { EstadoFactura, EstadosFacturasService } from '../../core/services/estados-facturas.service';
import { FacturasService } from '../../core/services/facturas.service';
import { PropuestasService } from '../../core/services/propuestas.service';
import { AutenticacionService } from '../../core/services/autenticacion.service';
import { fechaLocalIso } from '../../core/utils/fechas';
import { BuscadorOpcionesComponent, OpcionBuscable } from '../../shared/components/buscador-opciones/buscador-opciones.component';
import { MonedaPipe } from '../../shared/pipes/moneda.pipe';
import { GrillaPaginada } from '../../core/utils/grilla-paginada';
import { ColumnaOrdenableComponent } from '../../shared/components/columna-ordenable/columna-ordenable.component';
import { FiltrosGrillaComponent } from '../../shared/components/filtros-grilla/filtros-grilla.component';
import { AccionMenu, MenuAccionesComponent } from '../../shared/components/menu-acciones/menu-acciones.component';
import { EstadoComponent } from '../../shared/components/estado/estado.component';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';
import { ToastrService } from '../../core/services/toastr.service';
import { EncabezadoComponent } from '../../shared/components/encabezado/encabezado.component';
import { TablaComponent } from '../../shared/components/tabla/tabla.component';
import { OpcionSelector, SelectorComponent } from '../../shared/components/selector/selector.component';
import { SelectorFechaComponent } from '../../shared/components/selector-fecha/selector-fecha.component';
import { InputMonedaComponent } from '../../shared/components/input-moneda/input-moneda.component';

@Component({
  selector: 'app-facturas',
  imports: [
    ReactiveFormsModule,
    BuscadorOpcionesComponent,
    RouterLink,
    MonedaPipe,
    DatePipe,
    ColumnaOrdenableComponent,
    FiltrosGrillaComponent,
    MenuAccionesComponent,
    EstadoComponent,
    MenuPrincipalComponent,
    EncabezadoComponent,
    TablaComponent,
    SelectorComponent,
    SelectorFechaComponent,
    InputMonedaComponent
  ],
  templateUrl: './facturas.component.html'
})
export class FacturasComponent {
  private readonly toastr = inject(ToastrService);
  private readonly facturasService = inject(FacturasService);
  private readonly propuestasService = inject(PropuestasService);
  private readonly conceptosService = inject(ConceptosService);
  private readonly estadosService = inject(EstadosFacturasService);
  private readonly autenticacion = inject(AutenticacionService);
  private readonly formularioConstructor = inject(FormBuilder);
  private readonly ruta = inject(ActivatedRoute);

  protected readonly grilla = new GrillaPaginada<Factura>(
    consulta => this.facturasService.obtenerPagina(consulta),
    'No se pudieron cargar las facturas.');
  protected readonly propuestasAceptadas = signal<Propuesta[]>([]);
  protected readonly conceptos = signal<Concepto[]>([]);
  protected readonly estados = signal<EstadoFactura[]>([]);
  protected readonly modalAbierto = signal(false);
  protected readonly guardando = signal(false);

  protected readonly formulario = this.formularioConstructor.nonNullable.group({
    propuestaId: [0, [Validators.required, Validators.min(1)]],
    fechaVencimiento: [this.fechaMinima(), Validators.required],
    detalles: this.formularioConstructor.array([this.crearDetalle()])
  });

  protected readonly opcionesPropuestas = computed<OpcionSelector[]>(() =>
    this.propuestasAceptadas().map(propuesta => ({
      id: propuesta.id,
      nombre: `#${propuesta.id} — ${propuesta.clienteNombre}`
    })));

  constructor() {
    this.estadosService.obtenerTodos().subscribe(estados => this.estados.set(estados));
    this.cargarPropuestasAceptadas();
  }

  protected puedeCrear(): boolean {
    return this.autenticacion.tienePermiso('facturacion.crear');
  }

  protected puedeGestionarConceptos(): boolean {
    return this.autenticacion.tienePermiso('conceptos.gestionar');
  }

  protected acciones(factura: Factura): AccionMenu[] {
    return [{ texto: 'Ver detalle', ruta: ['/facturacion', factura.id.toString()] }];
  }

  protected opcionesConceptos(): OpcionBuscable[] {
    const elegidos = this.obtenerDetalles().getRawValue().map(detalle => detalle.conceptoId);

    return this.conceptos()
      .filter(concepto => !elegidos.includes(concepto.id))
      .map(concepto => ({ id: concepto.id.toString(), nombre: concepto.descripcion }));
  }

  protected agregarConcepto(opcion: OpcionBuscable): void {
    const concepto = this.conceptos().find(item => item.id === Number(opcion.id));

    if (concepto) {
      const detalle = this.crearDetalle();
      detalle.patchValue({ conceptoId: concepto.id });
      this.obtenerDetalles().push(detalle);
    }
  }

  protected total(): number {
    return this.costoDelViaje() + this.totalConceptos();
  }

  protected conceptoDe(indice: number): Concepto | undefined {
    const { conceptoId } = this.obtenerDetalles().at(indice).value;

    return this.conceptos().find(item => item.id === conceptoId);
  }

  protected costoDelViaje(): number {
    const { propuestaId } = this.formulario.getRawValue();

    return this.propuestasAceptadas().find(item => item.id === propuestaId)?.presupuestoUsado ?? 0;
  }

  protected subtotal(indice: number): number {
    const concepto = this.conceptoDe(indice);

    if (concepto?.porcentaje != null) {
      return Math.round(this.costoDelViaje() * concepto.porcentaje) / 100;
    }

    return (this.obtenerDetalles().at(indice).value.cantidad ?? 0) * (concepto?.precio ?? 0);
  }

  protected totalConceptos(): number {
    return this.obtenerDetalles().controls.reduce((suma, _, indice) => suma + this.subtotal(indice), 0);
  }

  protected obtenerDetalles() {
    return this.formulario.controls.detalles;
  }

  protected abrirModal(propuestaId = 0): void {
    this.formulario.reset({
      propuestaId,
      fechaVencimiento: this.fechaMinima()
    });
    this.obtenerDetalles().clear();
    this.modalAbierto.set(true);
    this.conceptosService.obtenerTodos().subscribe({
      next: conceptos => this.conceptos.set(conceptos),
      error: () => this.toastr.error('No se pudieron cargar los conceptos.')
    });
  }

  protected cerrarModal(): void {
    if (!this.guardando()) {
      this.modalAbierto.set(false);
    }
  }

  protected quitarDetalle(indice: number): void {
    this.obtenerDetalles().removeAt(indice);
  }

  protected guardar(): void {
    if (this.formulario.invalid || !this.obtenerDetalles().length) {
      this.formulario.markAllAsTouched();
      this.toastr.advertencia('Completá la propuesta, el vencimiento y al menos un concepto.');
      return;
    }

    this.guardando.set(true);
    const request = this.formulario.getRawValue();

    this.facturasService.crear(request).subscribe({
      next: () => {
        this.guardando.set(false);
        this.modalAbierto.set(false);
        this.grilla.cargar();
        this.toastr.exito('Factura creada.');
      },
      error: () => this.guardando.set(false)
    });
  }

  private crearDetalle() {
    return this.formularioConstructor.nonNullable.group({
      conceptoId: [0, [Validators.required, Validators.min(1)]],
      cantidad: [1, [Validators.required, Validators.min(0.01)]]
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
    return fechaLocalIso(new Date());
  }
}
