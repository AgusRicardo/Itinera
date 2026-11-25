import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
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
import { ToastrService } from '../../core/services/toastr.service';
import { textoRequerido } from '../../core/utils/validadores';
import { EncabezadoComponent } from '../../shared/components/encabezado/encabezado.component';
import { SelectorComponent } from '../../shared/components/selector/selector.component';

@Component({
  selector: 'app-grupos',
  imports: [
    ReactiveFormsModule,
    MenuPrincipalComponent,
    BuscadorOpcionesComponent,
    EncabezadoComponent,
    SelectorComponent
  ],
  templateUrl: './grupos.component.html'
})
export class GruposComponent {
  private readonly toastr = inject(ToastrService);
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

  protected readonly formulario = this.constructorFormulario.nonNullable.group({
    nombre: ['', textoRequerido],
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
        this.toastr.exito('Grupo creado correctamente');
      },
      error: () => this.guardando.set(false)
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

  protected yaEsMiembro(): boolean {
    const miembroId = this.formulario.controls.miembroId.value;

    return Boolean(this.grupoSeleccionado()?.miembros.some(miembro => miembro.id === miembroId));
  }

  private actualizarGrupo(grupo: GrupoAdministracion): void {
    this.grupoSeleccionado.set(grupo);
    this.grupos.update(grupos => grupos.map(item => item.id === grupo.id ? grupo : item));
  }

  protected agregarMiembro(): void {
    const grupo = this.grupoSeleccionado();
    const miembroId = this.formulario.controls.miembroId.value;
    if (!grupo || !miembroId || this.yaEsMiembro()) {
      return;
    }

    this.gruposService.agregarMiembro(grupo.id, miembroId).subscribe({
      next: () => {
        const nombre = this.usuarios().find(usuario => usuario.id === miembroId)?.nombre ?? '';
        this.actualizarGrupo({
          ...grupo,
          miembros: [...grupo.miembros, { id: miembroId, nombre }]
        });
        this.formulario.controls.miembroId.reset();
        this.toastr.exito('Miembro agregado al grupo');
      }
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
        this.actualizarGrupo({
          ...grupo,
          miembros: grupo.miembros.filter(miembro => miembro.id !== miembroId)
        });
        this.toastr.exito('Miembro quitado del grupo');
      }
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
        this.actualizarGrupo({
          ...grupo,
          roles: [...new Set([...grupo.roles, rol.nombre])]
        });
        this.formulario.controls.rolId.reset(0);
        this.toastr.exito('Rol asignado al grupo');
      }
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
        this.actualizarGrupo({
          ...grupo,
          roles: grupo.roles.filter(nombre => nombre !== rolNombre)
        });
        this.toastr.exito('Rol quitado del grupo');
      }
    });
  }
}
