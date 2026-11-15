import { DestinoItinerario } from './destino-itinerario';

export interface Itinerario {
  id: number;
  nombre: string;
  fechaInicio: string;
  fechaFin: string;
  destinos: DestinoItinerario[];
}
