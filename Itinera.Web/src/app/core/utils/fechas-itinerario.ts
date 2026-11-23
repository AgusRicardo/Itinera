import { DestinoPropuesta, LimitesIntensidad } from '../models/propuesta';

const formatoDia = new Intl.DateTimeFormat('es-AR', {
  weekday: 'long',
  day: '2-digit',
  month: '2-digit',
  timeZone: 'UTC'
});

export function diasDeEstadia(destino: DestinoPropuesta): string[] {
  const dias: string[] = [];
  const actual = new Date(`${destino.fechaLlegada}T00:00:00Z`);
  const fin = new Date(`${destino.fechaPartida}T00:00:00Z`);

  while (actual <= fin) {
    dias.push(actual.toISOString().slice(0, 10));
    actual.setUTCDate(actual.getUTCDate() + 1);
  }

  return dias;
}

export function describirDia(fecha: string): string {
  const texto = formatoDia.format(new Date(`${fecha}T00:00:00Z`)).replace('-', '/');

  return texto.charAt(0).toUpperCase() + texto.slice(1);
}

export function fechaDe(fechaHora: string): string {
  return fechaHora.slice(0, 10);
}

export function horaDe(fechaHora: string): string {
  return fechaHora.slice(11, 16);
}

export function minutosOcupados(
  duraciones: number[],
  esDiaDeLlegada: boolean,
  limites: LimitesIntensidad
): number {
  if (!duraciones.length) {
    return 0;
  }

  return duraciones.reduce((total, duracion) => total + duracion, 0)
    + duraciones.length * limites.minutosTrasladoEntreActividades
    + (duraciones.length > 1 ? limites.minutosPausaDescanso : 0)
    + (esDiaDeLlegada ? limites.minutosTrasladoPorDestino : 0);
}
