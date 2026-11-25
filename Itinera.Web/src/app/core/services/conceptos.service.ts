import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { ConsultaPagina, Pagina } from '../models/pagina';
import { RutasApiService } from './rutas-api.service';

export interface Concepto {
  id: number;
  descripcion: string;
  precio: number;
  porcentaje: number | null;
}

export type ConceptoRequest = Omit<Concepto, 'id'>;

@Injectable({ providedIn: 'root' })
export class ConceptosService {
  private readonly http = inject(HttpClient);
  private readonly rutasApi = inject(RutasApiService);

  obtenerPagina(consulta: ConsultaPagina): Observable<Pagina<Concepto>> {
    return this.http.get<Pagina<Concepto>>(this.rutasApi.paginado(this.rutasApi.conceptos), { params: consulta });
  }

  obtenerTodos(): Observable<Concepto[]> {
    return this.http.get<Concepto[]>(this.rutasApi.conceptos);
  }

  crear(request: ConceptoRequest): Observable<Concepto> {
    return this.http.post<Concepto>(this.rutasApi.conceptos, request);
  }

  actualizar(id: number, request: ConceptoRequest): Observable<Concepto> {
    return this.http.put<Concepto>(this.rutasApi.conceptoPorId(id), request);
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(this.rutasApi.conceptoPorId(id));
  }
}
