import { Component, computed, input } from '@angular/core';

import { GrillaPaginada } from '../../../core/utils/grilla-paginada';

@Component({
  selector: 'th[appColumnaOrdenable]',
  host: { '[attr.aria-sort]': 'sentido()' },
  template: `
    <button
      type="button"
      class="inline-flex items-center gap-1 uppercase tracking-[inherit] hover:text-itinera-foreground"
      [class.text-itinera-foreground]="sentido()"
      (click)="grilla().ordenarPor(campo())"
    >
      <ng-content />
      <span class="w-2 text-itinera-accent" aria-hidden="true">
        {{ sentido() === 'ascending' ? '↑' : sentido() === 'descending' ? '↓' : '' }}
      </span>
    </button>
  `
})
export class ColumnaOrdenableComponent {
  readonly grilla = input.required<GrillaPaginada<unknown>>();
  readonly campo = input.required<string>({ alias: 'appColumnaOrdenable' });

  protected readonly sentido = computed(() =>
    this.grilla().orden() !== this.campo()
      ? null
      : this.grilla().descendente() ? 'descending' : 'ascending');
}
