import { Injectable, effect, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class MenuService {
  private readonly clave = 'itinera_menu_contraido';

  readonly contraido = signal(this.leer());

  constructor() {
    effect(() => document.documentElement.classList.toggle('menu-contraido', this.contraido()));
  }

  alternar(): void {
    this.contraido.update(valor => !valor);

    try {
      localStorage.setItem(this.clave, String(this.contraido()));
    } catch {
      return;
    }
  }

  private leer(): boolean {
    try {
      return localStorage.getItem(this.clave) === 'true';
    } catch {
      return false;
    }
  }
}
