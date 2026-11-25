import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { ConsultaPagina, Pagina } from '../models/pagina';
import { RutasApiService } from './rutas-api.service';

export interface UsuarioAdministracion {
  id: string;
  nombre: string;
  email: string;
  activo: boolean;
  roles: string[];
  empresaId?: number;
  empresaRazonSocial?: string;
  empresaActiva?: boolean;
  cargoId?: number;
}

export interface RolAdministracion {
  id: number;
  nombre: string;
  descripcion: string;
  permisos: string[];
}

export interface CargoAdministracion {
  id: number;
  descripcion: string;
  activo: boolean;
}

export interface CrearUsuarioEmpresaRequest {
  empresaId: number;
  nombre: string;
  apellido: string;
  email: string;
  telefono: string;
  password: string;
  cargoId: number;
  rolId: number;
}

@Injectable({ providedIn: 'root' })
export class UsuariosService {
  private readonly http = inject(HttpClient);
  private readonly rutasApi = inject(RutasApiService);

  obtenerPagina(consulta: ConsultaPagina): Observable<Pagina<UsuarioAdministracion>> {
    return this.http.get<Pagina<UsuarioAdministracion>>(this.rutasApi.paginado(this.rutasApi.usuarios), { params: consulta });
  }

  buscar(texto: string): Observable<UsuarioAdministracion[]> {
    return this.http.get<UsuarioAdministracion[]>(
      this.rutasApi.usuariosBuscar,
      { params: { texto } }
    );
  }

  obtenerRoles(): Observable<RolAdministracion[]> {
    return this.http.get<RolAdministracion[]>(this.rutasApi.roles);
  }

  obtenerCargos(): Observable<CargoAdministracion[]> {
    return this.http.get<CargoAdministracion[]>(this.rutasApi.cargos);
  }

  crear(request: CrearUsuarioEmpresaRequest): Observable<unknown> {
    return this.http.post(this.rutasApi.administracionUsuarios, request);
  }

  asignarEmpresa(
    usuarioId: string,
    empresaId: number,
    cargoId: number,
    telefono = ''
  ): Observable<void> {
    return this.http.put<void>(this.rutasApi.usuarioEmpresa(usuarioId), {
      empresaId,
      cargoId,
      telefono
    });
  }

  asignarRol(usuarioId: string, rolId: number): Observable<void> {
    return this.http.post<void>(this.rutasApi.usuarioRoles(usuarioId), { rolId });
  }

  quitarRol(usuarioId: string, rolId: number): Observable<void> {
    return this.http.delete<void>(`${this.rutasApi.usuarioRoles(usuarioId)}/${rolId}`);
  }

  cambiarEstado(usuarioId: string, activo: boolean): Observable<void> {
    const ruta = activo
      ? this.rutasApi.usuarioActivar(usuarioId)
      : this.rutasApi.usuarioDesactivar(usuarioId);

    return this.http.post<void>(ruta, {});
  }
}
