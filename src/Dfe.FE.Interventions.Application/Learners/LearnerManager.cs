using System;
using System.Threading;
using System.Threading.Tasks;
using Dfe.FE.Interventions.Domain.FeProviders;
using Dfe.FE.Interventions.Domain.Learners;
using Microsoft.Extensions.Logging;

namespace Dfe.FE.Interventions.Application.Learners
{
    public interface ILearnerManager
    {
        Task<Guid> UpsertLearner(Learner learner, CancellationToken cancellationToken);
    }

    public class LearnerManager : ILearnerManager
    {
        private readonly ILearnerRepository _learnerRepository;
        private readonly IFeProviderRepository _providerRepository;
        private readonly ILogger<LearnerManager> _logger;

        public LearnerManager(
            ILearnerRepository learnerRepository,
            IFeProviderRepository providerRepository,
            ILogger<LearnerManager> logger)
        {
            _learnerRepository = learnerRepository;
            _providerRepository = providerRepository;
            _logger = logger;
        }

        public async Task<Guid> UpsertLearner(Learner learner, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(learner.LearnRefNumber))
            {
                throw new InvalidRequestException("Must provide LearnRefNumber");
            }

            var provider = await _providerRepository.RetrieveProviderAsync(learner.Ukprn, cancellationToken);
            if (provider == null)
            {
                throw new InvalidRequestException($"Cannot find provider with UKPRN {learner.Ukprn}");
            }

            var upsertResult = await _learnerRepository.UpsertLearnerAsync(learner, cancellationToken);
            _logger.LogInformation("Upsert learner {UKPRN} / {LearnRefNumber} resulted in the learner being {UpsertAction}, id: {LearnerId}",
                learner.Ukprn, learner.LearnRefNumber, upsertResult.Created ? "CREATED" : "UPDATED", upsertResult.Key);
            return upsertResult.Key;
        }
    }
}