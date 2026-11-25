import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-modal',
  template: `
    <div class="fixed inset-0 z-40 flex items-center justify-center bg-itinera-foreground/30 p-4 backdrop-blur-[2px]" (click)="cerrar.emit()">
      <section
        role="dialog"
        aria-modal="true"
        [attr.aria-label]="titulo()"
        class="max-h-[90vh] w-full overflow-y-auto rounded-2xl border bg-itinera-card shadow-xl"
        [class]="ancho()"
        (click)="$event.stopPropagation()"
      >
        <header class="flex items-start justify-between border-b px-6 py-5">
          <div>
            <p class="text-[10px] font-semibold uppercase tracking-[0.16em] text-itinera-accent">{{ seccion() }}</p>
            <h2 class="mt-1 font-itinera-display text-xl">{{ titulo() }}</h2>
          </div>
          <button type="button" aria-label="Cerrar" class="p-2 text-itinera-muted" (click)="cerrar.emit()">✕</button>
        </header>
        <ng-content />
      </section>
    </div>
  `
})
export class ModalComponent {
  readonly seccion = input.required<string>();
  readonly titulo = input.required<string>();
  readonly ancho = input('max-w-md');
  readonly cerrar = output<void>();
}
