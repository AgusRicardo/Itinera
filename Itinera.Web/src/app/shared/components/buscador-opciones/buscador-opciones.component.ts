import { Component, EventEmitter, Input, Output, signal, computed } from '@angular/core';

export interface OpcionBuscable {
  id: string;
  nombre: string;
}

@Component({
  selector: 'app-buscador-opciones',
  templateUrl: './buscador-opciones.component.html'
})
export class BuscadorOpcionesComponent {
  private readonly opcionesActuales = signal<OpcionBuscable[]>([]);

  @Input()
  set opciones(valor: OpcionBuscable[]) {
    this.opcionesActuales.set(valor);
  }

  @Input() placeholder = 'Buscar...';
  @Output() seleccionada = new EventEmitter<OpcionBuscable>();
  @Output() textoBuscado = new EventEmitter<string>();

  protected readonly texto = signal('');
  protected readonly abierto = signal(false);
  protected readonly resultados = computed(() => {
    const busqueda = this.texto().trim().toLowerCase();
    return busqueda.length < 3
      ? []
      : this.opcionesActuales()
          .filter(opcion => opcion.nombre.toLowerCase().includes(busqueda))
          .slice(0, 8);
  });

  protected buscar(valor: string): void {
    this.texto.set(valor);
    const textoNormalizado = valor.trim();
    this.abierto.set(textoNormalizado.length >= 3);
    this.textoBuscado.emit(textoNormalizado);
  }

  protected seleccionar(opcion: OpcionBuscable): void {
    this.texto.set(opcion.nombre);
    this.abierto.set(false);
    this.seleccionada.emit(opcion);
  }

  protected cerrar(): void {
    this.abierto.set(false);
  }
}
