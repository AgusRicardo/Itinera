import { Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { map } from 'rxjs';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { ClienteApi, ClientesService, ResumenClientes } from '../../core/services/clientes.service';
import { GrillaPaginada } from '../../core/utils/grilla-paginada';
import { ColumnaOrdenableComponent } from '../../shared/components/columna-ordenable/columna-ordenable.component';
import { AccionMenu, MenuAccionesComponent } from '../../shared/components/menu-acciones/menu-acciones.component';
import { EstadoComponent } from '../../shared/components/estado/estado.component';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';
import { AutenticacionService } from '../../core/services/autenticacion.service';

interface ClienteVista {
  id: number;
  nombre: string;
  nombrePrincipal: string;
  apellido: string;
  email: string;
  telefono: string;
  propuestas: number;
  ultimaGestion: string | null;
  estado: 'Activo' | 'Inactivo';
}
import { ConfirmacionService } from '../../core/services/confirmacion.service';
import { ToastrService } from '../../core/services/toastr.service';
import { EncabezadoComponent } from '../../shared/components/encabezado/encabezado.component';
import { FiltrosGrillaComponent } from '../../shared/components/filtros-grilla/filtros-grilla.component';
import { ModalComponent } from '../../shared/components/modal/modal.component';
import { TablaComponent } from '../../shared/components/tabla/tabla.component';
import { FiltrosGrilla } from '../../core/models/pagina';

@Component({
  selector: 'app-clientes',
  imports: [
    ReactiveFormsModule,
    DatePipe,
    ColumnaOrdenableComponent,
    MenuAccionesComponent,
    EstadoComponent,
    MenuPrincipalComponent,
    EncabezadoComponent,
    FiltrosGrillaComponent,
    ModalComponent,
    TablaComponent
  ],
  templateUrl: './clientes.component.html'
})
export class ClientesComponent {
  private readonly confirmacion = inject(ConfirmacionService);
  private readonly toastr = inject(ToastrService);
  private readonly clientesService = inject(ClientesService);
  private readonly constructorFormulario = inject(FormBuilder);
  private readonly autenticacion = inject(AutenticacionService);

  protected readonly grilla = new GrillaPaginada<ClienteVista>(
    consulta => this.clientesService.obtenerPagina(consulta).pipe(map(pagina => ({
      ...pagina,
      elementos: pagina.elementos.map(cliente => this.convertirCliente(cliente))
    }))),
    'No se pudieron cargar los clientes.');
  protected readonly resumen = signal<ResumenClientes>({
    total: 0,
    activos: 0,
    conPropuestas: 0,
    nuevosEsteMes: 0
  });
  protected readonly editor = signal<ClienteVista | 'nuevo' | null>(null);
  protected readonly guardando = signal(false);

  protected readonly formulario = this.constructorFormulario.nonNullable.group({
    nombre: ['', Validators.required],
    apellido: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    telefono: ['', Validators.required]
  });

  protected readonly estados = [
    { id: 1, descripcion: 'Activo' },
    { id: 2, descripcion: 'Inactivo' }
  ];

  protected readonly campos = [
    { id: 'nombre', etiqueta: 'Nombre', tipo: 'text', largo: 100 },
    { id: 'apellido', etiqueta: 'Apellido', tipo: 'text', largo: 100 },
    { id: 'email', etiqueta: 'Correo electrónico', tipo: 'email', largo: 200 },
    { id: 'telefono', etiqueta: 'Teléfono', tipo: 'tel', largo: 30 }
  ];

  protected filtrar(filtros: FiltrosGrilla): void {
    const estados = filtros['estadoIds'] as number[];

    this.grilla.filtrar({
      texto: filtros['texto'],
      activo: estados.length === 1 ? estados[0] === 1 : ''
    });
  }

  protected puedeGestionarClientes(): boolean {
    return this.autenticacion.tienePermiso('clientes.gestionar');
  }

  protected acciones(cliente: ClienteVista): AccionMenu[] {
    return [
      { texto: 'Editar', ejecutar: () => this.abrirEdicion(cliente) },
      cliente.estado === 'Activo'
        ? { texto: 'Desactivar', peligro: true, ejecutar: () => this.desactivar(cliente) }
        : { texto: 'Activar', ejecutar: () => this.activar(cliente) }
    ];
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
      telefono: ''
    });
    this.editor.set('nuevo');
  }

  protected abrirEdicion(cliente: ClienteVista): void {
    this.formulario.reset({
      nombre: cliente.nombrePrincipal,
      apellido: cliente.apellido,
      email: cliente.email,
      telefono: cliente.telefono
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
      telefono: datos.telefono
    };
    const peticion = actual === 'nuevo'
      ? this.clientesService.crear(datosApi)
      : this.clientesService.actualizar(actual.id, datosApi);

    peticion.subscribe({
      next: () => {
        this.guardando.set(false);
        this.cerrarEditor();
        this.recargar();
        this.toastr.exito(actual === 'nuevo' ? 'Cliente creado.' : 'Cliente actualizado.');
      },
      error: () => this.guardando.set(false)
    });
  }

  private async desactivar(cliente: ClienteVista): Promise<void> {
    const confirmado = await this.confirmacion.confirmar({
      titulo: `¿Desactivar a ${cliente.nombre}?`,
      mensaje: 'Va a seguir en la grilla como inactivo, pero no se podrá elegir en propuestas nuevas.',
      accion: 'Desactivar',
      peligro: true
    });

    if (!confirmado) {
      return;
    }

    this.clientesService.desactivar(cliente.id).subscribe(() => {
      this.recargar();
      this.toastr.exito('Cliente desactivado.');
    });
  }

  private activar(cliente: ClienteVista): void {
    this.clientesService.actualizar(cliente.id, {
      nombre: cliente.nombrePrincipal,
      apellido: cliente.apellido,
      email: cliente.email,
      telefono: cliente.telefono,
      activo: true
    }).subscribe({
      next: () => {
        this.recargar();
        this.toastr.exito('Cliente activado.');
      }
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
      propuestas: cliente.cantidadPropuestas ?? 0,
      ultimaGestion: cliente.ultimaGestion ?? null,
      estado: cliente.activo === false ? 'Inactivo' : 'Activo'
    };
  }
}
