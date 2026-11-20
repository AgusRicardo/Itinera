import { Component, inject } from '@angular/core';
import { ToastrService } from '../../../core/services/toastr.service';

@Component({
  selector: 'app-toastr',
  template: `
    <div class="pointer-events-none fixed right-4 top-4 z-[100] flex w-[min(420px,calc(100vw-2rem))] flex-col gap-3">
      @for (mensaje of toastr.mensajes(); track mensaje.id) {
        <div
          class="pointer-events-auto flex items-start gap-3 rounded-xl border px-4 py-3 text-sm shadow-xl"
          [class]="clase(mensaje.tipo)"
          role="status"
        >
          <span class="mt-0.5 font-bold">{{ simbolo(mensaje.tipo) }}</span>
          <p class="flex-1">{{ mensaje.texto }}</p>
          <button type="button" class="opacity-70 hover:opacity-100" (click)="toastr.cerrar(mensaje.id)">✕</button>
        </div>
      }
    </div>
  `
})
export class ToastrComponent {
  protected readonly toastr = inject(ToastrService);

  protected clase(tipo: string): string {
    return {
      success: 'border-emerald-200 bg-emerald-50 text-emerald-800',
      error: 'border-red-200 bg-red-50 text-red-800',
      info: 'border-blue-200 bg-blue-50 text-blue-800',
      warning: 'border-amber-200 bg-amber-50 text-amber-800'
    }[tipo] ?? 'border-itinera-border bg-itinera-card text-itinera-foreground';
  }

  protected simbolo(tipo: string): string {
    return tipo === 'success' ? '✓' : tipo === 'error' ? '!' : tipo === 'warning' ? '⚠' : 'i';
  }
}
