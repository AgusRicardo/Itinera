import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import {
  Empresa,
  EmpresasService
} from '../../core/services/empresas.service';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';

@Component({
  selector: 'app-empresas',
  imports: [
    ReactiveFormsModule,
    MenuPrincipalComponent
  ],
  templateUrl: './empresas.component.html'
})
export class EmpresasComponent {
  private readonly empresasService = inject(EmpresasService);
  private readonly constructorFormulario = inject(FormBuilder);

  protected readonly empresas = signal<Empresa[]>([]);
  protected readonly cargando = signal(true);
  protected readonly guardando = signal(false);
  protected readonly modalAbierto = signal(false);
  protected readonly empresaEditando = signal<Empresa | null>(null);
  protected readonly error = signal<string | null>(null);
  protected readonly aviso = signal('');

  protected readonly formulario = this.constructorFormulario.nonNullable.group({
    razonSocial: ['', Validators.required],
    cuit: ['', Validators.required],
    telefono: [''],
    administradorNombre: ['', Validators.required],
    administradorApellido: ['', Validators.required],
    administradorEmail: ['', [Validators.required, Validators.email]],
    administradorPassword: ['', [Validators.required, Validators.minLength(8)]]
  });

  protected readonly formularioEdicion = this.constructorFormulario.nonNullable.group({
    razonSocial: ['', Validators.required],
    cuit: ['', Validators.required],
    telefono: ['']
  });

  protected readonly empresasActivas = computed(() =>
    this.empresas().filter(empresa => empresa.activo).length
  );

  constructor() {
    this.cargarEmpresas();
  }

  protected abrirModal(): void {
    this.formulario.reset();
    this.error.set(null);
    this.modalAbierto.set(true);
  }

  protected cerrarModal(): void {
    this.modalAbierto.set(false);
  }

  protected abrirEdicion(empresa: Empresa): void {
    this.formularioEdicion.reset({
      razonSocial: empresa.razonSocial,
      cuit: empresa.cuit,
      telefono: empresa.telefono
    });
    this.error.set(null);
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
        this.cargarEmpresas();
        this.aviso.set('Empresa actualizada correctamente');
        setTimeout(() => this.aviso.set(''), 2600);
      },
      error: respuesta => {
        this.guardando.set(false);
        this.error.set(respuesta.error?.detail ?? 'No se pudo actualizar la empresa.');
      }
    });
  }

  protected desactivar(empresa: Empresa): void {
    if (!window.confirm(`¿Querés desactivar ${empresa.razonSocial}?`)) {
      return;
    }

    this.empresasService.desactivar(empresa.id).subscribe({
      next: () => {
        this.cargarEmpresas();
        this.aviso.set('Empresa desactivada');
        setTimeout(() => this.aviso.set(''), 2600);
      },
      error: respuesta => this.error.set(
        respuesta.error?.detail ?? 'No se pudo desactivar la empresa.')
    });
  }

  protected registrar(event?: Event): void {
    event?.preventDefault();

    if (this.formulario.invalid) {
      this.formulario.markAllAsTouched();
      this.error.set(`Completá los campos obligatorios: ${this.camposInvalidos()}.`);
      return;
    }

    this.guardando.set(true);
    this.error.set(null);

    this.empresasService.registrar(this.formulario.getRawValue()).subscribe({
      next: () => {
        this.guardando.set(false);
        this.cerrarModal();
        this.cargarEmpresas();
        this.aviso.set('Empresa creada correctamente');
        setTimeout(() => this.aviso.set(''), 2600);
      },
      error: respuesta => {
        this.guardando.set(false);
        this.error.set(
          respuesta.error?.detail ?? 'No se pudo registrar la empresa.'
        );
      }
    });
  }

  private camposInvalidos(): string {
    const nombres: Record<string, string> = {
      razonSocial: 'razón social',
      cuit: 'CUIT',
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

  private cargarEmpresas(): void {
    this.empresasService.obtenerTodas().subscribe({
      next: empresas => {
        this.empresas.set(empresas);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set('No se pudieron cargar las empresas.');
        this.cargando.set(false);
      }
    });
  }
}
