import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { RutasApiService } from './rutas-api.service';

export interface EstadoFactura {
  id: number;
  descripcion: string;
}

@Injectable({ providedIn: 'root' })
export class EstadosFacturasService {
  private readonly http = inject(HttpClient);
  private readonly rutasApi = inject(RutasApiService);

  obtenerTodos(): Observable<EstadoFactura[]> {
    return this.http.get<EstadoFactura[]>(this.rutasApi.estadosFacturas);
  }
}
