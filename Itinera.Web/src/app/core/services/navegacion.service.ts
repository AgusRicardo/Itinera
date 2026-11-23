import { Location } from '@angular/common';
import { Injectable, inject, signal } from '@angular/core';
import { NavigationEnd, NavigationStart, Router } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class NavegacionService {
  private readonly router = inject(Router);
  private readonly location = inject(Location);
  private readonly clave = 'itinera_historial';
  private historial = this.leer();
  private esRetroceso = false;
  private reemplaza = false;

  readonly nombreDestino = signal('');

  constructor() {
    this.router.events.subscribe(evento => {
      if (evento instanceof NavigationStart) {
        this.esRetroceso = evento.navigationTrigger === 'popstate';
        this.reemplaza = this.router.getCurrentNavigation()?.extras.replaceUrl ?? false;
        this.nombreDestino.set('');
      }

      if (evento instanceof NavigationEnd) {
        this.registrar(evento.urlAfterRedirects);
      }
    });
  }

  volver(rutaAlternativa: string): void {
    if (this.historial.length > 1) {
      this.location.back();
      return;
    }

    void this.router.navigateByUrl(rutaAlternativa, { replaceUrl: true });
  }

  terminarEn(ruta: string): void {
    if (this.historial.at(-2) === ruta) {
      this.location.back();
      return;
    }

    void this.router.navigateByUrl(ruta, { replaceUrl: true });
  }

  private registrar(url: string): void {
    if (url.startsWith('/login')) {
      this.historial = [];
    } else if (this.historial.at(-1) !== url) {
      if (this.esRetroceso && this.historial.at(-2) === url) {
        this.historial.pop();
      } else if (this.reemplaza && !this.esRetroceso && this.historial.length) {
        this.historial[this.historial.length - 1] = url;
      } else {
        this.historial.push(url);
      }
    }

    this.guardar();
  }

  private leer(): string[] {
    try {
      return JSON.parse(sessionStorage.getItem(this.clave) ?? '[]') as string[];
    } catch {
      return [];
    }
  }

  private guardar(): void {
    try {
      sessionStorage.setItem(this.clave, JSON.stringify(this.historial.slice(-30)));
    } catch {
      return;
    }
  }
}
