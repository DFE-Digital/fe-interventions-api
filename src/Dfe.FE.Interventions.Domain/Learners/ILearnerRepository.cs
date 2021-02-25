using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dfe.FE.Interventions.Domain.Learners
{
    public interface ILearnerRepository
    {
        Task<Learner> GetAsync(Guid id, CancellationToken cancellationToken);
        
        Task<UpsertResult<Guid>> UpsertLearnerAsync(Learner learner, CancellationToken cancellationToken);

        Task<int> GetCountOfContinuingLearnersAtProviderAsync(int ukprn, CancellationToken cancellationToken);

        Task<int> GetCountOfLearnersOnABreakAtProviderAsync(int ukprn, CancellationToken cancellationToken);
        Task<int> GetCountOfContinuingLearnersAtProviderWithFundingModelsAsync(int ukprn, int[] fundingModels, CancellationToken cancellationToken);

        Task<Dictionary<string, int>> GetCountOfLearnersByProviderLocationAsync(int ukprn, CancellationToken cancellationToken);
    }
}