using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dfe.FE.Interventions.Domain.Learners
{
    public interface ILearnerRepository
    {
        Task<Learner> GetAsync(Guid id, CancellationToken cancellationToken);
        
        Task<UpsertResult<Guid>> UpsertLearnerAsync(Learner learner, CancellationToken cancellationToken);

        Task<int> GetCountOfContinuingLearnersAtProviderWithFundingModelAsync(int ukprn, int fundingModel, CancellationToken cancellationToken);
    }
}