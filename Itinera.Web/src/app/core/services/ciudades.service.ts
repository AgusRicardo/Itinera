import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { ConsultaPagina, Pagina } from '../models/pagina';
import { RutasApiService } from './rutas-api.service';

export interface Ciudad {
  id: number;
  nombre: string;
  activo: boolean;
  paisId: number;
  paisNombre: string;
}

@Injectable({ providedIn: 'root' })
export class CiudadesService {
  private readonly http = inject(HttpClient);
  private readonly rutasApi = inject(RutasApiService);

  obtenerPagina(consulta: ConsultaPagina): Observable<Pagina<Ciudad>> {
    return this.http.get<Pagina<Ciudad>>(this.rutasApi.paginado(this.rutasApi.ciudades), { params: consulta });
  }

  obtenerTodos(): Observable<Ciudad[]> {
    return this.http.get<Ciudad[]>(this.rutasApi.ciudades);
  }

  crear(nombre: string, paisId: number): Observable<Ciudad> {
    return this.http.post<Ciudad>(this.rutasApi.ciudades, { nombre, paisId });
  }

  actualizar(id: number, nombre: string, paisId: number): Observable<Ciudad> {
    return this.http.put<Ciudad>(this.rutasApi.ciudadPorId(id), { nombre, paisId });
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(this.rutasApi.ciudadPorId(id));
  }
}
