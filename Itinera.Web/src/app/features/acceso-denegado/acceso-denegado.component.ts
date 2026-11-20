import { Component } from '@angular/core';
import { MenuPrincipalComponent } from '../../shared/components/menu-principal/menu-principal.component';

@Component({
  selector: 'app-acceso-denegado',
  imports: [MenuPrincipalComponent],
  template: `
    <app-menu-principal />
    <main class="flex min-h-screen items-center justify-center bg-itinera-background p-6 lg:pl-64">
      <section class="rounded-2xl border bg-itinera-card p-8 text-center shadow-sm">
        <h1 class="font-itinera-display text-2xl">Acceso denegado</h1>
        <p class="mt-2 text-sm text-itinera-muted">
          No tenés permisos para acceder a esta sección.
        </p>
      </section>
    </main>
  `
})
export class AccesoDenegadoComponent {}
