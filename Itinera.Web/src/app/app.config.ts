import {
  ApplicationConfig,
  inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners
} from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { interceptorToken } from './core/interceptors/interceptor-token';
import { ConfiguracionAplicacionService } from './core/services/configuracion-aplicacion.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([interceptorToken])),
    provideAppInitializer(() =>
      inject(ConfiguracionAplicacionService).cargar()
    )
  ]
};
