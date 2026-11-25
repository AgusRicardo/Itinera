import { Component, computed, input } from '@angular/core';

const colores: Record<string, string> = {
  Activo: 'bg-emerald-500',
  Activa: 'bg-emerald-500',
  Aceptada: 'bg-emerald-500',
  Listo: 'bg-emerald-500',
  Incluida: 'bg-emerald-500',
  Pagada: 'bg-emerald-500',
  Presentada: 'bg-blue-500',
  ParcialmentePagada: 'bg-blue-500',
  'Sin itinerario': 'bg-amber-500',
  Incompleto: 'bg-amber-500',
  'Falta ubicar': 'bg-amber-500',
  Pendiente: 'bg-amber-500',
  Rechazada: 'bg-red-500',
  Anulada: 'bg-red-500'
};

@Component({
  selector: 'app-estado',
  template: `
    <span class="inline-flex items-center gap-1.5 whitespace-nowrap text-xs font-medium">
      <span class="h-1.5 w-1.5 rounded-full" [class]="color()" aria-hidden="true"></span>
      {{ texto() }}
    </span>
  `
})
export class EstadoComponent {
  readonly estado = input.required<string>();

  protected readonly color = computed(() => colores[this.estado()] ?? 'bg-stone-400');
  protected readonly texto = computed(() =>
    this.estado() === 'ParcialmentePagada' ? 'Parcialmente pagada' : this.estado());
}
