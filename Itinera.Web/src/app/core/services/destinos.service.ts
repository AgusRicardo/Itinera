import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { ConsultaPagina, Pagina } from '../models/pagina';
import { RutasApiService } from './rutas-api.service';

export interface DestinoApi {
  id: number;
  activo: boolean;
  ciudadId: number;
  ciudadNombre: string;
  paisId: number;
  paisNombre: string;
}

@Injectable({ providedIn: 'root' })
export class DestinosService {
  private readonly http = inject(HttpClient);
  private readonly rutasApi = inject(RutasApiService);

  obtenerPagina(consulta: ConsultaPagina): Observable<Pagina<DestinoApi>> {
    return this.http.get<Pagina<DestinoApi>>(this.rutasApi.paginado(this.rutasApi.destinos), { params: consulta });
  }

  obtenerTodos(): Observable<DestinoApi[]> {
    return this.http.get<DestinoApi[]>(this.rutasApi.destinos);
  }

  crear(ciudadId: number): Observable<DestinoApi> {
    return this.http.post<DestinoApi>(this.rutasApi.destinos, { ciudadId });
  }

  actualizar(id: number, ciudadId: number): Observable<DestinoApi> {
    return this.http.put<DestinoApi>(this.rutasApi.destinoPorId(id), { ciudadId });
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(this.rutasApi.destinoPorId(id));
  }
}
