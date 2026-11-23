import { ActividadItinerario } from './actividad-itinerario';

export interface DestinoItinerario {
  id?: number;
  orden?: number;
  fechaLlegada?: string;
  fechaPartida?: string;
  destinoId?: number;
  destinoDescripcion?: string;
  actividades?: ActividadItinerario[];
}
