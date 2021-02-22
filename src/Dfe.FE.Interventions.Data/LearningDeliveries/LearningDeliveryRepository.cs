using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dfe.FE.Interventions.Domain;
using Dfe.FE.Interventions.Domain.LearningDeliveries;
using Microsoft.EntityFrameworkCore;

namespace Dfe.FE.Interventions.Data.LearningDeliveries
{
    public class LearningDeliveryRepository : ILearningDeliveryRepository
    {
        private readonly IFeInterventionsDbContext _dbContext;

        public LearningDeliveryRepository(IFeInterventionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedSearchResult<LearningDeliverySynopsis>> ListForProviderAsync(int ukprn, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var query = _dbContext.LearningDeliveries
                .Join(_dbContext.Learners,
                    ld => ld.LearnerId,
                    l => l.Id,
                    (ld, l) => new
                    {
                        ld.Id,
                        ld.FundingModel,
                        ld.DeliveryLocationPostcode,
                        ld.ProgrammeType,
                        l.Ukprn,
                        l.LearnRefNumber,
                    })
                .Where(x => x.Ukprn == ukprn);

            var recordCount = await query.CountAsync(cancellationToken);
            
            var skip = (pageNumber - 1) * pageSize;
            var records = await query
                .OrderBy(x => x.LearnRefNumber)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedSearchResult<LearningDeliverySynopsis>
            {
                Results = records.Select(x => new LearningDeliverySynopsis
                {
                    Id = x.Id,
                    FundingModel = x.FundingModel,
                    DeliveryLocationPostcode = x.DeliveryLocationPostcode,
                    ProgrammeType = x.ProgrammeType,
                }).ToArray(),
                CurrentPage = pageNumber,
                TotalNumberOfRecords = recordCount,
                TotalNumberOfPages = (int) Math.Ceiling((float) recordCount / pageSize),
                PageStartIndex = skip + 1,
                PageFinishIndex = skip + records.Count,
            };
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