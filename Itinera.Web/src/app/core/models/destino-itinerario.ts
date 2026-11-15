import { ActividadItinerario } from './actividad-itinerario';

export interface DestinoItinerario {
  destinoId?: number;
  destinoNombre?: string;
  destinoDescripcion?: string;
  orden?: number;
  fechaLlegada?: string;
  fechaPartida?: string;
  actividades?: ActividadItinerario[];
}
