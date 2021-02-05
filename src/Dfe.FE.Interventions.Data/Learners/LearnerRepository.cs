using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dfe.FE.Interventions.Domain.Learners;
using Microsoft.EntityFrameworkCore;

namespace Dfe.FE.Interventions.Data.Learners
{
    public class LearnerRepository : ILearnerRepository
    {
        private readonly IFeInterventionsDbContext _dbContext;

        public LearnerRepository(IFeInterventionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<bool> UpsertLearnerAsync(Learner learner, CancellationToken cancellationToken)
        {
            bool created;
            
            var existingLearner = await _dbContext.Learners
                .Where(x => x.Ukprn == learner.Ukprn &&
                            x.LearnRefNumber == learner.LearnRefNumber)
                .SingleOrDefaultAsync(cancellationToken);
            if (existingLearner == null)
            {
                _dbContext.Learners.Add(learner);
                created = true;
            }
            else
            {
                existingLearner.UpdateFrom(learner);
                created = false;
            }
            
            await _dbContext.CommitAsync(cancellationToken);
            return created;
        }
    }
}