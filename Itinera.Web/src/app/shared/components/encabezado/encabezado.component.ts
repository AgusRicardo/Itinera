import { Component, input } from '@angular/core';

@Component({
  selector: 'app-encabezado',
  template: `
    <header class="mb-8 flex flex-wrap items-end justify-between gap-5">
      <div>
        <p class="mb-2 text-[10px] font-semibold uppercase tracking-[0.18em] text-itinera-accent">{{ seccion() }}</p>
        <h1 class="font-itinera-display text-3xl tracking-tight sm:text-4xl">{{ titulo() }}</h1>
        <p class="mt-1.5 text-sm text-itinera-muted">{{ descripcion() }}</p>
      </div>
      <ng-content />
    </header>
  `
})
export class EncabezadoComponent {
  readonly seccion = input.required<string>();
  readonly titulo = input.required<string>();
  readonly descripcion = input.required<string>();
}
