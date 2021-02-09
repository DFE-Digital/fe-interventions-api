using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dfe.FE.Interventions.Domain.Learners
{
    public interface ILearnerRepository
    {
        Task<UpsertResult<Guid>> UpsertLearnerAsync(Learner learner, CancellationToken cancellationToken);
    }
}