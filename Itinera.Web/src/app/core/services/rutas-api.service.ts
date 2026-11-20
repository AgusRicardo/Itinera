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

  get usuariosBuscar(): string {
    return `${this.usuarios}/buscar`;
  }

  get roles(): string {
    return `${this.apiUrl}/roles`;
  }

  get cargos(): string {
    return `${this.apiUrl}/cargos`;
  }

  get facturas(): string {
    return `${this.apiUrl}/facturas`;
  }

  get etiquetas(): string {
    return `${this.apiUrl}/etiquetas`;
  }

  get paises(): string {
    return `${this.apiUrl}/paises`;
  }

  get ciudades(): string {
    return `${this.apiUrl}/ciudades`;
  }

  etiquetaPorId(id: number): string {
    return `${this.etiquetas}/${id}`;
  }

  paisPorId(id: number): string {
    return `${this.paises}/${id}`;
  }

  ciudadPorId(id: number): string {
    return `${this.ciudades}/${id}`;
  }

  destinoPorId(id: number): string {
    return `${this.destinos}/${id}`;
  }

  actividadPorId(id: number): string {
    return `${this.actividades}/${id}`;
  }

  cargoPorId(id: number): string {
    return `${this.cargos}/${id}`;
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

  facturaPorId(id: number): string {
    return `${this.facturas}/${id}`;
  }

  pagosFactura(id: number): string {
    return `${this.facturaPorId(id)}/pagos`;
  }

  anularFactura(id: number): string {
    return `${this.facturaPorId(id)}/anular`;
  }

  pdfFactura(id: number): string {
    return `${this.facturaPorId(id)}/pdf`;
  }
}
