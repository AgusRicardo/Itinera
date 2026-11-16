using Itinera.Application.Plataforma.Dtos;
using Itinera.Application.Plataforma.Interfaces;
using Itinera.Domain.Empresa;
using Itinera.Domain.Usuarios;
using Itinera.Infrastructure.Persistence;
using Itinera.Security.Application.Common;
using Itinera.Security.Domain;
using Microsoft.EntityFrameworkCore;
using EmpresaEntidad = Itinera.Domain.Empresa.Empresa;

namespace Itinera.Infrastructure.Servicios;

public sealed class AdministracionPlataformaService(AppDbContext context)
    : IAdministracionPlataformaService
{
    public async Task<UsuarioEmpresaResponse> CrearUsuarioEmpresaAsync(
        CrearUsuarioEmpresaRequest request,
        CancellationToken cancellationToken = default)
    {
        var empresa = await context.Empresas
            .FirstOrDefaultAsync(
                item => item.Id == request.EmpresaId && item.Activo,
                cancellationToken)
            ?? throw new KeyNotFoundException("Empresa no encontrada.");

        var cargo = await context.Cargos
            .FirstOrDefaultAsync(
                item => item.Id == request.CargoId && item.Activo,
                cancellationToken)
            ?? throw new KeyNotFoundException("Cargo no encontrado.");

        var rol = await context.Roles
            .FirstOrDefaultAsync(item => item.Id == request.RolId, cancellationToken)
            ?? throw new KeyNotFoundException("Rol no encontrado.");

        if (rol.Nombre == Roles.AdministradorItinera)
            throw new InvalidOperationException(
                "El administrador de plataforma no puede asignarse a una empresa.");

        var emailExiste = await context.Usuarios
            .AnyAsync(usuario => usuario.Email == request.Email, cancellationToken);

        if (emailExiste)
            throw new InvalidOperationException("El email ya está registrado.");

        var empleado = new Empleado(
            request.Nombre,
            request.Apellido,
            request.Email,
            request.Telefono,
            cargo,
            empresa);

        var usuario = new Usuario(
            $"{request.Nombre} {request.Apellido}".Trim(),
            request.Email,
            PasswordHasher.Hash(request.Password));

        context.Empleados.Add(empleado);
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync(cancellationToken);

        context.UsuariosEmpleados.Add(new UsuarioEmpleado(usuario.Id, empleado.Id));
        context.UsuarioComponentRoles.Add(new UsuarioComponentRol(usuario, rol));
        await context.SaveChangesAsync(cancellationToken);

        return new UsuarioEmpresaResponse(
            usuario.Id,
            empresa.Id,
            empleado.Id,
            usuario.Nombre,
            usuario.Email,
            rol.Nombre);
    }

    public async Task AsignarUsuarioEmpresaAsync(
        Guid usuarioId,
        AsignarUsuarioEmpresaRequest request,
        CancellationToken cancellationToken = default)
    {
        var usuario = await context.Usuarios
            .FirstOrDefaultAsync(item => item.Id == usuarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Usuario no encontrado.");

        var empresa = await context.Empresas
            .FirstOrDefaultAsync(
                item => item.Id == request.EmpresaId && item.Activo,
                cancellationToken)
            ?? throw new KeyNotFoundException("Empresa no encontrada.");

        var cargo = await context.Cargos
            .FirstOrDefaultAsync(
                item => item.Id == request.CargoId && item.Activo,
                cancellationToken)
            ?? throw new KeyNotFoundException("Cargo no encontrado.");

        var vinculo = await context.UsuariosEmpleados
            .Include(item => item.Empleado)
            .FirstOrDefaultAsync(item => item.UsuarioId == usuarioId, cancellationToken);

        var partesNombre = usuario.Nombre.Split(' ', 2);
        var empleado = new Empleado(
            partesNombre[0],
            partesNombre.Length > 1 ? partesNombre[1] : string.Empty,
            usuario.Email,
            request.Telefono,
            cargo,
            empresa);

        if (vinculo is null)
        {
            context.Empleados.Add(empleado);
            await context.SaveChangesAsync(cancellationToken);
            context.UsuariosEmpleados.Add(new UsuarioEmpleado(usuario.Id, empleado.Id));
        }
        else
        {
            vinculo.Empleado.Desactivar();
            context.Empleados.Add(empleado);
            await context.SaveChangesAsync(cancellationToken);
            vinculo.ReasignarEmpleado(empleado.Id);
            vinculo.Activar();
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
