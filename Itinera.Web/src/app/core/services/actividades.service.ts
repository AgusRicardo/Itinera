import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { RutasApiService } from './rutas-api.service';

export interface ActividadDisponible {
  id: number;
  nombre: string;
  descripcion: string;
  costoBase: number;
  duracionEstimada: number;
  destinoId: number;
  activo: boolean;
}

@Injectable({ providedIn: 'root' })
export class ActividadesService {
  private readonly http = inject(HttpClient);
  private readonly rutasApi = inject(RutasApiService);

  obtenerTodas(): Observable<ActividadDisponible[]> {
    return this.http.get<ActividadDisponible[]>(this.rutasApi.actividades);
  }

  crear(request: Omit<ActividadDisponible, 'id' | 'activo'>): Observable<ActividadDisponible> {
    return this.http.post<ActividadDisponible>(this.rutasApi.actividades, request);
  }

  actualizar(id: number, request: Omit<ActividadDisponible, 'id' | 'activo'>): Observable<ActividadDisponible> {
    return this.http.put<ActividadDisponible>(this.rutasApi.actividadPorId(id), request);
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(this.rutasApi.actividadPorId(id));
  }
}
