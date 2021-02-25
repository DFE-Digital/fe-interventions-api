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
    public class WhenGettingCountOfAimTypesDeliveredByProviderLocation
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
        public async Task ThenItShouldReturnCorrectCountOfAimsForProviderAndLocations()
        {
            var ukprn = 1234578;
            var postcode1 = "AA1 1AA";
            var postcode2 = "BB2 2BB";

            // Arrange
            _learners.Add(new Learner {Id = Guid.NewGuid(), Ukprn = ukprn});
            _learningDeliveries.Add(new LearningDelivery {LearnerId = _learners[0].Id, DeliveryLocationPostcode = postcode1, AimType = 1});
            _learningDeliveries.Add(new LearningDelivery {LearnerId = _learners[0].Id, DeliveryLocationPostcode = postcode1, AimType = 2});
            _learningDeliveries.Add(new LearningDelivery {LearnerId = _learners[0].Id, DeliveryLocationPostcode = null, AimType = null});

            _learners.Add(new Learner {Id = Guid.NewGuid(), Ukprn = ukprn});
            _learningDeliveries.Add(new LearningDelivery {LearnerId = _learners[1].Id, DeliveryLocationPostcode = postcode1, AimType = 1});
            _learningDeliveries.Add(new LearningDelivery {LearnerId = _learners[1].Id, DeliveryLocationPostcode = postcode2, AimType = 1});
            _learningDeliveries.Add(new LearningDelivery {LearnerId = _learners[1].Id, DeliveryLocationPostcode = postcode2, AimType = null});
            _learningDeliveries.Add(new LearningDelivery {LearnerId = _learners[1].Id, DeliveryLocationPostcode = null, AimType = 1});

            _learners.Add(new Learner {Id = Guid.NewGuid(), Ukprn = ukprn + 1000});
            _learningDeliveries.Add(new LearningDelivery {LearnerId = _learners[2].Id, DeliveryLocationPostcode = postcode2, AimType = 1});

            // Act
            var actual = await _repository.GetCountOfAimTypesDeliveredByProviderLocationAsync(
                ukprn,
                CancellationToken.None);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Count);
            Assert.IsTrue(actual.ContainsKey(postcode1));
            Assert.AreEqual(2, actual[postcode1]);
            Assert.IsTrue(actual.ContainsKey(postcode2));
            Assert.AreEqual(1, actual[postcode2]);
        }
    }
}