export interface Pagina<T> {
  elementos: T[];
  numeroPagina: number;
  tamanoPagina: number;
  total: number;
}

export interface ConsultaPagina {
  pagina: number;
  tamanoPagina: number;
  [filtro: string]: string | number;
}
