import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { RutasApiService } from './rutas-api.service';

export interface EstadoPropuesta {
  id: number;
  descripcion: string;
}

@Injectable({ providedIn: 'root' })
export class EstadosPropuestasService {
  private readonly http = inject(HttpClient);
  private readonly rutasApi = inject(RutasApiService);

  obtenerTodos(): Observable<EstadoPropuesta[]> {
    return this.http.get<EstadoPropuesta[]>(this.rutasApi.estadosPropuestas);
  }
}
