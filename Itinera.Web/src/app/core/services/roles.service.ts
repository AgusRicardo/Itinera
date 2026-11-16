import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { RutasApiService } from './rutas-api.service';

export interface RolAdministracion {
  id: number;
  nombre: string;
  descripcion: string;
  permisos: string[];
}

export interface PermisoAdministracion {
  id: number;
  codigo: string;
  descripcion: string;
}

@Injectable({ providedIn: 'root' })
export class RolesService {
  private readonly http = inject(HttpClient);
  private readonly rutasApi = inject(RutasApiService);

  obtenerTodos(): Observable<RolAdministracion[]> {
    return this.http.get<RolAdministracion[]>(this.rutasApi.roles);
  }

  obtenerPermisos(): Observable<PermisoAdministracion[]> {
    return this.http.get<PermisoAdministracion[]>(
      `${this.rutasApi.roles}/permisos`
    );
  }

  crear(request: {
    nombre: string;
    descripcion: string;
    permisos: string[];
  }): Observable<RolAdministracion> {
    return this.http.post<RolAdministracion>(this.rutasApi.roles, request);
  }
}
