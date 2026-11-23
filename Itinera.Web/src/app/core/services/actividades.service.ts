import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { ConsultaPagina, Pagina } from '../models/pagina';
import { RutasApiService } from './rutas-api.service';

export interface ActividadDisponible {
  id: number;
  nombre: string;
  descripcion: string;
  costoBase: number;
  duracionEstimada: number;
  destinoId: number;
  activo: boolean;
  generadoPorIA: boolean;
  ciudadId: number;
  ciudadNombre: string;
  paisId: number;
  paisNombre: string;
}

export type ActividadRequest = Pick<
  ActividadDisponible,
  'nombre' | 'descripcion' | 'costoBase' | 'duracionEstimada' | 'destinoId'
>;

@Injectable({ providedIn: 'root' })
export class ActividadesService {
  private readonly http = inject(HttpClient);
  private readonly rutasApi = inject(RutasApiService);

  obtenerPagina(consulta: ConsultaPagina): Observable<Pagina<ActividadDisponible>> {
    return this.http.get<Pagina<ActividadDisponible>>(this.rutasApi.paginado(this.rutasApi.actividades), { params: consulta });
  }

  obtenerTodas(): Observable<ActividadDisponible[]> {
    return this.http.get<ActividadDisponible[]>(this.rutasApi.actividades);
  }

  crear(request: ActividadRequest): Observable<ActividadDisponible> {
    return this.http.post<ActividadDisponible>(this.rutasApi.actividades, request);
  }

  actualizar(id: number, request: ActividadRequest): Observable<ActividadDisponible> {
    return this.http.put<ActividadDisponible>(this.rutasApi.actividadPorId(id), request);
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(this.rutasApi.actividadPorId(id));
  }
}
