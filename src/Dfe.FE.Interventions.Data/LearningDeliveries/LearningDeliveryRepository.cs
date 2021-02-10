using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dfe.FE.Interventions.Domain.LearningDeliveries;

namespace Dfe.FE.Interventions.Data.LearningDeliveries
{
    public class LearningDeliveryRepository : ILearningDeliveryRepository
    {
        private readonly IFeInterventionsDbContext _dbContext;

        public LearningDeliveryRepository(IFeInterventionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task ReplaceAllLearningDeliveriesForLearnerAsync(Guid learnerId, IEnumerable<LearningDelivery> learningDeliveries, CancellationToken cancellationToken)
        {
            // Done this as the OTB EF method to delete by anything other than PK would be slow.
            // If moving away from SQL Server, then should look at a more generic way of doing this
            await _dbContext.ExecuteSqlAsync("DELETE FROM LearningDelivery WHERE LearnerId = {0}", new object[] {learnerId}, cancellationToken);
            
            await _dbContext.LearningDeliveries.AddRangeAsync(learningDeliveries, cancellationToken);

            await _dbContext.CommitAsync(cancellationToken);
        }
    }
}