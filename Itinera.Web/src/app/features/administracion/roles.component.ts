import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import {
  PermisoAdministracion,
  RolAdministracion,
  RolesService
} from '../../core/services/roles.service';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';
import { ToastrService } from '../../core/services/toastr.service';
import { EncabezadoComponent } from '../../shared/components/encabezado/encabezado.component';
import { CasillaComponent } from '../../shared/components/casilla/casilla.component';

@Component({
  selector: 'app-roles',
  imports: [
    ReactiveFormsModule,
    MenuPrincipalComponent,
    EncabezadoComponent,
    CasillaComponent
  ],
  templateUrl: './roles.component.html'
})
export class RolesComponent {
  private readonly toastr = inject(ToastrService);
  private readonly rolesService = inject(RolesService);
  private readonly constructorFormulario = inject(FormBuilder);

  protected readonly roles = signal<RolAdministracion[]>([]);
  protected readonly permisos = signal<PermisoAdministracion[]>([]);
  protected readonly cargando = signal(true);
  protected readonly modalAbierto = signal(false);
  protected readonly guardando = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly permisosSeleccionados = signal<string[]>([]);

  protected readonly formulario = this.constructorFormulario.nonNullable.group({
    nombre: ['', Validators.required],
    descripcion: ['', Validators.required]
  });

  protected readonly permisosAgrupados = computed(() => {
    const grupos = new Map<string, PermisoAdministracion[]>();
    for (const permiso of this.permisos()) {
      const modulo = permiso.codigo.split('.')[0];
      grupos.set(modulo, [...(grupos.get(modulo) ?? []), permiso]);
    }
    return Array.from(grupos.entries());
  });

  constructor() {
    this.cargarDatos();
  }

  protected abrirModal(): void {
    this.formulario.reset();
    this.permisosSeleccionados.set([]);
    this.modalAbierto.set(true);
  }

  protected cerrarModal(): void {
    this.modalAbierto.set(false);
  }

  protected alternarPermiso(codigo: string): void {
    this.permisosSeleccionados.update(permisos =>
      permisos.includes(codigo)
        ? permisos.filter(permiso => permiso !== codigo)
        : [...permisos, codigo]
    );
  }

  protected tienePermiso(codigo: string): boolean {
    return this.permisosSeleccionados().includes(codigo);
  }

  protected crear(): void {
    if (this.formulario.invalid) {
      this.formulario.markAllAsTouched();
      return;
    }

    this.guardando.set(true);

    this.rolesService.crear({
      ...this.formulario.getRawValue(),
      permisos: this.permisosSeleccionados()
    }).subscribe({
      next: () => {
        this.guardando.set(false);
        this.cerrarModal();
        this.cargarDatos();
        this.toastr.exito('Rol creado.');
      },
      error: () => this.guardando.set(false)
    });
  }

  private cargarDatos(): void {
    this.rolesService.obtenerTodos().subscribe({
      next: roles => {
        this.roles.set(roles);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set('No se pudieron cargar los roles.');
        this.cargando.set(false);
      }
    });

    this.rolesService.obtenerPermisos().subscribe({
      next: permisos => this.permisos.set(permisos),
      error: () => this.error.set('No se pudieron cargar los permisos.')
    });
  }
}
