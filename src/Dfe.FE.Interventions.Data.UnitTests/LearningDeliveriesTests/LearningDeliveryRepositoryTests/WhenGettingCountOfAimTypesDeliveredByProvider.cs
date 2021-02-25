using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dfe.FE.Interventions.Data.LearningDeliveries;
using Dfe.FE.Interventions.Domain.Learners;
using Dfe.FE.Interventions.Domain.LearningDeliveries;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace Dfe.FE.Interventions.Data.UnitTests.LearningDeliveriesTests.LearningDeliveryRepositoryTests
{
    public class WhenGettingCountOfAimTypesDeliveredByProvider
    {
        
        private List<Learner> _learners;
        private List<LearningDelivery> _learningDeliveries;
        private Mock<IFeInterventionsDbContext> _dbContext;
        private LearningDeliveryRepository _repository;

        [SetUp]
        public void Arrange()
        {
            _learners = new List<Learner>();

            _learningDeliveries = new List<LearningDelivery>();

            _dbContext = new Mock<IFeInterventionsDbContext>();
            _dbContext.Setup(db => db.Learners)
                .Returns(_learners.AsQueryable().BuildMockDbSet().Object);
            _dbContext.Setup(db => db.LearningDeliveries)
                .Returns(_learningDeliveries.AsQueryable().BuildMockDbSet().Object);

            _repository = new LearningDeliveryRepository(_dbContext.Object);
        }

        [Test]
        public async Task ThenItShouldReturnCorrectCountOfAimTypesForProvider()
        {
            var ukprn = 1234578;

            // Arrange
            _learners.Add(new Learner {Id = Guid.NewGuid(), Ukprn = ukprn});
            _learningDeliveries.Add(new LearningDelivery {LearnerId = _learners[0].Id, AimType = 1});
            _learningDeliveries.Add(new LearningDelivery {LearnerId = _learners[0].Id, AimType = 2});

            _learners.Add(new Learner {Id = Guid.NewGuid(), Ukprn = ukprn});
            _learningDeliveries.Add(new LearningDelivery {LearnerId = _learners[1].Id, AimType = 1});
            _learningDeliveries.Add(new LearningDelivery {LearnerId = _learners[1].Id, AimType = 3});
            _learningDeliveries.Add(new LearningDelivery {LearnerId = _learners[1].Id, AimType = null});

            _learners.Add(new Learner {Id = Guid.NewGuid(), Ukprn = ukprn + 1000});
            _learningDeliveries.Add(new LearningDelivery {LearnerId = _learners[2].Id, AimType = 4});

            // Act
            var actual = await _repository.GetCountOfAimTypesDeliveredByProviderAsync(
                ukprn,
                CancellationToken.None);

            // Assert
            Assert.AreEqual(3, actual);
        }
    }
}