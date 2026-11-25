export interface Miga {
  texto: string;
  ruta?: string;
}

const propuestas: Miga = { texto: 'Propuestas', ruta: '/propuestas' };
const propuesta: Miga = { texto: 'Propuesta #:id', ruta: '/propuestas/:id' };
const itinerario: Miga = { texto: ':destino', ruta: '/propuestas/:id/destinos/:destinoId' };
const clientes: Miga = { texto: 'Clientes', ruta: '/clientes' };
const facturacion: Miga = { texto: 'Facturación', ruta: '/facturacion' };
const catalogos: Miga = { texto: 'Catálogos' };
const administracion: Miga = { texto: 'Administración' };

export const MIGAS_POR_RUTA: Record<string, Miga[]> = {
  'propuestas': [propuestas],
  'propuestas/nueva': [propuestas, { texto: 'Nueva propuesta' }],
  'propuestas/:id': [propuestas, propuesta],
  'propuestas/:id/editar': [propuestas, propuesta, { texto: 'Editar' }],
  'propuestas/:id/destinos/:destinoId': [propuestas, propuesta, itinerario],
  'propuestas/:id/destinos/:destinoId/armar': [propuestas, propuesta, itinerario, { texto: 'Armar a mano' }],
  'propuestas/:id/destinos/:destinoId/generar': [propuestas, propuesta, itinerario, { texto: 'Generar con IA' }],
  'clientes': [clientes],
  'facturacion': [facturacion],
  'facturacion/:id': [facturacion, { texto: 'Detalle de factura' }],
  'conceptos': [facturacion, { texto: 'Conceptos' }],
  'etiquetas': [catalogos, { texto: 'Etiquetas' }],
  'catalogos/paises': [catalogos, { texto: 'Países' }],
  'catalogos/ciudades': [catalogos, { texto: 'Ciudades' }],
  'catalogos/destinos': [catalogos, { texto: 'Destinos' }],
  'catalogos/actividades': [catalogos, { texto: 'Actividades' }],
  'administracion/empresas': [administracion, { texto: 'Empresas' }],
  'administracion/usuarios': [administracion, { texto: 'Usuarios' }],
  'administracion/roles': [administracion, { texto: 'Roles y permisos' }],
  'administracion/grupos': [administracion, { texto: 'Grupos' }],
  'administracion/cargos': [administracion, { texto: 'Cargos' }]
};
