import { Component, ElementRef, effect, inject, viewChild } from '@angular/core';

import { ConfirmacionService } from '../../../core/services/confirmacion.service';

@Component({
  selector: 'app-confirmacion',
  host: { '(document:keydown.escape)': 'servicio.responder(false)' },
  template: `
    @if (servicio.actual(); as confirmacion) {
      <div
        class="fixed inset-0 z-[70] flex items-center justify-center bg-itinera-foreground/30 p-4 backdrop-blur-[2px]"
        (click)="servicio.responder(false)"
      >
        <section
          role="alertdialog"
          aria-modal="true"
          aria-labelledby="tituloConfirmacion"
          class="w-full max-w-sm rounded-2xl border bg-itinera-card p-6 shadow-xl"
          (click)="$event.stopPropagation()"
        >
          <h2 id="tituloConfirmacion" class="font-itinera-display text-xl">{{ confirmacion.titulo }}</h2>
          @if (confirmacion.mensaje) {
            <p class="mt-2 text-sm text-itinera-muted">{{ confirmacion.mensaje }}</p>
          }
          <div class="mt-6 flex justify-end gap-2">
            <button #cancelar type="button" class="rounded-lg border px-4 py-2.5 text-sm" (click)="servicio.responder(false)">Cancelar</button>
            <button
              type="button"
              class="rounded-lg px-4 py-2.5 text-sm font-semibold"
              [class]="confirmacion.peligro ? 'bg-red-700 text-white' : 'bg-itinera-foreground text-itinera-background'"
              (click)="servicio.responder(true)"
            >
              {{ confirmacion.accion }}
            </button>
          </div>
        </section>
      </div>
    }
  `
})
export class ConfirmacionComponent {
  protected readonly servicio = inject(ConfirmacionService);
  private readonly cancelar = viewChild<ElementRef<HTMLButtonElement>>('cancelar');

  constructor() {
    effect(() => this.cancelar()?.nativeElement.focus());
  }
}
