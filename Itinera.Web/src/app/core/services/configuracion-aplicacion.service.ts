import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

import { environment } from '../../../environments/environment';

interface ConfiguracionRuntime {
  apiUrl: string;
}

@Injectable({ providedIn: 'root' })
export class ConfiguracionAplicacionService {
  private readonly http = inject(HttpClient);
  private configuracion: ConfiguracionRuntime = {
    apiUrl: environment.apiUrl
  };

  async cargar(): Promise<void> {
    if (!environment.production) {
      return;
    }

    this.configuracion = await firstValueFrom(
      this.http.get<ConfiguracionRuntime>('/config.json')
    );
  }

  obtenerApiUrl(): string {
    return this.configuracion.apiUrl;
  }
}
