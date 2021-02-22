using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Dfe.FE.Interventions.Data.LearningDeliveries;
using Dfe.FE.Interventions.Domain.Learners;
using Dfe.FE.Interventions.Domain.LearningDeliveries;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace Dfe.FE.Interventions.Data.UnitTests.LearningDeliveriesTests.LearningDeliveryRepositoryTests
{
    public class WhenListingForProvider
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

        [Test, AutoData]
        public async Task ThenItShouldReturnMatchingLearningDeliveriesForUkprn(int ukprn)
        {
            _learners.Add(new Learner {Id = Guid.NewGuid(), Ukprn = ukprn});
            _learners.Add(new Learner {Id = Guid.NewGuid(), Ukprn = ukprn + 12});
            _learningDeliveries.Add(new LearningDelivery
                {Id = Guid.NewGuid(), LearnerId = _learners[0].Id, FundingModel = 35, DeliveryLocationPostcode = "AB12 3DE", ProgrammeType = 45});
            _learningDeliveries.Add(new LearningDelivery
                {Id = Guid.NewGuid(), LearnerId = _learners[0].Id, FundingModel = 46, DeliveryLocationPostcode = "ZZ65 3ZZ", ProgrammeType = 12});
            _learningDeliveries.Add(new LearningDelivery
                {Id = Guid.NewGuid(), LearnerId = _learners[1].Id, FundingModel = 85, DeliveryLocationPostcode = "AK1 9HA", ProgrammeType = 95});

            var actual = await _repository.ListForProviderAsync(ukprn, 1, 12, CancellationToken.None);

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Results);
            Assert.AreEqual(2, actual.Results.Length);
            Assert.AreEqual(_learningDeliveries[0].Id, actual.Results[0].Id);
            Assert.AreEqual(_learningDeliveries[0].FundingModel, actual.Results[0].FundingModel);
            Assert.AreEqual(_learningDeliveries[0].DeliveryLocationPostcode, actual.Results[0].DeliveryLocationPostcode);
            Assert.AreEqual(_learningDeliveries[0].ProgrammeType, actual.Results[0].ProgrammeType);
            Assert.AreEqual(_learningDeliveries[1].Id, actual.Results[1].Id);
            Assert.AreEqual(_learningDeliveries[1].FundingModel, actual.Results[1].FundingModel);
            Assert.AreEqual(_learningDeliveries[1].DeliveryLocationPostcode, actual.Results[1].DeliveryLocationPostcode);
            Assert.AreEqual(_learningDeliveries[1].ProgrammeType, actual.Results[1].ProgrammeType);
        }

        [Test, AutoData]
        public async Task ThenItShouldReturnPaginationInformation(int ukprn)
        {
            _learners.Add(new Learner {Id = Guid.NewGuid(), Ukprn = ukprn});
            for (var i = 0; i < 20; i++)
            {
                _learningDeliveries.Add(new LearningDelivery
                {
                    Id = Guid.NewGuid(),
                    LearnerId = _learners[0].Id,
                    FundingModel = 10 + i,
                    DeliveryLocationPostcode = "AB12 3DE",
                    ProgrammeType = 50 + i
                });
            }

            var actual = await _repository.ListForProviderAsync(ukprn, 1, 12, CancellationToken.None);

            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.CurrentPage);
            Assert.AreEqual(2, actual.TotalNumberOfPages);
            Assert.AreEqual(20, actual.TotalNumberOfRecords);
            Assert.AreEqual(1, actual.PageStartIndex);
            Assert.AreEqual(12, actual.PageFinishIndex);
        }

        [TestCase(25, 1, 12, 1, 12)]
        [TestCase(25, 2, 12, 13, 24)]
        [TestCase(25, 3, 12, 25, 25)]
        public async Task ThenItShouldReturnCorrectPageOfInformation(int numberOfRecords, int pageNumber, int pageSize, int expectedStart, int expectedEnd)
        {
            var ukprn = 10000001;
            _learners.Add(new Learner {Id = Guid.NewGuid(), Ukprn = ukprn});
            for (var i = 0; i < numberOfRecords; i++)
            {
                _learningDeliveries.Add(new LearningDelivery
                {
                    Id = Guid.NewGuid(),
                    LearnerId = _learners[0].Id,
                    FundingModel = 10 + i,
                    DeliveryLocationPostcode = "AB12 3DE",
                    ProgrammeType = 50 + i
                });
            }

            var actual = await _repository.ListForProviderAsync(ukprn, pageNumber, pageSize, CancellationToken.None);

            var expectedResults = _learningDeliveries.Skip(expectedStart - 1).Take(expectedEnd - expectedStart + 1).ToArray();
            Assert.IsNotNull(actual);
            Assert.AreEqual(expectedResults.Length, actual.Results.Length);
            for (var i = 0; i < expectedResults.Length; i++)
            {
                Assert.IsNotNull(actual.Results.SingleOrDefault(x => x.Id == expectedResults[i].Id),
                    $"Expected results to contain {expectedResults[i].Id} but was missing");
            }

            Assert.AreEqual(pageNumber, actual.CurrentPage);
            Assert.AreEqual((int) Math.Ceiling((float) numberOfRecords / pageSize), actual.TotalNumberOfPages);
            Assert.AreEqual(numberOfRecords, actual.TotalNumberOfRecords);
            Assert.AreEqual(expectedStart, actual.PageStartIndex);
            Assert.AreEqual(expectedEnd, actual.PageFinishIndex);
        }
    }
}