import { Itinerario } from './itinerario';

export interface Propuesta {
  id: number;
  fechaCreacion: string;
  presupuesto: number;
  estado: string;
  clienteId: number;
  clienteNombre: string;
  empleadoId: number;
  empleadoNombre: string;
  itinerario?: Itinerario;
}
