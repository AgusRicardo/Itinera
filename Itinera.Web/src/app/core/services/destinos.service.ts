import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { RutasApiService } from './rutas-api.service';

interface DestinoApi {
  id: number;
  ciudadNombre: string;
  paisNombre: string;
}

@Injectable({ providedIn: 'root' })
export class DestinosService {
  private readonly http = inject(HttpClient);
  private readonly rutasApi = inject(RutasApiService);

  obtenerTodos(): Observable<DestinoApi[]> {
    return this.http.get<DestinoApi[]>(this.rutasApi.destinos);
  }
}
