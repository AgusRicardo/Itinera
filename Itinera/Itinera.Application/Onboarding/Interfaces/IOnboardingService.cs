using Itinera.Application.Onboarding.Dtos;

namespace Itinera.Application.Onboarding.Interfaces;

public interface IOnboardingService
{
    Task<int> RegistrarEmpresaAsync(
        RegistrarEmpresaRequest request,
        CancellationToken cancellationToken = default);
}
