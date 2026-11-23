import { Component, computed, input } from '@angular/core';

import { GrillaPaginada } from '../../../core/utils/grilla-paginada';

@Component({
  selector: 'app-paginador',
  templateUrl: './paginador.component.html'
})
export class PaginadorComponent {
  readonly grilla = input.required<GrillaPaginada<unknown>>();

  protected readonly tamanos = [10, 25, 50];

  protected readonly cantidadPaginas = computed(() =>
    Math.max(1, Math.ceil(this.grilla().total() / this.grilla().tamanoPagina())));

  protected readonly desde = computed(() =>
    this.grilla().total() ? (this.grilla().pagina() - 1) * this.grilla().tamanoPagina() + 1 : 0);

  protected readonly hasta = computed(() =>
    Math.min(this.grilla().pagina() * this.grilla().tamanoPagina(), this.grilla().total()));

  protected readonly paginas = computed<(number | null)[]>(() => {
    const ultima = this.cantidadPaginas();
    const actual = this.grilla().pagina();
    const visibles = Array.from({ length: ultima }, (_, indice) => indice + 1)
      .filter(pagina => pagina === 1 || pagina === ultima || Math.abs(pagina - actual) <= 1);

    return visibles.flatMap((pagina, indice) =>
      indice && pagina - visibles[indice - 1] > 1 ? [null, pagina] : [pagina]);
  });
}
