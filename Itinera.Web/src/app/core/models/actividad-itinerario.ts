export interface ActividadItinerario {
  id?: number;
  actividadId: number;
  actividadNombre?: string;
  fechaHoraInicio?: string;
  costoFinal?: number;
  observaciones?: string;
  orden?: number;
}
