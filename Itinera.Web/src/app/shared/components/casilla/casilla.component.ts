import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-casilla',
  host: { class: 'block' },
  template: `
    <button
      type="button"
      role="checkbox"
      class="flex w-full items-center gap-2 py-1 text-left text-xs"
      [attr.aria-checked]="marcada()"
      (click)="cambio.emit(!marcada())"
    >
      <span
        class="flex h-4 w-4 shrink-0 items-center justify-center rounded border"
        [class]="marcada() ? 'border-itinera-foreground bg-itinera-foreground text-itinera-background' : 'bg-itinera-card'"
        aria-hidden="true"
      >
        @if (marcada()) {
          <svg viewBox="0 0 24 24" class="h-3 w-3" fill="none" stroke="currentColor" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"><path d="m5 12.5 4.5 4.5L19 7.5" /></svg>
        }
      </span>
      <ng-content />
    </button>
  `
})
export class CasillaComponent {
  readonly marcada = input.required<boolean>();
  readonly cambio = output<boolean>();
}
