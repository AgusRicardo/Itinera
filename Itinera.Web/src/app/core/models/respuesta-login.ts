export interface RespuestaLogin {
  token: string;
  usuarioId: string;
  nombre: string;
  email: string;
  roles: string[];
}
