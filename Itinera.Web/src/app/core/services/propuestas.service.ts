import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Propuesta } from '../models/propuesta';
import { RutasApiService } from './rutas-api.service';

export interface CrearPropuestaRequest {
  clienteId: number;
  empleadoId: number;
  presupuesto: number;
  fechaInicio: string;
  fechaFin: string;
  preferencias: string;
  destinoIds: number[];
}

export type RespuestaCliente = 'Aceptada' | 'Rechazada' | 'SolicitaModificaciones';

export interface ModificarItinerarioRequest {
  fechaInicio: string;
  fechaFin: string;
  destinos: {
    destinoId: number;
    orden: number;
    fechaLlegada: string;
    fechaPartida: string;
    actividades: {
      actividadId: number;
      fechaHoraInicio: string;
      costoFinal: number;
      observaciones: string;
      orden: number;
    }[];
  }[];
}

@Injectable({ providedIn: 'root' })
export class PropuestasService {
  private readonly http = inject(HttpClient);
  private readonly rutasApi = inject(RutasApiService);

  obtenerTodas(): Observable<Propuesta[]> {
    return this.http.get<Propuesta[]>(this.rutasApi.propuestas);
  }

  obtenerPorId(id: number): Observable<Propuesta> {
    return this.http.get<Propuesta>(this.rutasApi.propuestaPorId(id));
  }

  crear(request: CrearPropuestaRequest): Observable<Propuesta> {
    return this.http.post<Propuesta>(this.rutasApi.propuestas, request);
  }

  presentar(id: number): Observable<Propuesta> {
    return this.http.post<Propuesta>(this.rutasApi.presentarPropuesta(id), {});
  }

  registrarRespuesta(id: number, respuesta: RespuestaCliente): Observable<Propuesta> {
    return this.http.post<Propuesta>(
      this.rutasApi.registrarRespuesta(id),
      { respuesta }
    );
  }

  modificarItinerario(
    id: number,
    request: ModificarItinerarioRequest
  ): Observable<Propuesta> {
    return this.http.put<Propuesta>(this.rutasApi.modificarItinerario(id), request);
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(this.rutasApi.eliminarPropuesta(id));
  }
}
