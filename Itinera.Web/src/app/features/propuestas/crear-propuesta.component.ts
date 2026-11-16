import { Component, EventEmitter, computed, inject, signal, Output } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { forkJoin } from 'rxjs';

import { OpcionPropuesta } from '../../core/models/opciones-propuesta';
import { ClientesService } from '../../core/services/clientes.service';
import { DestinosService } from '../../core/services/destinos.service';
import { EmpleadosService } from '../../core/services/empleados.service';
import { PropuestasService } from '../../core/services/propuestas.service';

@Component({
  selector: 'app-crear-propuesta',
  imports: [ReactiveFormsModule],
  templateUrl: './crear-propuesta.component.html'
})
export class CrearPropuestaComponent {
  @Output() protected readonly cerrar = new EventEmitter<void>();
  private readonly constructorFormulario = inject(FormBuilder);
  private readonly propuestasService = inject(PropuestasService);
  private readonly clientesService = inject(ClientesService);
  private readonly empleadosService = inject(EmpleadosService);
  private readonly destinosService = inject(DestinosService);
  private readonly router = inject(Router);

  protected readonly cargando = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly destinosSeleccionados = signal<number[]>([]);
  protected readonly busquedaCliente = signal('');
  protected readonly clienteSeleccionado = signal<OpcionPropuesta | null>(null);
  protected readonly clientesAbiertos = signal(false);

  protected readonly clientes = signal<OpcionPropuesta[]>([]);
  protected readonly empleados = signal<OpcionPropuesta[]>([]);
  protected readonly destinos = signal<OpcionPropuesta[]>([]);

  protected readonly formulario = this.constructorFormulario.nonNullable.group({
    clienteId: [0, Validators.required],
    empleadoId: [0, Validators.required],
    presupuesto: [5000, [Validators.required, Validators.min(1)]],
    fechaInicio: ['', Validators.required],
    fechaFin: ['', Validators.required],
    preferencias: ['']
  });

  protected readonly clientesFiltrados = computed(() => {
    const texto = this.busquedaCliente().trim().toLowerCase();

    if (texto.length < 3) {
      return [];
    }

    return this.clientes()
      .filter(cliente => cliente.nombre.toLowerCase().includes(texto))
      .slice(0, 8);
  });

  constructor() {
    this.cargarOpciones();
  }

  private cargarOpciones(): void {
    forkJoin({
      clientes: this.clientesService.obtenerTodos(),
      empleados: this.empleadosService.obtenerTodos(),
      destinos: this.destinosService.obtenerTodos()
    }).subscribe({
      next: respuesta => {
        this.clientes.set(
          respuesta.clientes.map(cliente => ({
            id: cliente.id,
            nombre: `${cliente.nombre} ${cliente.apellido}`.trim()
          }))
        );
        this.empleados.set(
          respuesta.empleados.map(empleado => ({
            id: empleado.id,
            nombre: `${empleado.nombre} ${empleado.apellido}`.trim()
          }))
        );
        this.destinos.set(
          respuesta.destinos.map(destino => ({
            id: destino.id,
            nombre: `${destino.ciudadNombre}, ${destino.paisNombre}`
          }))
        );

        if (respuesta.empleados.length) {
          this.formulario.controls.empleadoId.setValue(respuesta.empleados[0].id);
        }
      },
      error: () => this.error.set('No se pudieron cargar los datos de la propuesta.')
    });
  }

  protected alternarDestino(id: number): void {
    const seleccionados = this.destinosSeleccionados();

    this.destinosSeleccionados.set(
      seleccionados.includes(id)
        ? seleccionados.filter(destinoId => destinoId !== id)
        : [...seleccionados, id]
    );
  }

  protected buscarCliente(valor: string): void {
    this.busquedaCliente.set(valor);
    this.clienteSeleccionado.set(null);
    this.clientesAbiertos.set(valor.trim().length >= 3);
    this.formulario.controls.clienteId.setValue(0);
  }

  protected seleccionarCliente(cliente: OpcionPropuesta): void {
    this.clienteSeleccionado.set(cliente);
    this.busquedaCliente.set(cliente.nombre);
    this.formulario.controls.clienteId.setValue(cliente.id);
    this.clientesAbiertos.set(false);
  }

  protected abrirClientes(): void {
    this.clientesAbiertos.set(this.busquedaCliente().trim().length >= 3);
  }

  protected cerrarClientes(): void {
    this.clientesAbiertos.set(false);
  }

  protected estaSeleccionado(id: number): boolean {
    return this.destinosSeleccionados().includes(id);
  }

  protected crear(): void {
    if (this.formulario.invalid || this.destinosSeleccionados().length === 0) {
      this.formulario.markAllAsTouched();
      this.error.set('Completá los campos obligatorios y seleccioná al menos un destino.');
      return;
    }

    this.cargando.set(true);
    this.error.set(null);

    this.propuestasService.crear({
      ...this.formulario.getRawValue(),
      destinoIds: this.destinosSeleccionados()
    }).subscribe({
      next: propuesta => {
        this.cargando.set(false);
        void this.router.navigate(['/propuestas', propuesta.id]);
      },
      error: () => {
        this.cargando.set(false);
        this.error.set('No se pudo crear la propuesta.');
      }
    });
  }

  protected salir(): void {
    if (this.cerrar.observed) {
      this.cerrar.emit();
      return;
    }

    void this.router.navigateByUrl('/propuestas');
  }
}
