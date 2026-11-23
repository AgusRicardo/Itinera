import { Component, EventEmitter, Input, Output, signal, computed } from '@angular/core';

export interface OpcionBuscable {
  id: string;
  nombre: string;
}

const normalizar = (texto: string): string =>
  texto.normalize('NFD').replace(/\p{Diacritic}/gu, '').toLowerCase();

@Component({
  selector: 'app-buscador-opciones',
  host: { class: 'block' },
  templateUrl: './buscador-opciones.component.html'
})
export class BuscadorOpcionesComponent {
  private readonly opcionesActuales = signal<OpcionBuscable[]>([]);

  @Input()
  set opciones(valor: OpcionBuscable[]) {
    this.opcionesActuales.set(valor);
  }

  @Input() placeholder = 'Buscar...';
  @Input() idCampo = '';
  @Input() limpiarAlSeleccionar = false;
  @Output() seleccionada = new EventEmitter<OpcionBuscable>();
  @Output() textoBuscado = new EventEmitter<string>();

  protected readonly texto = signal('');
  protected readonly abierto = signal(false);
  protected readonly resultados = computed(() => {
    const busqueda = normalizar(this.texto().trim());
    return busqueda.length < 3
      ? []
      : this.opcionesActuales()
          .filter(opcion => normalizar(opcion.nombre).includes(busqueda))
          .slice(0, 8);
  });

  protected buscar(valor: string): void {
    this.texto.set(valor);
    const textoNormalizado = valor.trim();
    this.abierto.set(textoNormalizado.length >= 3);
    this.textoBuscado.emit(textoNormalizado);
  }

  protected seleccionar(opcion: OpcionBuscable): void {
    this.texto.set(this.limpiarAlSeleccionar ? '' : opcion.nombre);
    this.abierto.set(false);
    this.seleccionada.emit(opcion);
  }

  protected cerrar(): void {
    this.abierto.set(false);
  }
}
