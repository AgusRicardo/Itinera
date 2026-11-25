import { Routes } from '@angular/router';
import { guardAutenticacion } from './core/guards/guard-autenticacion';
import { guardPermiso } from './core/guards/guard-permiso';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () =>
      import('./features/autenticacion/login.component').then(
        componente => componente.LoginComponent
      )
  },
  {
    path: 'propuestas',
    canActivate: [guardAutenticacion, guardPermiso],
    data: { permisos: ['propuestas.ver'] },
    loadComponent: () =>
      import('./features/propuestas/propuestas.component').then(
        componente => componente.PropuestasComponent
      )
  },
  {
    path: 'propuestas/nueva',
    canActivate: [guardAutenticacion, guardPermiso],
    data: { permisos: ['propuestas.crear'] },
    loadComponent: () =>
      import('./features/propuestas/pagina-propuesta.component').then(
        componente => componente.PaginaPropuestaComponent
      )
  },
  {
    path: 'propuestas/:id/editar',
    canActivate: [guardAutenticacion, guardPermiso],
    data: { permisos: ['propuestas.editar'] },
    loadComponent: () =>
      import('./features/propuestas/pagina-propuesta.component').then(
        componente => componente.PaginaPropuestaComponent
      )
  },
  {
    path: 'propuestas/:id/destinos/:destinoId',
    canActivate: [guardAutenticacion, guardPermiso],
    data: { permisos: ['propuestas.ver'] },
    loadComponent: () =>
      import('./features/propuestas/itinerario.component').then(
        componente => componente.ItinerarioComponent
      )
  },
  {
    path: 'propuestas/:id/destinos/:destinoId/armar',
    canActivate: [guardAutenticacion, guardPermiso],
    data: { permisos: ['itinerarios.editar'] },
    loadComponent: () =>
      import('./features/propuestas/armar-itinerario.component').then(
        componente => componente.ArmarItinerarioComponent
      )
  },
  {
    path: 'propuestas/:id/destinos/:destinoId/generar',
    canActivate: [guardAutenticacion, guardPermiso],
    data: { permisos: ['itinerarios.generar-ia'] },
    loadComponent: () =>
      import('./features/propuestas/generar-itinerario.component').then(
        componente => componente.GenerarItinerarioComponent
      )
  },
  {
    path: 'clientes',
    canActivate: [guardAutenticacion, guardPermiso],
    data: { permisos: ['clientes.ver'] },
    loadComponent: () =>
      import('./features/clientes/clientes.component').then(
        componente => componente.ClientesComponent
      )
  },
  {
    path: 'facturacion',
    canActivate: [guardAutenticacion, guardPermiso],
    data: { permisos: ['facturacion.ver'] },
    loadComponent: () =>
      import('./features/facturacion/facturas.component').then(
        componente => componente.FacturasComponent
      )
  },
  {
    path: 'facturacion/:id',
    canActivate: [guardAutenticacion, guardPermiso],
    data: { permisos: ['facturacion.ver'] },
    loadComponent: () =>
      import('./features/facturacion/detalle-factura.component').then(
        componente => componente.DetalleFacturaComponent
      )
  },
  {
    path: 'conceptos',
    canActivate: [guardAutenticacion, guardPermiso],
    data: { permisos: ['conceptos.ver'] },
    loadComponent: () =>
      import('./features/facturacion/conceptos.component').then(
        componente => componente.ConceptosComponent
      )
  },
  {
    path: 'etiquetas',
    canActivate: [guardAutenticacion, guardPermiso],
    data: { permisos: ['etiquetas.ver'] },
    loadComponent: () =>
      import('./features/etiquetas/etiquetas.component').then(
        componente => componente.EtiquetasComponent
      )
  },
  {
    path: 'catalogos/paises',
    canActivate: [guardAutenticacion, guardPermiso],
    data: { permisos: ['catalogos.operativos.gestionar'] },
    loadComponent: () =>
      import('./features/catalogos/paises.component').then(componente => componente.PaisesComponent)
  },
  {
    path: 'catalogos/ciudades',
    canActivate: [guardAutenticacion, guardPermiso],
    data: { permisos: ['catalogos.operativos.gestionar'] },
    loadComponent: () =>
      import('./features/catalogos/ciudades.component').then(componente => componente.CiudadesComponent)
  },
  {
    path: 'catalogos/destinos',
    canActivate: [guardAutenticacion, guardPermiso],
    data: { permisos: ['catalogos.operativos.gestionar'] },
    loadComponent: () =>
      import('./features/catalogos/destinos.component').then(componente => componente.DestinosComponent)
  },
  {
    path: 'catalogos/actividades',
    canActivate: [guardAutenticacion, guardPermiso],
    data: { permisos: ['catalogos.operativos.gestionar'] },
    loadComponent: () =>
      import('./features/catalogos/actividades.component').then(componente => componente.ActividadesComponent)
  },
  {
    path: 'administracion/empresas',
    canActivate: [guardAutenticacion, guardPermiso],
    data: { permisos: ['plataforma.empresas.gestionar'] },
    loadComponent: () =>
      import('./features/administracion/empresas.component').then(
        componente => componente.EmpresasComponent
      )
  },
  {
    path: 'administracion/usuarios',
    canActivate: [guardAutenticacion, guardPermiso],
    data: { permisos: ['plataforma.usuarios.gestionar', 'seguridad.gestionar'] },
    loadComponent: () =>
      import('./features/administracion/usuarios.component').then(
        componente => componente.UsuariosComponent
      )
  },
  {
    path: 'administracion/roles',
    canActivate: [guardAutenticacion, guardPermiso],
    data: { permisos: ['plataforma.roles.gestionar', 'seguridad.gestionar'] },
    loadComponent: () =>
      import('./features/administracion/roles.component').then(
        componente => componente.RolesComponent
      )
  },
  {
    path: 'administracion/cargos',
    canActivate: [guardAutenticacion, guardPermiso],
    data: { permisos: ['catalogos.gestionar'] },
    loadComponent: () =>
      import('./features/administracion/cargos.component').then(
        componente => componente.CargosComponent
      )
  },
  {
    path: 'administracion/grupos',
    canActivate: [guardAutenticacion, guardPermiso],
    data: { permisos: ['plataforma.grupos.gestionar', 'seguridad.gestionar'] },
    loadComponent: () =>
      import('./features/administracion/grupos.component').then(
        componente => componente.GruposComponent
      )
  },
  {
    path: 'propuestas/:id',
    canActivate: [guardAutenticacion, guardPermiso],
    data: { permisos: ['propuestas.ver'] },
    loadComponent: () =>
      import('./features/propuestas/detalle-propuesta.component').then(
        componente => componente.DetallePropuestaComponent
      )
  },
  {
    path: 'acceso-denegado',
    canActivate: [guardAutenticacion],
    loadComponent: () =>
      import('./features/acceso-denegado/acceso-denegado.component').then(
        componente => componente.AccesoDenegadoComponent
      )
  },
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'login'
  },
  {
    path: '**',
    redirectTo: 'login'
  }
];
