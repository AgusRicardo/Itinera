import { Routes } from '@angular/router';
import { guardAutenticacion } from './core/guards/guard-autenticacion';

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
    canActivate: [guardAutenticacion],
    loadComponent: () =>
      import('./features/propuestas/propuestas.component').then(
        componente => componente.PropuestasComponent
      )
  },
  {
    path: 'propuestas/nueva',
    canActivate: [guardAutenticacion],
    loadComponent: () =>
      import('./features/propuestas/crear-propuesta.component').then(
        componente => componente.CrearPropuestaComponent
      )
  },
  {
    path: 'propuestas/:id/editar',
    canActivate: [guardAutenticacion],
    loadComponent: () =>
      import('./features/propuestas/editar-itinerario.component').then(
        componente => componente.EditarItinerarioComponent
      )
  },
  {
    path: 'clientes',
    canActivate: [guardAutenticacion],
    loadComponent: () =>
      import('./features/clientes/clientes.component').then(
        componente => componente.ClientesComponent
      )
  },
  {
    path: 'clientes/nuevo',
    canActivate: [guardAutenticacion],
    loadComponent: () =>
      import('./features/clientes/formulario-cliente.component').then(
        componente => componente.FormularioClienteComponent
      )
  },
  {
    path: 'clientes/:id/editar',
    canActivate: [guardAutenticacion],
    loadComponent: () =>
      import('./features/clientes/formulario-cliente.component').then(
        componente => componente.FormularioClienteComponent
      )
  },
  {
    path: 'propuestas/:id',
    canActivate: [guardAutenticacion],
    loadComponent: () =>
      import('./features/propuestas/detalle-propuesta.component').then(
        componente => componente.DetallePropuestaComponent
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
