import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { AutenticacionService } from '../../core/services/autenticacion.service';
import { Concepto, ConceptosService } from '../../core/services/conceptos.service';
import { ToastrService } from '../../core/services/toastr.service';
import { GrillaPaginada } from '../../core/utils/grilla-paginada';
import { ColumnaOrdenableComponent } from '../../shared/components/columna-ordenable/columna-ordenable.component';
import { InputMonedaComponent } from '../../shared/components/input-moneda/input-moneda.component';
import { AccionMenu, MenuAccionesComponent } from '../../shared/components/menu-acciones/menu-acciones.component';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';
import { MonedaPipe } from '../../shared/pipes/moneda.pipe';
import { ConfirmacionService } from '../../core/services/confirmacion.service';
import { EncabezadoComponent } from '../../shared/components/encabezado/encabezado.component';
import { FiltrosGrillaComponent } from '../../shared/components/filtros-grilla/filtros-grilla.component';
import { TablaComponent } from '../../shared/components/tabla/tabla.component';

@Component({
  selector: 'app-conceptos',
  imports: [
    ReactiveFormsModule,
    ColumnaOrdenableComponent,
    InputMonedaComponent,
    MenuAccionesComponent,
    MenuPrincipalComponent,
    MonedaPipe,
    EncabezadoComponent,
    FiltrosGrillaComponent,
    TablaComponent
  ],
  templateUrl: './conceptos.component.html'
})
export class ConceptosComponent {
  private readonly conceptosService = inject(ConceptosService);
  private readonly autenticacion = inject(AutenticacionService);
  private readonly confirmacion = inject(ConfirmacionService);
  private readonly toastr = inject(ToastrService);
  private readonly formularioConstructor = inject(FormBuilder);

  protected readonly grilla = new GrillaPaginada<Concepto>(
    consulta => this.conceptosService.obtenerPagina(consulta),
    'No se pudieron cargar los conceptos.');
  protected readonly editando = signal<Concepto | null>(null);
  protected readonly modalAbierto = signal(false);
  protected readonly guardando = signal(false);
  protected readonly puedeGestionar = this.autenticacion.tienePermiso('conceptos.gestionar');

  protected readonly formulario = this.formularioConstructor.nonNullable.group({
    descripcion: ['', [Validators.required, Validators.maxLength(300)]],
    esPorcentaje: [false],
    precio: [0, [Validators.required, Validators.min(0)]],
    porcentaje: [10, [Validators.required, Validators.min(0.01), Validators.max(100)]]
  });

  protected acciones(concepto: Concepto): AccionMenu[] {
    return [
      { texto: 'Editar', ejecutar: () => this.abrir(concepto) },
      { texto: 'Eliminar', peligro: true, ejecutar: () => this.eliminar(concepto) }
    ];
  }

  protected abrir(concepto?: Concepto): void {
    this.editando.set(concepto ?? null);
    this.formulario.reset({
      descripcion: concepto?.descripcion ?? '',
      esPorcentaje: concepto?.porcentaje != null,
      precio: concepto?.precio ?? 0,
      porcentaje: concepto?.porcentaje ?? 10
    });
    this.modalAbierto.set(true);
  }

  protected cerrar(): void {
    if (!this.guardando()) {
      this.modalAbierto.set(false);
    }
  }

  protected guardar(): void {
    const { descripcion, esPorcentaje, precio, porcentaje } = this.formulario.getRawValue();
    if (this.formulario.invalid || !descripcion.trim()) {
      this.formulario.markAllAsTouched();
      return;
    }

    this.guardando.set(true);
    const concepto = this.editando();
    const request = {
      descripcion: descripcion.trim(),
      precio: esPorcentaje ? 0 : precio,
      porcentaje: esPorcentaje ? porcentaje : null
    };
    const guardado$ = concepto
      ? this.conceptosService.actualizar(concepto.id, request)
      : this.conceptosService.crear(request);

    guardado$.subscribe({
      next: () => {
        this.guardando.set(false);
        this.modalAbierto.set(false);
        this.grilla.cargar();
        this.toastr.exito(concepto ? 'Concepto actualizado.' : 'Concepto creado.');
      },
      error: () => this.guardando.set(false)
    });
  }

  private async eliminar(concepto: Concepto): Promise<void> {
    const confirmado = await this.confirmacion.confirmar({
      titulo: `¿Eliminar el concepto ${concepto.descripcion}?`,
      mensaje: 'Las facturas ya creadas no cambian.',
      accion: 'Eliminar',
      peligro: true
    });

    if (!confirmado) {
      return;
    }

    this.conceptosService.eliminar(concepto.id).subscribe({
      next: () => {
        this.grilla.cargar();
        this.toastr.exito('Concepto eliminado.');
      }
    });
  }
}
