import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';

import { Empresa, EmpresasService } from '../../core/services/empresas.service';
import {
  CargoAdministracion,
  RolAdministracion,
  UsuarioAdministracion,
  UsuariosService
} from '../../core/services/usuarios.service';
import { AccionMenu, MenuAccionesComponent } from '../../shared/components/menu-acciones/menu-acciones.component';
import { EstadoComponent } from '../../shared/components/estado/estado.component';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';
import { AutenticacionService } from '../../core/services/autenticacion.service';
import { ConfirmacionService } from '../../core/services/confirmacion.service';
import { ToastrService } from '../../core/services/toastr.service';
import { EncabezadoComponent } from '../../shared/components/encabezado/encabezado.component';
import { FiltrosGrillaComponent } from '../../shared/components/filtros-grilla/filtros-grilla.component';
import { TablaComponent } from '../../shared/components/tabla/tabla.component';
import { ColumnaOrdenableComponent } from '../../shared/components/columna-ordenable/columna-ordenable.component';
import { OpcionSelector, SelectorComponent } from '../../shared/components/selector/selector.component';
import { FiltrosGrilla } from '../../core/models/pagina';
import { GrillaPaginada } from '../../core/utils/grilla-paginada';
import { telefonoValido, textoRequerido } from '../../core/utils/validadores';

@Component({
  selector: 'app-usuarios',
  imports: [
    ReactiveFormsModule,
    MenuAccionesComponent,
    EstadoComponent,
    MenuPrincipalComponent,
    EncabezadoComponent,
    FiltrosGrillaComponent,
    TablaComponent,
    ColumnaOrdenableComponent,
    SelectorComponent
  ],
  templateUrl: './usuarios.component.html'
})
export class UsuariosComponent {
  private readonly confirmacion = inject(ConfirmacionService);
  private readonly toastr = inject(ToastrService);
  private readonly usuariosService = inject(UsuariosService);
  private readonly empresasService = inject(EmpresasService);
  private readonly constructorFormulario = inject(FormBuilder);
  private readonly autenticacion = inject(AutenticacionService);

  protected readonly grilla = new GrillaPaginada<UsuarioAdministracion>(
    consulta => this.usuariosService.obtenerPagina(consulta),
    'No se pudieron cargar los usuarios.');
  protected readonly empresas = signal<Empresa[]>([]);
  protected readonly roles = signal<RolAdministracion[]>([]);
  protected readonly cargos = signal<CargoAdministracion[]>([]);
  protected readonly guardando = signal(false);
  protected readonly modalAbierto = signal(false);
  protected readonly usuarioSeleccionado = signal<UsuarioAdministracion | null>(null);
  protected readonly rolElegido = new FormControl(0, { nonNullable: true });
  protected readonly empresaElegida = new FormControl(0, { nonNullable: true });
  protected readonly cargoElegido = new FormControl(0, { nonNullable: true });

  protected readonly estados = [
    { id: 1, descripcion: 'Activo' },
    { id: 2, descripcion: 'Inactivo' }
  ];

  protected readonly opcionesEmpresas = computed<OpcionSelector[]>(() =>
    this.empresas().map(empresa => ({ id: empresa.id, nombre: empresa.razonSocial })));

  protected readonly opcionesCargos = computed<OpcionSelector[]>(() =>
    this.cargos().map(cargo => ({ id: cargo.id, nombre: cargo.descripcion })));

  protected readonly formulario = this.constructorFormulario.nonNullable.group({
    empresaId: [0, [Validators.required, Validators.min(1)]],
    nombre: ['', textoRequerido],
    apellido: ['', textoRequerido],
    email: ['', [Validators.required, Validators.email]],
    telefono: ['', telefonoValido],
    password: ['', [Validators.required, Validators.minLength(8)]],
    cargoId: [0, [Validators.required, Validators.min(1)]],
    rolId: [0, [Validators.required, Validators.min(1)]]
  });

  protected filtrar(filtros: FiltrosGrilla): void {
    const estados = filtros['estadoIds'] as number[];

    this.grilla.filtrar({
      texto: filtros['texto'],
      activo: estados.length === 1 ? estados[0] === 1 : ''
    });
  }

  constructor() {
    this.cargarDatos();
  }

  protected abrirModal(): void {
    this.formulario.reset();
    this.modalAbierto.set(true);
  }

  protected cerrarModal(): void {
    this.modalAbierto.set(false);
  }

  protected crear(): void {
    if (this.formulario.invalid) {
      this.formulario.markAllAsTouched();
      if (this.formulario.controls.password.hasError('minlength')) {
        this.toastr.advertencia('La contraseña debe tener al menos 8 caracteres.');
      } else {
        this.toastr.advertencia('Completá todos los campos obligatorios.');
      }
      return;
    }

    this.guardando.set(true);

    this.usuariosService.crear(this.formulario.getRawValue()).subscribe({
      next: () => {
        this.guardando.set(false);
        this.cerrarModal();
        this.grilla.cargar();
        this.toastr.exito('Usuario creado.');
      },
      error: () => this.guardando.set(false)
    });
  }

  protected acciones(usuario: UsuarioAdministracion): AccionMenu[] {
    return [
      { texto: 'Roles y empresa', ejecutar: () => this.seleccionarUsuario(usuario) },
      {
        texto: usuario.activo ? 'Desactivar' : 'Activar',
        peligro: usuario.activo,
        deshabilitada: this.esUsuarioActual(usuario),
        ejecutar: () => this.cambiarEstado(usuario)
      }
    ];
  }

  protected seleccionarUsuario(usuario: UsuarioAdministracion): void {
    this.usuarioSeleccionado.set(usuario);
    this.rolElegido.setValue(0);
    this.empresaElegida.setValue(usuario.empresaId ?? 0);
    this.cargoElegido.setValue(usuario.cargoId ?? 0);
  }

  protected cerrarAcciones(): void {
    this.usuarioSeleccionado.set(null);
  }

  private async cambiarEstado(usuario: UsuarioAdministracion): Promise<void> {
    const confirmado = !usuario.activo || await this.confirmacion.confirmar({
      titulo: `¿Desactivar a ${usuario.nombre}?`,
      mensaje: 'No va a poder iniciar sesión.',
      accion: 'Desactivar',
      peligro: true
    });

    if (!confirmado) {
      return;
    }

    this.usuariosService.cambiarEstado(usuario.id, !usuario.activo).subscribe(() => {
      this.grilla.cargar();
      this.toastr.exito(usuario.activo ? 'Usuario desactivado.' : 'Usuario activado.');
    });
  }

  protected esUsuarioActual(usuario: UsuarioAdministracion): boolean {
    return this.autenticacion.obtenerUsuario()?.usuarioId === usuario.id;
  }

  protected asignarRol(): void {
    const usuario = this.usuarioSeleccionado();
    const rolId = this.rolElegido.value;
    if (!usuario || !rolId) {
      return;
    }

    const rol = this.roles().find(item => item.id === rolId);
    if (!rol) {
      return;
    }

    this.usuariosService.asignarRol(usuario.id, rolId).subscribe({
      next: () => {
        this.grilla.cargar();
        this.cerrarAcciones();
        this.toastr.exito('Rol asignado.');
      }
    });
  }

  protected quitarRol(usuario: UsuarioAdministracion, nombreRol: string): void {
    const rol = this.roles().find(item => item.nombre === nombreRol);
    if (!rol) {
      return;
    }

    this.usuariosService.quitarRol(usuario.id, rol.id).subscribe({
      next: () => {
        this.grilla.cargar();
        this.toastr.exito('Rol quitado.');
      }
    });
  }

  protected asignarEmpresa(): void {
    const usuario = this.usuarioSeleccionado();
    const empresaId = this.empresaElegida.value;
    const cargoId = this.cargoElegido.value;
    if (!usuario || !empresaId || !cargoId) {
      return;
    }

    this.usuariosService.asignarEmpresa(usuario.id, empresaId, cargoId).subscribe({
      next: () => {
        const empresa = this.empresas().find(item => item.id === empresaId);
        this.grilla.cargar();
        this.usuarioSeleccionado.update(actual =>
          actual && actual.id === usuario.id
            ? { ...actual, empresaId, cargoId, empresaActiva: true, empresaRazonSocial: empresa?.razonSocial }
            : actual
        );
        this.toastr.exito('Empresa asignada.');
      }
    });
  }

  private cargarDatos(): void {
    forkJoin({
      empresas: this.empresasService.obtenerTodas(),
      roles: this.usuariosService.obtenerRoles(),
      cargos: this.usuariosService.obtenerCargos()
    }).subscribe({
      next: datos => {
        this.empresas.set(datos.empresas.filter(empresa => empresa.activo));
        this.roles.set(datos.roles.filter(rol => rol.nombre !== 'AdministradorItinera'));
        this.cargos.set(datos.cargos.filter(cargo => cargo.activo));
      },
      error: () => this.toastr.error('No se pudieron cargar los datos de administración.')
    });
  }
}
