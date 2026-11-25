export interface Factura {
  id: number;
  numero: string;
  fechaEmision: string;
  fechaVencimiento: string;
  importeTotal: number;
  saldoPendiente: number;
  estado: string;
  propuestaId: number;
  empresaId: number;
  empresaNombre: string;
  clienteNombre: string;
  detalles: DetalleFactura[];
  pagos: PagoFactura[];
}

export interface DetalleFactura {
  id: number;
  descripcion: string;
  cantidad: number;
  precioUnitario: number;
  porcentaje: number | null;
  subTotal: number;
}

export interface PagoFactura {
  id: number;
  fecha: string;
  importe: number;
  medioPago: string;
}
