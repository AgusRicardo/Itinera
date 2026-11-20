import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { forkJoin, Subject, of } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';

import { GruposService, GrupoAdministracion } from '../../core/services/grupos.service';
import { UsuarioAdministracion, UsuariosService } from '../../core/services/usuarios.service';
import { RolAdministracion, RolesService } from '../../core/services/roles.service';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';
import {
  BuscadorOpcionesComponent,
  OpcionBuscable
} from '../../shared/components/buscador-opciones/buscador-opciones.component';

@Component({
  selector: 'app-grupos',
  imports: [ReactiveFormsModule, MenuPrincipalComponent, BuscadorOpcionesComponent],
  templateUrl: './grupos.component.html'
})
export class GruposComponent {
  private readonly gruposService = inject(GruposService);
  private readonly usuariosService = inject(UsuariosService);
  private readonly rolesService = inject(RolesService);
  private readonly constructorFormulario = inject(FormBuilder);
  private readonly busquedaUsuarios = new Subject<string>();

  protected readonly grupos = signal<GrupoAdministracion[]>([]);
  protected readonly usuarios = signal<UsuarioAdministracion[]>([]);
  protected readonly roles = signal<RolAdministracion[]>([]);
  protected readonly cargandoOpciones = signal(true);
  protected readonly cargando = signal(true);
  protected readonly modalAbierto = signal(false);
  protected readonly grupoSeleccionado = signal<GrupoAdministracion | null>(null);
  protected readonly guardando = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly aviso = signal('');

  protected readonly formulario = this.constructorFormulario.nonNullable.group({
    nombre: ['', Validators.required],
    miembroId: [''],
    rolId: [0]
  });

  constructor() {
    this.busquedaUsuarios.pipe(
      debounceTime(250),
      distinctUntilChanged(),
      switchMap(texto => texto.length < 3
        ? of([])
        : this.usuariosService.buscar(texto).pipe(catchError(() => of([]))))
    ).subscribe(usuarios => this.usuarios.set(usuarios));

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
    if (this.formulario.controls.nombre.invalid) {
      this.formulario.controls.nombre.markAsTouched();
      return;
    }

    this.guardando.set(true);
    this.gruposService.crear(this.formulario.controls.nombre.value).subscribe({
      next: () => {
        this.guardando.set(false);
        this.cerrarModal();
        this.cargarDatos();
        this.mostrarAviso('Grupo creado correctamente');
      },
      error: respuesta => {
        this.guardando.set(false);
        this.error.set(respuesta.error?.detail ?? 'No se pudo crear el grupo.');
      }
    });
  }

  protected seleccionarGrupo(grupo: GrupoAdministracion): void {
    this.grupoSeleccionado.set(grupo);
    this.formulario.controls.miembroId.reset();
  }

  protected seleccionarMiembro(opcion: OpcionBuscable): void {
    this.formulario.controls.miembroId.setValue(opcion.id);
  }

  protected buscarUsuario(texto: string): void {
    if (texto.length < 3) {
      this.usuarios.set([]);
    }

    this.formulario.controls.miembroId.reset();
    this.busquedaUsuarios.next(texto);
  }

  protected opcionesUsuarios(): OpcionBuscable[] {
    return this.usuarios().map(usuario => ({
      id: usuario.id,
      nombre: `${usuario.nombre} — ${usuario.email}`
    }));
  }

  protected agregarMiembro(): void {
    const grupo = this.grupoSeleccionado();
    const miembroId = this.formulario.controls.miembroId.value;
    if (!grupo || !miembroId) {
      return;
    }

    this.gruposService.agregarMiembro(grupo.id, miembroId).subscribe({
      next: () => {
        this.grupoSeleccionado.set({
          ...grupo,
          miembros: [...grupo.miembros, miembroId]
        });
        this.mostrarAviso('Miembro agregado al grupo');
      },
      error: respuesta => this.error.set(
        respuesta.error?.detail ?? 'No se pudo agregar el miembro.'
      )
    });
  }

  private cargarDatos(): void {
    forkJoin({
      grupos: this.gruposService.obtenerTodos(),
      roles: this.rolesService.obtenerTodos()
    }).subscribe({
      next: datos => {
        this.grupos.set(datos.grupos);
        this.roles.set(datos.roles);
        this.cargandoOpciones.set(false);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set('No se pudieron cargar los grupos.');
        this.cargando.set(false);
        this.cargandoOpciones.set(false);
      }
    });
  }

  protected quitarMiembro(miembroId: string): void {
    const grupo = this.grupoSeleccionado();
    if (!grupo) {
      return;
    }

    this.gruposService.quitarMiembro(grupo.id, miembroId).subscribe({
      next: () => {
        this.grupoSeleccionado.set({
          ...grupo,
          miembros: grupo.miembros.filter(id => id !== miembroId)
        });
        this.mostrarAviso('Miembro quitado del grupo');
      },
      error: respuesta => this.error.set(
        respuesta.error?.detail ?? 'No se pudo quitar el miembro.')
    });
  }

  protected asignarRol(): void {
    const grupo = this.grupoSeleccionado();
    const rolId = this.formulario.controls.rolId.value;
    if (!grupo || !rolId) {
      return;
    }

    const rol = this.roles().find(item => item.id === rolId);
    if (!rol) {
      return;
    }

    this.gruposService.asignarRol(grupo.id, rolId).subscribe({
      next: () => {
        this.grupoSeleccionado.set({
          ...grupo,
          roles: [...new Set([...grupo.roles, rol.nombre])]
        });
        this.formulario.controls.rolId.reset(0);
        this.mostrarAviso('Rol asignado al grupo');
      },
      error: respuesta => this.error.set(
        respuesta.error?.detail ?? 'No se pudo asignar el rol.')
    });
  }

  protected quitarRol(rolNombre: string): void {
    const grupo = this.grupoSeleccionado();
    const rol = this.roles().find(item => item.nombre === rolNombre);
    if (!grupo || !rol) {
      return;
    }

    this.gruposService.quitarRol(grupo.id, rol.id).subscribe({
      next: () => {
        this.grupoSeleccionado.set({
          ...grupo,
          roles: grupo.roles.filter(nombre => nombre !== rolNombre)
        });
        this.mostrarAviso('Rol quitado del grupo');
      },
      error: respuesta => this.error.set(
        respuesta.error?.detail ?? 'No se pudo quitar el rol.')
    });
  }

  private mostrarAviso(mensaje: string): void {
    this.aviso.set(mensaje);
    setTimeout(() => this.aviso.set(''), 2600);
  }
}
