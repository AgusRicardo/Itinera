import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import {
  Empresa,
  EmpresasService
} from '../../core/services/empresas.service';
import { AccionMenu, MenuAccionesComponent } from '../../shared/components/menu-acciones/menu-acciones.component';
import { EstadoComponent } from '../../shared/components/estado/estado.component';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';
import { ConfirmacionService } from '../../core/services/confirmacion.service';
import { ToastrService } from '../../core/services/toastr.service';
import { EncabezadoComponent } from '../../shared/components/encabezado/encabezado.component';
import { FiltrosGrillaComponent } from '../../shared/components/filtros-grilla/filtros-grilla.component';
import { TablaComponent } from '../../shared/components/tabla/tabla.component';
import { ColumnaOrdenableComponent } from '../../shared/components/columna-ordenable/columna-ordenable.component';
import { FiltrosGrilla } from '../../core/models/pagina';
import { GrillaPaginada } from '../../core/utils/grilla-paginada';
import { cuitValido, telefonoValido, textoRequerido } from '../../core/utils/validadores';

@Component({
  selector: 'app-empresas',
  imports: [
    ReactiveFormsModule,
    MenuAccionesComponent,
    EstadoComponent,
    MenuPrincipalComponent,
    EncabezadoComponent,
    FiltrosGrillaComponent,
    TablaComponent,
    ColumnaOrdenableComponent
  ],
  templateUrl: './empresas.component.html'
})
export class EmpresasComponent {
  private readonly confirmacion = inject(ConfirmacionService);
  private readonly toastr = inject(ToastrService);
  private readonly empresasService = inject(EmpresasService);
  private readonly constructorFormulario = inject(FormBuilder);

  protected readonly grilla = new GrillaPaginada<Empresa>(
    consulta => this.empresasService.obtenerPagina(consulta),
    'No se pudieron cargar las empresas.');
  protected readonly guardando = signal(false);
  protected readonly modalAbierto = signal(false);
  protected readonly empresaEditando = signal<Empresa | null>(null);

  protected readonly formulario = this.constructorFormulario.nonNullable.group({
    razonSocial: ['', textoRequerido],
    cuit: ['', [textoRequerido, cuitValido]],
    telefono: ['', telefonoValido],
    administradorNombre: ['', textoRequerido],
    administradorApellido: ['', textoRequerido],
    administradorEmail: ['', [Validators.required, Validators.email]],
    administradorPassword: ['', [Validators.required, Validators.minLength(8)]]
  });

  protected readonly formularioEdicion = this.constructorFormulario.nonNullable.group({
    razonSocial: ['', textoRequerido],
    cuit: ['', [textoRequerido, cuitValido]],
    telefono: ['', telefonoValido]
  });

  protected readonly estados = [
    { id: 1, descripcion: 'Activa' },
    { id: 2, descripcion: 'Inactiva' }
  ];

  protected filtrar(filtros: FiltrosGrilla): void {
    const estados = filtros['estadoIds'] as number[];

    this.grilla.filtrar({
      texto: filtros['texto'],
      activo: estados.length === 1 ? estados[0] === 1 : ''
    });
  }

  protected abrirModal(): void {
    this.formulario.reset();
    this.modalAbierto.set(true);
  }

  protected cerrarModal(): void {
    this.modalAbierto.set(false);
  }

  protected acciones(empresa: Empresa): AccionMenu[] {
    const acciones: AccionMenu[] = [{ texto: 'Editar', ejecutar: () => this.abrirEdicion(empresa) }];

    if (empresa.activo) {
      acciones.push({ texto: 'Desactivar', peligro: true, ejecutar: () => this.desactivar(empresa) });
    }

    return acciones;
  }

  protected abrirEdicion(empresa: Empresa): void {
    this.formularioEdicion.reset({
      razonSocial: empresa.razonSocial,
      cuit: empresa.cuit,
      telefono: empresa.telefono
    });
    this.empresaEditando.set(empresa);
  }

  protected cerrarEdicion(): void {
    this.empresaEditando.set(null);
  }

  protected actualizar(): void {
    const empresa = this.empresaEditando();
    if (!empresa || this.formularioEdicion.invalid) {
      this.formularioEdicion.markAllAsTouched();
      return;
    }

    this.guardando.set(true);
    this.empresasService.actualizar(empresa.id, this.formularioEdicion.getRawValue()).subscribe({
      next: () => {
        this.guardando.set(false);
        this.cerrarEdicion();
        this.grilla.cargar();
        this.toastr.exito('Empresa actualizada.');
      },
      error: () => this.guardando.set(false)
    });
  }

  private async desactivar(empresa: Empresa): Promise<void> {
    const confirmado = await this.confirmacion.confirmar({
      titulo: `¿Desactivar ${empresa.razonSocial}?`,
      mensaje: 'Va a seguir en la grilla como inactiva, pero no se podrá elegir para usuarios nuevos.',
      accion: 'Desactivar',
      peligro: true
    });

    if (!confirmado) {
      return;
    }

    this.empresasService.desactivar(empresa.id).subscribe(() => {
      this.grilla.cargar();
      this.toastr.exito('Empresa desactivada.');
    });
  }

  protected registrar(event?: Event): void {
    event?.preventDefault();

    if (this.formulario.invalid) {
      this.formulario.markAllAsTouched();
      this.toastr.advertencia(`Completá los campos obligatorios: ${this.camposInvalidos()}.`);
      return;
    }

    this.guardando.set(true);

    this.empresasService.registrar(this.formulario.getRawValue()).subscribe({
      next: () => {
        this.guardando.set(false);
        this.cerrarModal();
        this.grilla.cargar();
        this.toastr.exito('Empresa creada.');
      },
      error: () => this.guardando.set(false)
    });
  }

  private camposInvalidos(): string {
    const nombres: Record<string, string> = {
      razonSocial: 'razón social',
      cuit: 'CUIT',
      telefono: 'teléfono',
      administradorNombre: 'nombre del administrador',
      administradorApellido: 'apellido del administrador',
      administradorEmail: 'email del administrador',
      administradorPassword: 'contraseña del administrador'
    };

    return Object.entries(this.formulario.controls)
      .filter(([, control]) => control.invalid)
      .map(([nombre]) => nombres[nombre] ?? nombre)
      .join(', ');
  }
}
