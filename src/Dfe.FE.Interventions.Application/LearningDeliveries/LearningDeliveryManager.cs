using System;
using System.Threading;
using System.Threading.Tasks;
using Dfe.FE.Interventions.Domain;
using Dfe.FE.Interventions.Domain.Learners;
using Dfe.FE.Interventions.Domain.LearningDeliveries;
using Microsoft.Extensions.Logging;

namespace Dfe.FE.Interventions.Application.LearningDeliveries
{
    public interface ILearningDeliveryManager
    {
        Task<PagedSearchResult<LearningDeliverySynopsis>> ListForProviderAsync(int ukprn, int pageNumber, CancellationToken cancellationToken);
        Task UpdateLearnersLearningDeliveriesAsync(LearningDelivery[] learningDeliveries, CancellationToken cancellationToken);
    }

    public class LearningDeliveryManager : ILearningDeliveryManager
    {
        private readonly ILearningDeliveryRepository _learningDeliveryRepository;
        private readonly ILearnerRepository _learnerRepository;
        private readonly ILogger<LearningDeliveryManager> _logger;

        public LearningDeliveryManager(
            ILearningDeliveryRepository learningDeliveryRepository,
            ILearnerRepository learnerRepository,
            ILogger<LearningDeliveryManager> logger)
        {
            _learningDeliveryRepository = learningDeliveryRepository;
            _learnerRepository = learnerRepository;
            _logger = logger;
        }

        public async Task<PagedSearchResult<LearningDeliverySynopsis>> ListForProviderAsync(int ukprn, int pageNumber, CancellationToken cancellationToken)
        {
            if (ukprn < 10000000 || ukprn > 99999999)
            {
                throw new InvalidRequestException("UKPRN must be an 8 digit number");
            }

            if (pageNumber < 1)
            {
                throw new InvalidRequestException("Page must be a number greater than 0");
            }
            
            var result = await _learningDeliveryRepository.ListForProviderAsync(ukprn, pageNumber, PaginationConstants.PageSize, cancellationToken);
            if (pageNumber > result.TotalNumberOfPages)
            {
                throw new InvalidRequestException("Page number exceeds available pages. " +
                                                  $"Requested page {pageNumber}, but only {result.TotalNumberOfPages} pages available");
            }
            
            return result;
        }

        public async Task UpdateLearnersLearningDeliveriesAsync(LearningDelivery[] learningDeliveries, CancellationToken cancellationToken)
        {
            // Ensure all deliveries have same learner id set
            var learnerId = Guid.Empty;
            foreach (var learningDelivery in learningDeliveries)
            {
                if (learningDelivery.LearnerId == Guid.Empty)
                {
                    throw new InvalidRequestException("All learning deliveries must have LearnerId set");
                }

                if (learnerId == Guid.Empty)
                {
                    learnerId = learningDelivery.LearnerId;
                }

                if (learningDelivery.LearnerId != learnerId)
                {
                    throw new InvalidRequestException("All learning deliveries must have the same LearnerId");
                }
            }

            // Check learner exists
            var learner = await _learnerRepository.GetAsync(learnerId, cancellationToken);
            if (learner == null)
            {
                throw new InvalidRequestException($"Cannot find learner with id {learnerId}");
            }

            // Store
            await _learningDeliveryRepository.ReplaceAllLearningDeliveriesForLearnerAsync(learnerId, learningDeliveries, cancellationToken);
            _logger.LogInformation("Updated learner {LearnerId} with {NumberOfLearningDeliveries} learning deliveries",
                learnerId, learningDeliveries.Length);
        }
    }
}