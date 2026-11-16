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
  @Input() opciones: OpcionBuscable[] = [];
  @Input() placeholder = 'Buscar...';
  @Output() seleccionada = new EventEmitter<OpcionBuscable>();

  protected readonly texto = signal('');
  protected readonly abierto = signal(false);
  protected readonly resultados = computed(() => {
    const busqueda = this.texto().trim().toLowerCase();
    return busqueda.length < 3
      ? []
      : this.opciones
          .filter(opcion => opcion.nombre.toLowerCase().includes(busqueda))
          .slice(0, 8);
  });

  protected buscar(valor: string): void {
    this.texto.set(valor);
    this.abierto.set(valor.trim().length >= 3);
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
