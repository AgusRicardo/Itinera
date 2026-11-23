import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Propuesta } from '../models/propuesta';
import { ConsultaPagina, Pagina } from '../models/pagina';
import { RutasApiService } from './rutas-api.service';

export interface EstadiaRequest {
  destinoId: number;
  fechaLlegada: string;
  fechaPartida: string;
}

export interface ActualizarPropuestaRequest {
  presupuesto: number;
  cantidadViajeros: number;
  fechaInicio: string;
  fechaFin: string;
  preferencias: string;
  intensidadViaje: 'Baja' | 'Media' | 'Alta';
  destinos: EstadiaRequest[];
  actividadIds: number[];
  etiquetaIds: number[];
}

export interface CrearPropuestaRequest extends ActualizarPropuestaRequest {
  clienteId: number;
  empleadoId: number;
}

export type RespuestaCliente = 'Aceptada' | 'Rechazada' | 'SolicitaModificaciones';

export interface GuardarItinerarioRequest {
  actividades: {
    actividadId: number;
    fechaHoraInicio: string;
    observaciones: string;
  }[];
}

export interface ActividadAlternativa {
  actividadId: number | null;
  nombre: string;
  descripcion: string;
  costoFinal: number;
  duracionEstimada: number;
  fechaHoraInicio: string;
  observaciones: string;
  orden: number;
  pedidaPorCliente: boolean;
}

export interface AlternativaItinerario {
  nombre: string;
  descripcion: string;
  costoEstimado: number;
  actividades: ActividadAlternativa[];
}

export interface AlternativaDia {
  fecha: string;
  descripcion: string;
  costoEstimado: number;
  actividades: ActividadAlternativa[];
}

export interface ResumenPropuestas {
  total: number;
  borradores: number;
  presentadas: number;
  aceptadas: number;
}

@Injectable({ providedIn: 'root' })
export class PropuestasService {
  private readonly http = inject(HttpClient);
  private readonly rutasApi = inject(RutasApiService);

  obtenerPagina(consulta: ConsultaPagina): Observable<Pagina<Propuesta>> {
    return this.http.get<Pagina<Propuesta>>(this.rutasApi.paginado(this.rutasApi.propuestas), { params: consulta });
  }

  obtenerResumen(): Observable<ResumenPropuestas> {
    return this.http.get<ResumenPropuestas>(this.rutasApi.resumen(this.rutasApi.propuestas));
  }

  obtenerTodas(): Observable<Propuesta[]> {
    return this.http.get<Propuesta[]>(this.rutasApi.propuestas);
  }

  obtenerPorId(id: number): Observable<Propuesta> {
    return this.http.get<Propuesta>(this.rutasApi.propuestaPorId(id));
  }

  crear(request: CrearPropuestaRequest): Observable<Propuesta> {
    return this.http.post<Propuesta>(this.rutasApi.propuestas, request);
  }

  actualizar(id: number, request: ActualizarPropuestaRequest): Observable<Propuesta> {
    return this.http.put<Propuesta>(this.rutasApi.propuestaPorId(id), request);
  }

  presentar(id: number): Observable<Propuesta> {
    return this.http.post<Propuesta>(this.rutasApi.presentarPropuesta(id), {});
  }

  registrarRespuesta(id: number, respuesta: RespuestaCliente): Observable<Propuesta> {
    return this.http.post<Propuesta>(this.rutasApi.registrarRespuesta(id), { respuesta });
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(this.rutasApi.eliminarPropuesta(id));
  }

  guardarItinerario(
    id: number,
    destinoId: number,
    request: GuardarItinerarioRequest
  ): Observable<Propuesta> {
    return this.http.put<Propuesta>(this.rutasApi.itinerarioDeDestino(id, destinoId), request);
  }

  generarAlternativas(
    id: number,
    destinoId: number,
    preferencias: string
  ): Observable<AlternativaItinerario[]> {
    return this.http.post<AlternativaItinerario[]>(
      `${this.rutasApi.itinerarioDeDestino(id, destinoId)}/alternativas`,
      { preferencias }
    );
  }

  aplicarAlternativa(
    id: number,
    destinoId: number,
    alternativa: AlternativaItinerario
  ): Observable<Propuesta> {
    return this.http.post<Propuesta>(
      `${this.rutasApi.itinerarioDeDestino(id, destinoId)}/aplicar-alternativa`,
      { alternativa }
    );
  }

  generarAlternativasDia(
    id: number,
    destinoId: number,
    fecha: string,
    instrucciones: string
  ): Observable<AlternativaDia[]> {
    return this.http.post<AlternativaDia[]>(
      `${this.rutasApi.itinerarioDeDestino(id, destinoId)}/dia/alternativas`,
      { fecha, instrucciones }
    );
  }

  aplicarAlternativaDia(
    id: number,
    destinoId: number,
    alternativa: AlternativaDia
  ): Observable<Propuesta> {
    return this.http.post<Propuesta>(
      `${this.rutasApi.itinerarioDeDestino(id, destinoId)}/dia/aplicar-alternativa`,
      { alternativa }
    );
  }

  generarCambioDestino(
    id: number,
    destinoId: number,
    nuevoDestinoId: number,
    preferencias: string
  ): Observable<AlternativaItinerario[]> {
    return this.http.post<AlternativaItinerario[]>(
      `${this.rutasApi.cambiarDestino(id, destinoId)}/alternativas`,
      { nuevoDestinoId, preferencias }
    );
  }

  cambiarDestino(
    id: number,
    destinoId: number,
    nuevoDestinoId: number,
    alternativa: AlternativaItinerario
  ): Observable<Propuesta> {
    return this.http.post<Propuesta>(
      this.rutasApi.cambiarDestino(id, destinoId),
      { nuevoDestinoId, alternativa }
    );
  }
}
