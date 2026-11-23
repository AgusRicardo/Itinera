export interface RespuestaLogin {
  token: string;
  usuarioId: string;
  nombre: string;
  email: string;
  roles: string[];
  permisos: string[];
  empresaRazonSocial?: string | null;
}
