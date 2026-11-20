import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { RutasApiService } from './rutas-api.service';

export interface Cargo {
  id: number;
  descripcion: string;
  activo: boolean;
}

@Injectable({ providedIn: 'root' })
export class CargosService {
  private readonly http = inject(HttpClient);
  private readonly rutasApi = inject(RutasApiService);

  obtenerTodos(): Observable<Cargo[]> {
    return this.http.get<Cargo[]>(this.rutasApi.cargos);
  }

  crear(descripcion: string): Observable<Cargo> {
    return this.http.post<Cargo>(this.rutasApi.cargos, { descripcion });
  }

  actualizar(id: number, descripcion: string): Observable<Cargo> {
    return this.http.put<Cargo>(this.rutasApi.cargoPorId(id), { descripcion });
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(this.rutasApi.cargoPorId(id));
  }
}
