import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { AutenticacionService } from '../../core/services/autenticacion.service';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule],
  templateUrl: './login.component.html'
})
export class LoginComponent {
  private readonly constructorFormulario = inject(FormBuilder);
  private readonly autenticacion = inject(AutenticacionService);
  private readonly router = inject(Router);

  protected readonly cargando = signal(false);
  protected readonly error = signal<string | null>(null);

  protected readonly formulario = this.constructorFormulario.nonNullable.group({
    email: ['', Validators.required],
    password: ['', Validators.required]
  });

  protected iniciarSesion(): void {
    if (this.formulario.invalid) {
      this.formulario.markAllAsTouched();
      return;
    }

    this.cargando.set(true);
    this.error.set(null);

    const { email, password } = this.formulario.getRawValue();

    this.autenticacion.iniciarSesion(email, password).subscribe({
      next: () => {
        this.cargando.set(false);
        void this.router.navigateByUrl(this.obtenerRutaInicial());
      },
      error: () => {
        this.cargando.set(false);
        this.error.set('No se pudo iniciar sesión. Verificá tus credenciales.');
      }
    });
  }

  private obtenerRutaInicial(): string {
    if (this.autenticacion.tienePermiso('plataforma.empresas.gestionar')) {
      return '/administracion/empresas';
    }

    if (this.autenticacion.tienePermiso('propuestas.ver')) {
      return '/propuestas';
    }

    if (this.autenticacion.tienePermiso('clientes.ver')) {
      return '/clientes';
    }

    return '/acceso-denegado';
  }
}
