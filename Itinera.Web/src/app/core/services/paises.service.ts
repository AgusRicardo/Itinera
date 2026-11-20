import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { RutasApiService } from './rutas-api.service';

export interface Pais {
  id: number;
  nombre: string;
  activo: boolean;
}

@Injectable({ providedIn: 'root' })
export class PaisesService {
  private readonly http = inject(HttpClient);
  private readonly rutasApi = inject(RutasApiService);

  obtenerTodos(): Observable<Pais[]> {
    return this.http.get<Pais[]>(this.rutasApi.paises);
  }

  crear(nombre: string): Observable<Pais> {
    return this.http.post<Pais>(this.rutasApi.paises, { nombre });
  }

  actualizar(id: number, nombre: string): Observable<Pais> {
    return this.http.put<Pais>(this.rutasApi.paisPorId(id), { nombre });
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(this.rutasApi.paisPorId(id));
  }
}
