using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dfe.FE.Interventions.Domain;
using Dfe.FE.Interventions.Domain.FeProviders;
using Microsoft.EntityFrameworkCore;

namespace Dfe.FE.Interventions.Data.FeProviders
{
    public class FeProviderRepository : IFeProviderRepository
    {
        private readonly IFeInterventionsDbContext _dbContext;

        public FeProviderRepository(IFeInterventionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedSearchResult<FeProviderSynopsis>> SearchFeProvidersAsync(int? ukprn, string legalName, int pageNumber, int pageSize,
            CancellationToken cancellationToken)
        {
            IQueryable<FeProvider> query = _dbContext.FeProviders;

            if (ukprn.HasValue)
            {
                query = query.Where(x => x.Ukprn == ukprn.Value);
            }

            if (!string.IsNullOrEmpty(legalName))
            {
                query = query.Where(x => EF.Functions.Like(x.LegalName, $"%{legalName}%"));
            }

            var skip = (pageNumber - 1) * pageSize;
            var recordCount = await query.CountAsync(cancellationToken);
            var records = await query
                .OrderBy(x => x.Ukprn)
                .Skip(skip)
                .Take(pageSize)
                .Select(x => new
                {
                    x.Ukprn,
                    x.LegalName,
                    x.Status,
                })
                .ToListAsync(cancellationToken);

            return new PagedSearchResult<FeProviderSynopsis>
            {
                Results = records.Select(x => new FeProviderSynopsis
                {
                    Ukprn = x.Ukprn,
                    LegalName = x.LegalName,
                    Status = x.Status,
                }).ToArray(),
                CurrentPage = pageNumber,
                TotalNumberOfRecords = recordCount,
                TotalNumberOfPages = (int) Math.Ceiling((float) recordCount / pageSize),
                PageStartIndex = skip + 1,
                PageFinishIndex = skip + records.Count,
            };
        }

        public async Task<FeProvider> RetrieveProviderAsync(int ukprn, CancellationToken cancellationToken)
        {
            var record = await _dbContext.FeProviders
                .Where(x => x.Ukprn == ukprn)
                .SingleOrDefaultAsync(cancellationToken);
            return record;
        }

        public async Task<bool> UpsertProviderAsync(FeProvider provider, CancellationToken cancellationToken)
        {
            bool created;
            
            var existingProvider = await _dbContext.FeProviders
                .Where(x => x.Ukprn == provider.Ukprn)
                .SingleOrDefaultAsync(cancellationToken);
            if (existingProvider == null)
            {
                
                _dbContext.FeProviders.Add(provider);
                created = true;
            }
            else
            {
                existingProvider.UpdateFrom(provider);
                created = false;
            }
            
            await _dbContext.CommitAsync(cancellationToken);
            return created;
        }
    }
}