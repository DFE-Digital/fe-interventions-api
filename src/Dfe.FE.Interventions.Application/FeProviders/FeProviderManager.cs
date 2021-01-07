using System;
using System.Threading;
using System.Threading.Tasks;
using Dfe.FE.Interventions.Domain;
using Dfe.FE.Interventions.Domain.FeProviders;
using Microsoft.Extensions.Logging;

namespace Dfe.FE.Interventions.Application.FeProviders
{
    public interface IFeProviderManager
    {
        Task<PagedSearchResult<FeProviderSynopsis>> SearchAsync(int? ukprn, string legalName, int pageNumber, CancellationToken cancellationToken);
    }

    public class FeProviderManager : IFeProviderManager
    {
        private readonly IFeProviderRepository _feProviderRepository;
        private readonly ILogger<FeProviderManager> _logger;

        public FeProviderManager(
            IFeProviderRepository feProviderRepository,
            ILogger<FeProviderManager> logger)
        {
            _feProviderRepository = feProviderRepository;
            _logger = logger;
        }
        
        public async Task<PagedSearchResult<FeProviderSynopsis>> SearchAsync(int? ukprn, string legalName, int pageNumber, CancellationToken cancellationToken)
        {
            var result = await _feProviderRepository.SearchFeProvidersAsync(ukprn, legalName, pageNumber, 3, cancellationToken);
            return result;
        }
    }
}