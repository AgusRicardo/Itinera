import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

import { RespuestaLogin } from '../models/respuesta-login';
import { RutasApiService } from './rutas-api.service';

@Injectable({ providedIn: 'root' })
export class AutenticacionService {
  private readonly http = inject(HttpClient);
  private readonly rutasApi = inject(RutasApiService);
  private readonly claveToken = 'itinera_token';
  private readonly claveUsuario = 'itinera_usuario';

  iniciarSesion(email: string, password: string): Observable<RespuestaLogin> {
    return this.http
      .post<RespuestaLogin>(this.rutasApi.iniciarSesion, { email, password })
      .pipe(tap(respuesta => this.guardarSesion(respuesta)));
  }

  obtenerToken(): string | null {
    return localStorage.getItem(this.claveToken);
  }

  estaAutenticado(): boolean {
    return Boolean(this.obtenerToken());
  }

  cerrarSesion(): void {
    localStorage.removeItem(this.claveToken);
    localStorage.removeItem(this.claveUsuario);
  }

  private guardarSesion(respuesta: RespuestaLogin): void {
    localStorage.setItem(this.claveToken, respuesta.token);
    localStorage.setItem(this.claveUsuario, JSON.stringify(respuesta));
  }
}
