using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Dfe.FE.Interventions.Data.Learners;
using Dfe.FE.Interventions.Data.LearningDeliveries;
using Dfe.FE.Interventions.Domain.Learners;
using Dfe.FE.Interventions.Domain.LearningDeliveries;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace Dfe.FE.Interventions.Data.UnitTests.LearningDeliveriesTests.LearningDeliveryRepositoryTests
{
    public class WhenReplacingAllLearningDeliveriesForLearner
    {
        private List<LearningDelivery> _learningDeliveries;
        private Mock<IFeInterventionsDbContext> _dbContext;
        private LearningDeliveryRepository _repository;

        [SetUp]
        public void Arrange()
        {
            _learningDeliveries = new List<LearningDelivery>();

            _dbContext = new Mock<IFeInterventionsDbContext>();
            _dbContext.Setup(db => db.LearningDeliveries)
                .Returns(_learningDeliveries.AsQueryable().BuildMockDbSet().Object);

            _repository = new LearningDeliveryRepository(_dbContext.Object);
        }

        [Test, AutoData]
        public async Task ThenItShouldDeleteLearningDeliveriesForLearner(Guid learnerId)
        {
            var cancellationToken = new CancellationToken();

            await _repository.ReplaceAllLearningDeliveriesForLearnerAsync(learnerId, new LearningDelivery[0], cancellationToken);

            _dbContext.Verify(db => db.ExecuteSqlAsync(
                    "DELETE FROM LearningDelivery WHERE LearnerId = {0}",
                    It.Is<object[]>(p => p.Length == 1 && p[0].Equals(learnerId)),
                    cancellationToken),
                Times.Once);
        }

        [Test, AutoData]
        public async Task ThenItShouldAddAllLearningDeliveries(LearningDelivery[] deliveries)
        {
            var mockDbSet = _learningDeliveries.AsQueryable().BuildMockDbSet();
            _dbContext.Setup(db => db.LearningDeliveries)
                .Returns(mockDbSet.Object);
            var cancellationToken = new CancellationToken();

            await _repository.ReplaceAllLearningDeliveriesForLearnerAsync(Guid.NewGuid(), deliveries, cancellationToken);
            
            mockDbSet.Verify(dbSet =>dbSet.AddRangeAsync(deliveries, cancellationToken),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldCommitChanges()
        {
            var cancellationToken = new CancellationToken();
            
            await _repository.ReplaceAllLearningDeliveriesForLearnerAsync(Guid.NewGuid(), new LearningDelivery[0], cancellationToken);
            
            _dbContext.Verify(context => context.CommitAsync(cancellationToken), Times.Once);
        }
    }
}