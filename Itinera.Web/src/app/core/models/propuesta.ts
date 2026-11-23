import { Itinerario } from './itinerario';

export interface DestinoPropuesta {
  id: number;
  orden: number;
  descripcion: string;
  fechaLlegada: string;
  fechaPartida: string;
  itinerario: Itinerario | null;
  faltantes: string[];
}

export interface LimitesIntensidad {
  minutosDisponibles: number;
  minutosTrasladoEntreActividades: number;
  minutosPausaDescanso: number;
  minutosTrasladoPorDestino: number;
  maxActividadesPorDia: number;
}

export interface Propuesta {
  id: number;
  fechaCreacion: string;
  presupuesto: number;
  fechaInicio: string;
  fechaFin: string;
  cantidadViajeros: number;
  preferencias: string;
  intensidadViaje: 'Baja' | 'Media' | 'Alta';
  estado: string;
  clienteId: number;
  clienteNombre: string;
  empleadoId: number;
  empleadoNombre: string;
  etiquetas: {
    id: number;
    nombre: string;
  }[];
  destinos: DestinoPropuesta[];
  actividades: {
    id: number;
    destinoId: number;
    nombre: string;
    costoBase: number;
  }[];
  presupuestoUsado: number;
  presupuestoDisponible: number;
  itinerariosCompletos: boolean;
  limites: LimitesIntensidad;
}
