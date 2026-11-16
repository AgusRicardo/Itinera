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
      import('./features/propuestas/crear-propuesta.component').then(
        componente => componente.CrearPropuestaComponent
      )
  },
  {
    path: 'propuestas/:id/editar',
    canActivate: [guardAutenticacion, guardPermiso],
    data: { permisos: ['itinerarios.editar'] },
    loadComponent: () =>
      import('./features/propuestas/editar-itinerario.component').then(
        componente => componente.EditarItinerarioComponent
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
    path: 'clientes/nuevo',
    canActivate: [guardAutenticacion, guardPermiso],
    data: { permisos: ['clientes.gestionar'] },
    loadComponent: () =>
      import('./features/clientes/formulario-cliente.component').then(
        componente => componente.FormularioClienteComponent
      )
  },
  {
    path: 'clientes/:id/editar',
    canActivate: [guardAutenticacion, guardPermiso],
    data: { permisos: ['clientes.gestionar'] },
    loadComponent: () =>
      import('./features/clientes/formulario-cliente.component').then(
        componente => componente.FormularioClienteComponent
      )
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
    data: { permisos: ['plataforma.usuarios.gestionar'] },
    loadComponent: () =>
      import('./features/administracion/usuarios.component').then(
        componente => componente.UsuariosComponent
      )
  },
  {
    path: 'administracion/roles',
    canActivate: [guardAutenticacion, guardPermiso],
    data: { permisos: ['plataforma.roles.gestionar'] },
    loadComponent: () =>
      import('./features/administracion/roles.component').then(
        componente => componente.RolesComponent
      )
  },
  {
    path: 'administracion/grupos',
    canActivate: [guardAutenticacion, guardPermiso],
    data: { permisos: ['plataforma.grupos.gestionar'] },
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
