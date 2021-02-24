using System.Threading;
using System.Threading.Tasks;
using Dfe.FE.Interventions.Domain;
using Dfe.FE.Interventions.Domain.FeProviders;
using Dfe.FE.Interventions.Domain.Learners;
using Dfe.FE.Interventions.Domain.Locations;
using Microsoft.Extensions.Logging;

namespace Dfe.FE.Interventions.Application.FeProviders
{
    public interface IFeProviderManager
    {
        Task<PagedSearchResult<FeProviderSynopsis>> SearchAsync(int? ukprn, string legalName, int pageNumber, CancellationToken cancellationToken);
        Task<FeProvider> RetrieveAsync(int ukprn, CancellationToken cancellationToken);
        Task<FeProviderStatistics> RetrieveStatisticsAsync(int ukprn, CancellationToken cancellationToken);

        Task UpsertProvider(FeProvider provider, CancellationToken cancellationToken);
    }

    public class FeProviderManager : IFeProviderManager
    {
        private readonly IFeProviderRepository _feProviderRepository;
        private readonly ILearnerRepository _learnerRepository;
        private readonly ILocationService _locationService;
        private readonly ILogger<FeProviderManager> _logger;

        public FeProviderManager(
            IFeProviderRepository feProviderRepository,
            ILearnerRepository learnerRepository,
            ILocationService locationService,
            ILogger<FeProviderManager> logger)
        {
            _feProviderRepository = feProviderRepository;
            _learnerRepository = learnerRepository;
            _locationService = locationService;
            _logger = logger;
        }

        public async Task<PagedSearchResult<FeProviderSynopsis>> SearchAsync(int? ukprn, string legalName, int pageNumber, CancellationToken cancellationToken)
        {
            if (ukprn.HasValue && (ukprn < 10000000 || ukprn > 99999999))
            {
                throw new InvalidRequestException("UKPRN must be an 8 digit number");
            }

            if (pageNumber < 1)
            {
                throw new InvalidRequestException("Page must be a number greater than 0");
            }

            var result = await _feProviderRepository.SearchFeProvidersAsync(ukprn, legalName, pageNumber, PaginationConstants.PageSize, cancellationToken);
            if (pageNumber > result.TotalNumberOfPages)
            {
                throw new InvalidRequestException("Page number exceeds available pages. " +
                                                  $"Requested page {pageNumber}, but only {result.TotalNumberOfPages} pages available");
            }

            return result;
        }

        public async Task<FeProvider> RetrieveAsync(int ukprn, CancellationToken cancellationToken)
        {
            if (ukprn < 10000000 || ukprn > 99999999)
            {
                throw new InvalidRequestException("UKPRN must be an 8 digit number");
            }

            var provider = await _feProviderRepository.RetrieveProviderAsync(ukprn, cancellationToken);
            return provider;
        }

        public async Task<FeProviderStatistics> RetrieveStatisticsAsync(int ukprn, CancellationToken cancellationToken)
        {
            if (ukprn < 10000000 || ukprn > 99999999)
            {
                throw new InvalidRequestException("UKPRN must be an 8 digit number");
            }

            var numberOfApprenticeshipLearners = _learnerRepository.GetCountOfContinuingLearnersAtProviderWithFundingModelsAsync(
                ukprn,
                new[] {36},
                cancellationToken);

            await Task.WhenAll(new[]
            {
                numberOfApprenticeshipLearners,
            });

            return new FeProviderStatistics
            {
                NumberOfApprenticeshipLearners = numberOfApprenticeshipLearners.Result,
            };
        }

        public async Task UpsertProvider(FeProvider provider, CancellationToken cancellationToken)
        {
            if (provider.Ukprn < 10000000 || provider.Ukprn > 99999999)
            {
                throw new InvalidRequestException("UKPRN must be an 8 digit number");
            }

            var location = !string.IsNullOrEmpty(provider.LegalAddressPostcode)
                ? await _locationService.GetByPostcodeAsync(provider.LegalAddressPostcode, cancellationToken)
                : null;
            if (location != null)
            {
                _logger.LogInformation("Setting region for provider {UKPRN} to {Region} based on postcode {Postcode}",
                    provider.Ukprn, location.Region, provider.LegalAddressPostcode);
                provider.LegalAddressRegion = location.Region;
            }

            var created = await _feProviderRepository.UpsertProviderAsync(provider, cancellationToken);
            _logger.LogInformation("Upsert provider {UKPRN} resulted in the provider being {UpsertAction}",
                provider.Ukprn, created ? "CREATED" : "UPDATED");
        }
    }
}