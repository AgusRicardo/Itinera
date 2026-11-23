import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';

import { Empresa, EmpresasService } from '../../core/services/empresas.service';
import {
  CargoAdministracion,
  RolAdministracion,
  UsuarioAdministracion,
  UsuariosService
} from '../../core/services/usuarios.service';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';
import { AutenticacionService } from '../../core/services/autenticacion.service';

@Component({
  selector: 'app-usuarios',
  imports: [ReactiveFormsModule, MenuPrincipalComponent],
  templateUrl: './usuarios.component.html'
})
export class UsuariosComponent {
  private readonly usuariosService = inject(UsuariosService);
  private readonly empresasService = inject(EmpresasService);
  private readonly constructorFormulario = inject(FormBuilder);
  private readonly autenticacion = inject(AutenticacionService);

  protected readonly usuarios = signal<UsuarioAdministracion[]>([]);
  protected readonly empresas = signal<Empresa[]>([]);
  protected readonly roles = signal<RolAdministracion[]>([]);
  protected readonly cargos = signal<CargoAdministracion[]>([]);
  protected readonly cargando = signal(true);
  protected readonly guardando = signal(false);
  protected readonly modalAbierto = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly aviso = signal('');
  protected readonly busqueda = signal('');
  protected readonly usuarioSeleccionado = signal<UsuarioAdministracion | null>(null);
  protected readonly rolSeleccionado = signal(0);
  protected readonly empresaSeleccionada = signal(0);
  protected readonly cargoSeleccionado = signal(0);

  protected readonly formulario = this.constructorFormulario.nonNullable.group({
    empresaId: [0, [Validators.required, Validators.min(1)]],
    nombre: ['', Validators.required],
    apellido: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    telefono: [''],
    password: ['', [Validators.required, Validators.minLength(8)]],
    cargoId: [0, [Validators.required, Validators.min(1)]],
    rolId: [0, [Validators.required, Validators.min(1)]]
  });

  protected readonly usuariosFiltrados = computed(() => {
    const busqueda = this.busqueda().trim().toLowerCase();
    return this.usuarios().filter(usuario =>
      `${usuario.nombre} ${usuario.email} ${usuario.roles.join(' ')}`
        .toLowerCase()
        .includes(busqueda)
    );
  });

  constructor() {
    this.cargarDatos();
  }

  protected abrirModal(): void {
    this.formulario.reset();
    this.error.set(null);
    this.modalAbierto.set(true);
  }

  protected cerrarModal(): void {
    this.modalAbierto.set(false);
  }

  protected crear(): void {
    if (this.formulario.invalid) {
      this.formulario.markAllAsTouched();
      if (this.formulario.controls.password.hasError('minlength')) {
        this.error.set('La contraseña debe tener al menos 8 caracteres.');
      } else {
        this.error.set('Completá todos los campos obligatorios.');
      }
      return;
    }

    this.guardando.set(true);
    this.error.set(null);

    this.usuariosService.crear(this.formulario.getRawValue()).subscribe({
      next: () => {
        this.guardando.set(false);
        this.cerrarModal();
        this.cargarDatos();
        this.aviso.set('Usuario creado correctamente');
        setTimeout(() => this.aviso.set(''), 2600);
      },
      error: respuesta => {
        this.guardando.set(false);
        this.error.set(respuesta.error?.detail ?? 'No se pudo crear el usuario.');
      }
    });
  }

  protected seleccionarUsuario(usuario: UsuarioAdministracion): void {
    this.usuarioSeleccionado.set(usuario);
    this.rolSeleccionado.set(0);
    this.empresaSeleccionada.set(usuario.empresaId ?? 0);
    this.cargoSeleccionado.set(0);
  }

  protected cerrarAcciones(): void {
    this.usuarioSeleccionado.set(null);
  }

  protected cambiarEstado(usuario: UsuarioAdministracion): void {
    if (this.esUsuarioActual(usuario)) {
      this.error.set('No podés desactivar tu propio usuario.');
      return;
    }

    this.usuariosService.cambiarEstado(usuario.id, !usuario.activo).subscribe({
      next: () => {
        this.usuarios.update(usuarios =>
          usuarios.map(item => item.id === usuario.id
            ? { ...item, activo: !item.activo }
            : item)
        );
        this.mostrarAviso(usuario.activo ? 'Usuario desactivado' : 'Usuario activado');
      },
      error: respuesta => this.error.set(
        respuesta.error?.detail ?? 'No se pudo cambiar el estado del usuario.')
    });
  }

  protected esUsuarioActual(usuario: UsuarioAdministracion): boolean {
    return this.autenticacion.obtenerUsuario()?.usuarioId === usuario.id;
  }

  protected asignarRol(): void {
    const usuario = this.usuarioSeleccionado();
    const rolId = this.rolSeleccionado();
    if (!usuario || !rolId) {
      return;
    }

    const rol = this.roles().find(item => item.id === rolId);
    if (!rol) {
      return;
    }

    this.usuariosService.asignarRol(usuario.id, rolId).subscribe({
      next: () => {
        this.usuarios.update(usuarios =>
          usuarios.map(item => item.id === usuario.id
            ? { ...item, roles: [...new Set([...item.roles, rol.nombre])] }
            : item)
        );
        this.cerrarAcciones();
        this.mostrarAviso('Rol asignado correctamente');
      },
      error: respuesta => this.error.set(
        respuesta.error?.detail ?? 'No se pudo asignar el rol.')
    });
  }

  protected quitarRol(usuario: UsuarioAdministracion, nombreRol: string): void {
    const rol = this.roles().find(item => item.nombre === nombreRol);
    if (!rol) {
      return;
    }

    this.usuariosService.quitarRol(usuario.id, rol.id).subscribe({
      next: () => {
        this.usuarios.update(usuarios =>
          usuarios.map(item => item.id === usuario.id
            ? { ...item, roles: item.roles.filter(nombre => nombre !== nombreRol) }
            : item)
        );
        this.usuarioSeleccionado.update(actual =>
          actual && actual.id === usuario.id
            ? { ...actual, roles: actual.roles.filter(nombre => nombre !== nombreRol) }
            : actual
        );
        this.mostrarAviso('Rol quitado correctamente');
      },
      error: respuesta => this.error.set(
        respuesta.error?.detail ?? 'No se pudo quitar el rol.')
    });
  }

  protected asignarEmpresa(): void {
    const usuario = this.usuarioSeleccionado();
    const empresaId = this.empresaSeleccionada();
    const cargoId = this.cargoSeleccionado();
    if (!usuario || !empresaId || !cargoId) {
      return;
    }

    this.usuariosService.asignarEmpresa(usuario.id, empresaId, cargoId).subscribe({
      next: () => {
        const empresa = this.empresas().find(item => item.id === empresaId);
        this.usuarios.update(usuarios =>
          usuarios.map(item => item.id === usuario.id
            ? { ...item, empresaId, empresaRazonSocial: empresa?.razonSocial }
            : item)
        );
        this.usuarioSeleccionado.update(actual =>
          actual && actual.id === usuario.id
            ? { ...actual, empresaId, empresaRazonSocial: empresa?.razonSocial }
            : actual
        );
        this.mostrarAviso('Empresa asignada correctamente');
      },
      error: respuesta => this.error.set(
        respuesta.error?.detail ?? 'No se pudo asignar la empresa.')
    });
  }

  private cargarDatos(): void {
    forkJoin({
      usuarios: this.usuariosService.obtenerTodos(),
      empresas: this.empresasService.obtenerTodas(),
      roles: this.usuariosService.obtenerRoles(),
      cargos: this.usuariosService.obtenerCargos()
    }).subscribe({
      next: datos => {
        this.usuarios.set(datos.usuarios);
        this.empresas.set(datos.empresas);
        this.roles.set(datos.roles.filter(rol => rol.nombre !== 'AdministradorItinera'));
        this.cargos.set(datos.cargos.filter(cargo => cargo.activo));
        this.cargando.set(false);
      },
      error: () => {
        this.error.set('No se pudieron cargar los datos de administración.');
        this.cargando.set(false);
      }
    });
  }

  private mostrarAviso(mensaje: string): void {
    this.aviso.set(mensaje);
    setTimeout(() => this.aviso.set(''), 2600);
  }
}
