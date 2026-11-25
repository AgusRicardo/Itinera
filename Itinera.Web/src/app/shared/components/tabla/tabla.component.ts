import { Component, input } from '@angular/core';

import { GrillaPaginada } from '../../../core/utils/grilla-paginada';
import { PaginadorComponent } from '../paginador/paginador.component';

@Component({
  selector: 'app-tabla',
  imports: [PaginadorComponent],
  template: `
    @if (grilla().cargando()) {
      <p class="rounded-xl border bg-itinera-card p-12 text-center text-sm text-itinera-muted">Cargando {{ elementos() }}…</p>
    } @else if (grilla().error()) {
      <p class="rounded-xl border border-red-200 bg-red-50 p-12 text-center text-sm text-red-700">{{ grilla().error() }}</p>
    } @else {
      <div class="contenedor-tabla">
        <table class="tabla" [style.min-width.px]="anchoMinimo()">
          <ng-content />
        </table>
        @if (!grilla().elementos().length) {
          <div class="py-14 text-center">
            <p class="text-sm font-medium">No encontramos {{ elementos() }}</p>
            <p class="mt-1 text-xs text-itinera-muted">Probá ajustando la búsqueda o los filtros.</p>
          </div>
        }
      </div>
      <app-paginador [grilla]="grilla()" />
    }
  `
})
export class TablaComponent {
  readonly grilla = input.required<GrillaPaginada<unknown>>();
  readonly elementos = input.required<string>();
  readonly anchoMinimo = input(640);
}
