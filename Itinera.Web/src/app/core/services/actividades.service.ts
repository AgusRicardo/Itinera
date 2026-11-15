import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { RutasApiService } from './rutas-api.service';

export interface ActividadDisponible {
  id: number;
  nombre: string;
  costoBase: number;
  destinoId: number;
}

@Injectable({ providedIn: 'root' })
export class ActividadesService {
  private readonly http = inject(HttpClient);
  private readonly rutasApi = inject(RutasApiService);

  obtenerTodas(): Observable<ActividadDisponible[]> {
    return this.http.get<ActividadDisponible[]>(this.rutasApi.actividades);
  }
}
