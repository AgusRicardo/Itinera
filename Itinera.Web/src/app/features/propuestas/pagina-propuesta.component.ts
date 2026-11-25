import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { Propuesta } from '../../core/models/propuesta';
import { PropuestasService } from '../../core/services/propuestas.service';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';
import { CrearPropuestaComponent } from './crear-propuesta.component';

@Component({
  selector: 'app-pagina-propuesta',
  imports: [MenuPrincipalComponent, CrearPropuestaComponent],
  template: `
    <app-menu-principal />
    <main class="min-h-screen bg-itinera-background lg:pl-64">
      <div class="mx-auto w-full max-w-7xl px-4 py-7 sm:px-6 lg:px-10 lg:py-10">
        @if (cargada()) {
          <app-crear-propuesta [propuesta]="propuesta()" />
        } @else {
          <p class="rounded-2xl border bg-itinera-card p-10 text-center text-sm text-itinera-muted">
            {{ error() ?? 'Cargando propuesta…' }}
          </p>
        }
      </div>
    </main>
  `
})
export class PaginaPropuestaComponent {
  private readonly propuestasService = inject(PropuestasService);
  private readonly propuestaId = Number(inject(ActivatedRoute).snapshot.paramMap.get('id')) || null;

  protected readonly propuesta = signal<Propuesta | null>(null);
  protected readonly cargada = signal(this.propuestaId === null);
  protected readonly error = signal<string | null>(null);

  constructor() {
    if (this.propuestaId === null) {
      return;
    }

    this.propuestasService.obtenerPorId(this.propuestaId).subscribe({
      next: propuesta => {
        this.propuesta.set(propuesta);
        this.cargada.set(true);
      },
      error: () => this.error.set('No se pudo cargar la propuesta.')
    });
  }
}
