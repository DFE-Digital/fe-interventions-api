using System.Threading;
using System.Threading.Tasks;

namespace Dfe.FE.Interventions.Domain.FeProviders
{
    public interface IFeProviderRepository
    {
        Task<PagedSearchResult<FeProviderSynopsis>>
            SearchFeProvidersAsync(int? ukprn, string legalName, int pageNumber, int pageSize, CancellationToken cancellationToken);
    }
}