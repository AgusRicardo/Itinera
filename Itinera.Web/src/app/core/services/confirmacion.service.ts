import { Injectable, signal } from '@angular/core';

export interface Confirmacion {
  titulo: string;
  mensaje?: string;
  accion: string;
  peligro?: boolean;
}

@Injectable({ providedIn: 'root' })
export class ConfirmacionService {
  private resolver?: (confirmado: boolean) => void;

  readonly actual = signal<Confirmacion | null>(null);

  confirmar(confirmacion: Confirmacion): Promise<boolean> {
    this.responder(false);
    this.actual.set(confirmacion);

    return new Promise(resolver => this.resolver = resolver);
  }

  responder(confirmado: boolean): void {
    this.actual.set(null);
    this.resolver?.(confirmado);
    this.resolver = undefined;
  }
}
