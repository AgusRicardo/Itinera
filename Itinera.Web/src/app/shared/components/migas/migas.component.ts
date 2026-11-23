import { Component, computed, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { NavegacionService } from '../../../core/services/navegacion.service';
import { MIGAS_POR_RUTA } from './migas-por-ruta';

@Component({
  selector: 'app-migas',
  imports: [RouterLink],
  templateUrl: './migas.component.html'
})
export class MigasComponent {
  private readonly ruta = inject(ActivatedRoute);
  private readonly navegacion = inject(NavegacionService);

  protected readonly migas = computed(() =>
    (MIGAS_POR_RUTA[this.ruta.snapshot.routeConfig?.path ?? ''] ?? []).map(miga => ({
      texto: this.completar(miga.texto),
      ruta: miga.ruta ? this.completar(miga.ruta) : null
    })));

  protected readonly rutaPadre = computed(() =>
    this.migas().slice(0, -1).reverse().find(miga => miga.ruta)?.ruta ?? null);

  protected volver(): void {
    this.navegacion.volver(this.rutaPadre() ?? '/');
  }

  private completar(texto: string): string {
    const parametros = this.ruta.snapshot.paramMap;

    return texto
      .replace(':destinoId', parametros.get('destinoId') ?? '')
      .replace(':destino', this.navegacion.nombreDestino() || 'Itinerario')
      .replace(':id', parametros.get('id') ?? '');
  }
}
