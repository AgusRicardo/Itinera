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
    const token = localStorage.getItem(this.claveToken);

    if (token && this.estaVencido(token)) {
      this.cerrarSesion();
      return null;
    }

    return token;
  }

  obtenerUsuario(): RespuestaLogin | null {
    const usuario = localStorage.getItem(this.claveUsuario);
    return usuario ? JSON.parse(usuario) as RespuestaLogin : null;
  }

  estaAutenticado(): boolean {
    return Boolean(this.obtenerToken());
  }

  tienePermiso(codigo: string): boolean {
    return this.obtenerUsuario()?.permisos?.includes(codigo) ?? false;
  }

  cerrarSesion(): void {
    localStorage.removeItem(this.claveToken);
    localStorage.removeItem(this.claveUsuario);
  }

  private estaVencido(token: string): boolean {
    try {
      const contenido = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
      const { exp } = JSON.parse(atob(contenido)) as { exp?: number };

      return typeof exp !== 'number' || exp * 1000 <= Date.now();
    } catch {
      return true;
    }
  }

  private guardarSesion(respuesta: RespuestaLogin): void {
    localStorage.setItem(this.claveToken, respuesta.token);
    localStorage.setItem(this.claveUsuario, JSON.stringify(respuesta));
  }
}
