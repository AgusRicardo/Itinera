export interface ActividadItinerario {
  id: number;
  actividadId: number;
  nombre: string;
  duracionEstimada: number;
  fechaHoraInicio: string;
  costoFinal: number;
  observaciones: string;
  pedidaPorCliente: boolean;
  generadaPorIA: boolean;
}
