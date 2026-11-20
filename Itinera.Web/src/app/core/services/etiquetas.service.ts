import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Etiqueta } from '../models/etiqueta';
import { RutasApiService } from './rutas-api.service';

@Injectable({ providedIn: 'root' })
export class EtiquetasService {
  private readonly http = inject(HttpClient);
  private readonly rutasApi = inject(RutasApiService);

  obtenerTodas(): Observable<Etiqueta[]> {
    return this.http.get<Etiqueta[]>(this.rutasApi.etiquetas);
  }

  crear(nombre: string): Observable<Etiqueta> {
    return this.http.post<Etiqueta>(this.rutasApi.etiquetas, { nombre });
  }

  actualizar(id: number, nombre: string): Observable<Etiqueta> {
    return this.http.put<Etiqueta>(this.rutasApi.etiquetaPorId(id), { nombre });
  }

  desactivar(id: number): Observable<void> {
    return this.http.delete<void>(this.rutasApi.etiquetaPorId(id));
  }
}
