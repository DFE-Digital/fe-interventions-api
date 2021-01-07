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
        public FeProviderManager(ILogger<FeProviderManager> logger)
        {
            
        }
        
        public async Task<PagedSearchResult<FeProviderSynopsis>> SearchAsync(int? ukprn, string legalName, int pageNumber, CancellationToken cancellationToken)
        {
            return new PagedSearchResult<FeProviderSynopsis>
            {
                Results = new[]
                {
                    new FeProvider
                    {
                        Ukprn = 12345678,
                        LegalName = "Provider One",
                    },
                },
                CurrentPage = pageNumber,
                PageStartIndex = 1,
                PageFinishIndex = 1,
                TotalNumberOfPages = 1,
                TotalNumberOfRecords = 1,
            };
        }
    }
}