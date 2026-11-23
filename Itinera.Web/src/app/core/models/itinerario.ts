import { ActividadItinerario } from './actividad-itinerario';

export interface Itinerario {
  id: number;
  nombre: string;
  costo: number;
  actividades: ActividadItinerario[];
}
