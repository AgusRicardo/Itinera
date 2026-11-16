import { Injectable, inject } from '@angular/core';
import { ConfiguracionAplicacionService } from './configuracion-aplicacion.service';

@Injectable({ providedIn: 'root' })

export class RutasApiService {
  private readonly configuracion = inject(ConfiguracionAplicacionService);

  private get apiUrl(): string {
    return this.configuracion.obtenerApiUrl();
  }

  get iniciarSesion(): string {
    return `${this.apiUrl}/auth/login`;
  }

  get propuestas(): string {
    return `${this.apiUrl}/propuestas`;
  }

  get clientes(): string {
    return `${this.apiUrl}/clientes`;
  }

  get empleados(): string {
    return `${this.apiUrl}/empleados`;
  }

  get destinos(): string {
    return `${this.apiUrl}/destinos`;
  }

  get actividades(): string {
    return `${this.apiUrl}/actividades`;
  }

  get estadosPropuestas(): string {
    return `${this.apiUrl}/estadospropuestas`;
  }

  get usuarios(): string {
    return `${this.apiUrl}/usuarios`;
  }

  get roles(): string {
    return `${this.apiUrl}/roles`;
  }

  get cargos(): string {
    return `${this.apiUrl}/cargos`;
  }

  get administracionUsuarios(): string {
    return `${this.apiUrl}/administracion/usuarios`;
  }

  usuarioEmpresa(id: string): string {
    return `${this.administracionUsuarios}/${id}/empresa`;
  }

  usuarioRoles(id: string): string {
    return `${this.usuarios}/${id}/roles`;
  }

  usuarioActivar(id: string): string {
    return `${this.usuarios}/${id}/activar`;
  }

  usuarioDesactivar(id: string): string {
    return `${this.usuarios}/${id}/desactivar`;
  }

  get grupos(): string {
    return `${this.apiUrl}/grupos`;
  }

  grupoMiembros(id: string): string {
    return `${this.grupos}/${id}/miembros`;
  }

  grupoRoles(id: string): string {
    return `${this.grupos}/${id}/roles`;
  }

  get empresas(): string {
    return `${this.apiUrl}/empresas`;
  }

  empresaPorId(id: number): string {
    return `${this.empresas}/${id}`;
  }

  get onboardingEmpresas(): string {
    return `${this.apiUrl}/onboarding/empresas`;
  }

  clientePorId(id: number): string {
    return `${this.clientes}/${id}`;
  }

  propuestaPorId(id: number): string {
    return `${this.propuestas}/${id}`;
  }

  presentarPropuesta(id: number): string {
    return `${this.propuestaPorId(id)}/presentar`;
  }

  registrarRespuesta(id: number): string {
    return `${this.propuestaPorId(id)}/respuesta`;
  }

  itinerarioDePropuesta(id: number): string {
    return `${this.propuestaPorId(id)}/itinerario`;
  }

  modificarItinerario(id: number): string {
    return this.itinerarioDePropuesta(id);
  }

  eliminarPropuesta(id: number): string {
    return this.propuestaPorId(id);
  }
}
