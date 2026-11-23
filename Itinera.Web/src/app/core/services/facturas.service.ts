import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Factura } from '../models/factura';
import { ConsultaPagina, Pagina } from '../models/pagina';
import { RutasApiService } from './rutas-api.service';

export interface CrearFacturaRequest {
  propuestaId: number;
  fechaVencimiento: string;
  detalles: {
    descripcion: string;
    cantidad: number;
    precioUnitario: number;
  }[];
}

export interface RegistrarPagoRequest {
  importe: number;
  fecha: string;
  medioPago: string;
}

@Injectable({ providedIn: 'root' })
export class FacturasService {
  private readonly http = inject(HttpClient);
  private readonly rutasApi = inject(RutasApiService);

  obtenerPagina(consulta: ConsultaPagina): Observable<Pagina<Factura>> {
    return this.http.get<Pagina<Factura>>(this.rutasApi.paginado(this.rutasApi.facturas), { params: consulta });
  }

  obtenerTodas(): Observable<Factura[]> {
    return this.http.get<Factura[]>(this.rutasApi.facturas);
  }

  obtenerPorId(id: number): Observable<Factura> {
    return this.http.get<Factura>(this.rutasApi.facturaPorId(id));
  }

  crear(request: CrearFacturaRequest): Observable<Factura> {
    return this.http.post<Factura>(this.rutasApi.facturas, request);
  }

  registrarPago(id: number, request: RegistrarPagoRequest): Observable<Factura> {
    return this.http.post<Factura>(this.rutasApi.pagosFactura(id), request);
  }

  anular(id: number): Observable<void> {
    return this.http.post<void>(this.rutasApi.anularFactura(id), {});
  }

  descargarPdf(id: number): Observable<Blob> {
    return this.http.get(this.rutasApi.pdfFactura(id), { responseType: 'blob' });
  }
}
