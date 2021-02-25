using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dfe.FE.Interventions.Domain.LearningDeliveries
{
    public interface ILearningDeliveryRepository
    {
        Task<PagedSearchResult<LearningDeliverySynopsis>> ListForProviderAsync(int ukprn, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task ReplaceAllLearningDeliveriesForLearnerAsync(Guid learnerId, IEnumerable<LearningDelivery> learningDeliveries, CancellationToken cancellationToken);
        Task<int> GetCountOfAimTypesDeliveredByProviderAsync(int ukprn, CancellationToken cancellationToken);
    }
}