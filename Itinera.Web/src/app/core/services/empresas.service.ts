import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { RutasApiService } from './rutas-api.service';

export interface Empresa {
  id: number;
  razonSocial: string;
  cuit: string;
  telefono: string;
  activo: boolean;
}

export interface RegistrarEmpresaRequest {
  razonSocial: string;
  cuit: string;
  telefono: string;
  administradorNombre: string;
  administradorApellido: string;
  administradorEmail: string;
  administradorPassword: string;
}

@Injectable({ providedIn: 'root' })
export class EmpresasService {
  private readonly http = inject(HttpClient);
  private readonly rutasApi = inject(RutasApiService);

  obtenerTodas(): Observable<Empresa[]> {
    return this.http.get<Empresa[]>(this.rutasApi.empresas);
  }

  registrar(request: RegistrarEmpresaRequest): Observable<{ empresaId: number }> {
    return this.http.post<{ empresaId: number }>(
      this.rutasApi.onboardingEmpresas,
      request
    );
  }

  actualizar(
    id: number,
    empresa: Pick<Empresa, 'razonSocial' | 'cuit' | 'telefono'>
  ): Observable<Empresa> {
    return this.http.put<Empresa>(this.rutasApi.empresaPorId(id), empresa);
  }

  desactivar(id: number): Observable<void> {
    return this.http.delete<void>(this.rutasApi.empresaPorId(id));
  }
}
