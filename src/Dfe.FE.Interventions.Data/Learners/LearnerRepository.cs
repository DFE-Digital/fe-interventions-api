using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dfe.FE.Interventions.Domain;
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
        
        public async Task<UpsertResult<Guid>> UpsertLearnerAsync(Learner learner, CancellationToken cancellationToken)
        {
            bool created;
            Guid key;
            
            var existingLearner = await _dbContext.Learners
                .Where(x => x.Ukprn == learner.Ukprn &&
                            x.LearnRefNumber == learner.LearnRefNumber)
                .SingleOrDefaultAsync(cancellationToken);
            if (existingLearner == null)
            {
                if (learner.Id == Guid.Empty)
                {
                    learner.Id = new Guid();
                }

                _dbContext.Learners.Add(learner);
                
                key = learner.Id;
                created = true;
            }
            else
            {
                existingLearner.UpdateFrom(learner);
                
                key = existingLearner.Id;
                created = false;
            }
            
            await _dbContext.CommitAsync(cancellationToken);
            return new UpsertResult<Guid>
            {
                Created = created,
                Key = key,
            };
        }
    }
}