export const DIAS_SEMANA = ['Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sá', 'Do'];

export function formatearFecha(fecha: string): string {
  return fecha.split('-').reverse().join('/');
}

export function sumarMeses(mes: string, cantidad: number): string {
  const [anio, numero] = mes.split('-').map(Number);

  return new Date(Date.UTC(anio, numero - 1 + cantidad, 1)).toISOString().slice(0, 7);
}

export function armarMes(mes: string) {
  const [anio, numero] = mes.split('-').map(Number);
  const primero = new Date(Date.UTC(anio, numero - 1, 1));
  const cantidad = new Date(Date.UTC(anio, numero, 0)).getUTCDate();
  const titulo = primero.toLocaleDateString('es-AR', {
    month: 'long',
    year: 'numeric',
    timeZone: 'UTC'
  });

  return {
    clave: mes,
    titulo: titulo.charAt(0).toUpperCase() + titulo.slice(1),
    huecos: Array.from({ length: (primero.getUTCDay() + 6) % 7 }, (_, indice) => indice),
    dias: Array.from({ length: cantidad }, (_, indice) =>
      `${mes}-${String(indice + 1).padStart(2, '0')}`)
  };
}
