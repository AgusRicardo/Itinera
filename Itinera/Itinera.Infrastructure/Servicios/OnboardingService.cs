using Itinera.Application.Onboarding.Dtos;
using Itinera.Application.Onboarding.Interfaces;
using Itinera.Domain.Empresa;
using Itinera.Domain.Usuarios;
using Itinera.Infrastructure.Persistence;
using Itinera.Security.Application.Common;
using Itinera.Security.Domain;
using Microsoft.EntityFrameworkCore;
using EmpresaEntidad = Itinera.Domain.Empresa.Empresa;

namespace Itinera.Infrastructure.Servicios;

public sealed class OnboardingService(AppDbContext context) : IOnboardingService
{
    private readonly AppDbContext _context = context;

    public async Task<int> RegistrarEmpresaAsync(
        RegistrarEmpresaRequest request,
        CancellationToken cancellationToken = default)
    {
        await using var transaccion = await _context.Database.BeginTransactionAsync(cancellationToken);

        var (empresa, cargo) = await CrearEmpresaYCargoAsync(request, cancellationToken);
        await CrearAdministradorYVincularloAsync(request, empresa, cargo, cancellationToken);

        await transaccion.CommitAsync(cancellationToken);

        return empresa.Id;
    }

    private async Task<(EmpresaEntidad Empresa, Cargo Cargo)> CrearEmpresaYCargoAsync(
        RegistrarEmpresaRequest request,
        CancellationToken cancellationToken)
    {
        var empresa = new EmpresaEntidad(request.RazonSocial, request.CUIT, request.Telefono);
        var cargo = new Cargo("Administrador");

        _context.Empresas.Add(empresa);
        _context.Cargos.Add(cargo);
        await _context.SaveChangesAsync(cancellationToken);

        return (empresa, cargo);
    }

    private async Task CrearAdministradorYVincularloAsync(
        RegistrarEmpresaRequest request,
        EmpresaEntidad empresa,
        Cargo cargo,
        CancellationToken cancellationToken)
    {
        var empleado = new Empleado(
            request.AdministradorNombre,
            request.AdministradorApellido,
            request.AdministradorEmail,
            string.Empty,
            cargo,
            empresa);

        var usuario = new Usuario(
            $"{request.AdministradorNombre} {request.AdministradorApellido}".Trim(),
            request.AdministradorEmail,
            PasswordHasher.Hash(request.AdministradorPassword));

        _context.Empleados.Add(empleado);
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync(cancellationToken);

        var rolAdministrador = await _context.Roles
            .FirstAsync(rol => rol.Nombre == Roles.Administrador, cancellationToken);

        _context.UsuariosEmpleados.Add(new UsuarioEmpleado(usuario.Id, empleado.Id));
        _context.UsuarioComponentRoles.Add(new UsuarioComponentRol(usuario, rolAdministrador));
        await _context.SaveChangesAsync(cancellationToken);
    }
}
