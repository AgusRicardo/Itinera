import { Component, computed, inject, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { ClientesService } from '../../core/services/clientes.service';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';
import { AutenticacionService } from '../../core/services/autenticacion.service';

interface ClienteVista {
  id: number;
  nombre: string;
  nombrePrincipal: string;
  apellido: string;
  email: string;
  telefono: string;
  ciudad: string;
  propuestas?: number;
  ultimaGestion?: string;
  estado: 'Activo' | 'Inactivo';
}

@Component({
  selector: 'app-clientes',
  imports: [
    ReactiveFormsModule,
    MenuPrincipalComponent
  ],
  templateUrl: './clientes.component.html'
})
export class ClientesComponent {
  private readonly clientesService = inject(ClientesService);
  private readonly constructorFormulario = inject(FormBuilder);
  private readonly autenticacion = inject(AutenticacionService);

  protected readonly clientes = signal<ClienteVista[]>([]);
  protected readonly cargando = signal(true);
  protected readonly error = signal<string | null>(null);
  protected readonly busqueda = signal('');
  protected readonly estado = signal('');
  protected readonly editor = signal<ClienteVista | 'nuevo' | null>(null);
  protected readonly clienteAEliminar = signal<ClienteVista | null>(null);
  protected readonly aviso = signal('');
  protected readonly guardando = signal(false);

  protected readonly formulario = this.constructorFormulario.nonNullable.group({
    nombre: ['', Validators.required],
    apellido: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    telefono: ['', Validators.required],
    ciudad: [''],
    estado: ['Activo' as 'Activo' | 'Inactivo']
  });

  protected readonly clientesFiltrados = computed(() => {
    const texto = this.busqueda().toLowerCase().trim();
    const estado = this.estado();

    return this.clientes().filter(cliente => {
      const contenido = `${cliente.nombre} ${cliente.email} ${cliente.telefono} ${cliente.ciudad}`.toLowerCase();
      return contenido.includes(texto) && (!estado || cliente.estado === estado);
    });
  });

  protected readonly cantidadActivos = computed(() =>
    this.clientes().filter(cliente => cliente.estado === 'Activo').length
  );

  protected puedeGestionarClientes(): boolean {
    return this.autenticacion.tienePermiso('clientes.gestionar');
  }

  protected campoInvalido(nombre: string): boolean {
    const control = this.formulario.get(nombre);
    return Boolean(control?.touched && control.invalid);
  }

  constructor() {
    this.cargarClientes();
  }

  protected abrirNuevo(): void {
    this.formulario.reset({
      nombre: '',
      apellido: '',
      email: '',
      telefono: '',
      ciudad: '',
      estado: 'Activo'
    });
    this.editor.set('nuevo');
  }

  protected abrirEdicion(cliente: ClienteVista): void {
    this.formulario.reset({
      nombre: cliente.nombrePrincipal,
      apellido: cliente.apellido,
      email: cliente.email,
      telefono: cliente.telefono,
      ciudad: cliente.ciudad,
      estado: cliente.estado
    });
    this.editor.set(cliente);
  }

  protected cerrarEditor(): void {
    this.editor.set(null);
  }

  protected guardar(): void {
    if (this.formulario.invalid) {
      this.formulario.markAllAsTouched();
      return;
    }

    const actual = this.editor();
    if (!actual) {
      return;
    }

    this.guardando.set(true);
    const datos = this.formulario.getRawValue();
    const datosApi = {
      nombre: datos.nombre.trim(),
      apellido: datos.apellido.trim(),
      email: datos.email,
      telefono: datos.telefono,
      activo: datos.estado === 'Activo'
    };
    const peticion = actual === 'nuevo'
      ? this.clientesService.crear(datosApi)
      : this.clientesService.actualizar(actual.id, datosApi);

    peticion.subscribe({
      next: () => {
        this.guardando.set(false);
        this.cerrarEditor();
        this.cargarClientes();
        this.mostrarAviso(actual === 'nuevo' ? 'Cliente creado correctamente' : 'Cliente actualizado');
      },
      error: (error: HttpErrorResponse) => {
        this.guardando.set(false);
        this.error.set(
          error.error?.detail
          ?? error.error?.message
          ?? 'No se pudo guardar el cliente.'
        );
      }
    });
  }

  protected confirmarEliminacion(cliente: ClienteVista): void {
    this.clienteAEliminar.set(cliente);
  }

  protected cancelarEliminacion(): void {
    this.clienteAEliminar.set(null);
  }

  protected eliminar(): void {
    const cliente = this.clienteAEliminar();
    if (!cliente) {
      return;
    }

    this.clientesService.desactivar(cliente.id).subscribe({
      next: () => {
        this.cargarClientes();
        this.clienteAEliminar.set(null);
        this.mostrarAviso('Cliente desactivado');
      },
      error: () => this.error.set('No se pudo desactivar el cliente.')
    });
  }

  protected iniciales(nombre: string): string {
    return nombre
      .split(' ')
      .map(parte => parte[0])
      .slice(0, 2)
      .join('');
  }

  private cargarClientes(): void {
    this.clientesService.obtenerTodos().subscribe({
      next: clientes => {
        this.clientes.set(clientes.map(cliente => this.convertirCliente(cliente)));
        this.cargando.set(false);
      },
      error: () => {
        this.error.set('No se pudieron cargar los clientes.');
        this.cargando.set(false);
      }
    });
  }

  private convertirCliente(cliente: {
    id: number;
    nombre: string;
    apellido?: string;
    email: string;
    telefono: string;
    activo?: boolean;
  }): ClienteVista {
    return {
      id: cliente.id,
      nombre: `${cliente.nombre} ${cliente.apellido ?? ''}`.trim(),
      nombrePrincipal: cliente.nombre,
      apellido: cliente.apellido ?? '',
      email: cliente.email,
      telefono: cliente.telefono,
      ciudad: '—',
      estado: cliente.activo === false ? 'Inactivo' : 'Activo'
    };
  }

  private mostrarAviso(mensaje: string): void {
    this.aviso.set(mensaje);
    setTimeout(() => this.aviso.set(''), 2600);
  }
}
