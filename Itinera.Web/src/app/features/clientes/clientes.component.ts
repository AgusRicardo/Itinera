import { Component, inject, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { map } from 'rxjs';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { ClienteApi, ClientesService, ResumenClientes } from '../../core/services/clientes.service';
import { GrillaPaginada } from '../../core/utils/grilla-paginada';
import { PaginadorComponent } from '../../shared/components/paginador/paginador.component';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';
import { AutenticacionService } from '../../core/services/autenticacion.service';

interface ClienteVista {
  id: number;
  nombre: string;
  nombrePrincipal: string;
  apellido: string;
  email: string;
  telefono: string;
  propuestas?: number;
  ultimaGestion?: string;
  estado: 'Activo' | 'Inactivo';
}

@Component({
  selector: 'app-clientes',
  imports: [
    ReactiveFormsModule,
    MenuPrincipalComponent,
    PaginadorComponent
  ],
  templateUrl: './clientes.component.html'
})
export class ClientesComponent {
  private readonly clientesService = inject(ClientesService);
  private readonly constructorFormulario = inject(FormBuilder);
  private readonly autenticacion = inject(AutenticacionService);

  protected readonly grilla = new GrillaPaginada<ClienteVista>(
    consulta => this.clientesService.obtenerPagina(consulta).pipe(map(pagina => ({
      ...pagina,
      elementos: pagina.elementos.map(cliente => this.convertirCliente(cliente))
    }))),
    'No se pudieron cargar los clientes.');
  protected readonly resumen = signal<ResumenClientes>({ total: 0, activos: 0 });
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
    estado: ['Activo' as 'Activo' | 'Inactivo']
  });

  protected filtrar(): void {
    const activo = this.estado() && String(this.estado() === 'Activo');
    this.grilla.filtrar({ texto: this.busqueda().trim(), activo });
  }

  protected puedeGestionarClientes(): boolean {
    return this.autenticacion.tienePermiso('clientes.gestionar');
  }

  protected campoInvalido(nombre: string): boolean {
    const control = this.formulario.get(nombre);
    return Boolean(control?.touched && control.invalid);
  }

  constructor() {
    this.cargarResumen();
  }

  protected abrirNuevo(): void {
    this.formulario.reset({
      nombre: '',
      apellido: '',
      email: '',
      telefono: '',
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
        this.recargar();
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
        this.recargar();
        this.clienteAEliminar.set(null);
        this.mostrarAviso('Cliente desactivado');
      },
      error: () => this.error.set('No se pudo desactivar el cliente.')
    });
  }

  private recargar(): void {
    this.grilla.cargar();
    this.cargarResumen();
  }

  private cargarResumen(): void {
    this.clientesService.obtenerResumen().subscribe(resumen => this.resumen.set(resumen));
  }

  private convertirCliente(cliente: ClienteApi): ClienteVista {
    return {
      id: cliente.id,
      nombre: `${cliente.nombre} ${cliente.apellido ?? ''}`.trim(),
      nombrePrincipal: cliente.nombre,
      apellido: cliente.apellido ?? '',
      email: cliente.email,
      telefono: cliente.telefono,
      estado: cliente.activo === false ? 'Inactivo' : 'Activo'
    };
  }

  private mostrarAviso(mensaje: string): void {
    this.aviso.set(mensaje);
    setTimeout(() => this.aviso.set(''), 2600);
  }
}
