import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { ClientesService } from '../../core/services/clientes.service';

@Component({
  selector: 'app-formulario-cliente',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './formulario-cliente.component.html'
})
export class FormularioClienteComponent {
  private readonly constructorFormulario = inject(FormBuilder);
  private readonly clientesService = inject(ClientesService);
  private readonly ruta = inject(ActivatedRoute);
  private readonly router = inject(Router);

  protected readonly id = Number(this.ruta.snapshot.paramMap.get('id'));
  protected readonly editando = Boolean(this.id);
  protected readonly guardando = signal(false);
  protected readonly error = signal<string | null>(null);

  protected readonly formulario = this.constructorFormulario.nonNullable.group({
    nombre: ['', Validators.required],
    apellido: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    telefono: ['', Validators.required]
  });

  constructor() {
    if (this.editando) {
      this.clientesService.obtenerPorId(this.id).subscribe({
        next: cliente => this.formulario.patchValue(cliente),
        error: () => this.error.set('No se pudo cargar el cliente.')
      });
    }
  }

  protected guardar(): void {
    if (this.formulario.invalid) {
      this.formulario.markAllAsTouched();
      return;
    }

    this.guardando.set(true);
    this.error.set(null);
    const datos = { ...this.formulario.getRawValue(), activo: true };
    const peticion = this.editando
      ? this.clientesService.actualizar(this.id, datos)
      : this.clientesService.crear(datos);

    peticion.subscribe({
      next: () => void this.router.navigateByUrl('/clientes'),
      error: () => {
        this.guardando.set(false);
        this.error.set('No se pudo guardar el cliente.');
      }
    });
  }
}
