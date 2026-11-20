import { Injectable, signal } from '@angular/core';

export type TipoToastr = 'success' | 'error' | 'info' | 'warning';

export interface ToastrMensaje {
  id: number;
  tipo: TipoToastr;
  texto: string;
}

@Injectable({ providedIn: 'root' })
export class ToastrService {
  private siguienteId = 0;
  readonly mensajes = signal<ToastrMensaje[]>([]);

  exito(texto: string): void {
    this.mostrar('success', texto);
  }

  error(texto: string): void {
    this.mostrar('error', texto);
  }

  informacion(texto: string): void {
    this.mostrar('info', texto);
  }

  advertencia(texto: string): void {
    this.mostrar('warning', texto);
  }

  cerrar(id: number): void {
    this.mensajes.update(mensajes => mensajes.filter(mensaje => mensaje.id !== id));
  }

  private mostrar(tipo: TipoToastr, texto: string): void {
    const id = ++this.siguienteId;
    this.mensajes.update(mensajes => [...mensajes, { id, tipo, texto }]);
    setTimeout(() => this.cerrar(id), tipo === 'error' ? 6000 : 3500);
  }
}
