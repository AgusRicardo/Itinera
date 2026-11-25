export interface Pagina<T> {
  elementos: T[];
  numeroPagina: number;
  tamanoPagina: number;
  total: number;
}

export type FiltrosGrilla = Record<string, string | number | boolean | number[]>;

export type ConsultaPagina = FiltrosGrilla & { pagina: number; tamanoPagina: number };
