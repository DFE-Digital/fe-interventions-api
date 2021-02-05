using System.Threading;
using System.Threading.Tasks;

namespace Dfe.FE.Interventions.Domain.Learners
{
    public interface ILearnerRepository
    {
        Task<bool> UpsertLearnerAsync(Learner learner, CancellationToken cancellationToken);
    }
}