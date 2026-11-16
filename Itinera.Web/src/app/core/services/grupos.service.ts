import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { RutasApiService } from './rutas-api.service';

export interface GrupoAdministracion {
  id: string;
  nombre: string;
  miembros: string[];
  roles: string[];
}

@Injectable({ providedIn: 'root' })
export class GruposService {
  private readonly http = inject(HttpClient);
  private readonly rutasApi = inject(RutasApiService);

  obtenerTodos(): Observable<GrupoAdministracion[]> {
    return this.http.get<GrupoAdministracion[]>(this.rutasApi.grupos);
  }

  crear(nombre: string): Observable<GrupoAdministracion> {
    return this.http.post<GrupoAdministracion>(this.rutasApi.grupos, { nombre });
  }

  agregarMiembro(grupoId: string, miembroId: string): Observable<void> {
    return this.http.post<void>(this.rutasApi.grupoMiembros(grupoId), { miembroId });
  }

  quitarMiembro(grupoId: string, miembroId: string): Observable<void> {
    return this.http.delete<void>(`${this.rutasApi.grupoMiembros(grupoId)}/${miembroId}`);
  }

  asignarRol(grupoId: string, rolId: number): Observable<void> {
    return this.http.post<void>(this.rutasApi.grupoRoles(grupoId), { rolId });
  }

  quitarRol(grupoId: string, rolId: number): Observable<void> {
    return this.http.delete<void>(`${this.rutasApi.grupoRoles(grupoId)}/${rolId}`);
  }
}
