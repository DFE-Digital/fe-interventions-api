using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dfe.FE.Interventions.Domain.LearningDeliveries
{
    public interface ILearningDeliveryRepository
    {
        Task ReplaceAllLearningDeliveriesForLearnerAsync(int learnerId, IEnumerable<LearningDelivery> learningDeliveries, CancellationToken cancellationToken);
    }
}