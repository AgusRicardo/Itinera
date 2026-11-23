import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { ConsultaPagina, Pagina } from '../models/pagina';
import { RutasApiService } from './rutas-api.service';

export interface ClienteApi {
  id: number;
  nombre: string;
  apellido: string;
  email: string;
  telefono: string;
  activo?: boolean;
}

export interface ResumenClientes {
  total: number;
  activos: number;
}

@Injectable({ providedIn: 'root' })
export class ClientesService {
  private readonly http = inject(HttpClient);
  private readonly rutasApi = inject(RutasApiService);
  obtenerPagina(consulta: ConsultaPagina): Observable<Pagina<ClienteApi>> {
    return this.http.get<Pagina<ClienteApi>>(this.rutasApi.paginado(this.rutasApi.clientes), { params: consulta });
  }

  obtenerResumen(): Observable<ResumenClientes> {
    return this.http.get<ResumenClientes>(this.rutasApi.resumen(this.rutasApi.clientes));
  }

  obtenerTodos(): Observable<ClienteApi[]> {
    return this.http.get<ClienteApi[]>(this.rutasApi.clientes);
  }

  obtenerPorId(id: number): Observable<ClienteApi> {
    return this.http.get<ClienteApi>(this.rutasApi.clientePorId(id));
  }

  crear(cliente: Omit<ClienteApi, 'id'>): Observable<ClienteApi> {
    return this.http.post<ClienteApi>(this.rutasApi.clientes, cliente);
  }

  actualizar(id: number, cliente: Omit<ClienteApi, 'id'>): Observable<ClienteApi> {
    return this.http.put<ClienteApi>(this.rutasApi.clientePorId(id), cliente);
  }

  desactivar(id: number): Observable<void> {
    return this.http.delete<void>(this.rutasApi.clientePorId(id));
  }
}
