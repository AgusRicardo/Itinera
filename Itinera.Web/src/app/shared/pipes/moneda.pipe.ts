import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'moneda' })
export class MonedaPipe implements PipeTransform {
  transform(valor: number | null | undefined, decimales = 0): string {
    if (valor === null || valor === undefined) {
      return '';
    }

    return `USD ${valor.toLocaleString('es-AR', {
      minimumFractionDigits: decimales,
      maximumFractionDigits: decimales
    })}`;
  }
}
