using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dfe.FE.Interventions.Data.Learners;
using Dfe.FE.Interventions.Domain.Learners;
using Dfe.FE.Interventions.Domain.LearningDeliveries;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace Dfe.FE.Interventions.Data.UnitTests.LearnersTests.LearnerRepositoryTests
{
    public class WhenGettingCountOfContinuingLearnersAtProviderWithFundingModels
    {
        private List<Learner> _learners;
        private List<LearningDelivery> _learningDeliveries;
        private Mock<IFeInterventionsDbContext> _dbContext;
        private LearnerRepository _repository;

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

            _repository = new LearnerRepository(_dbContext.Object);
        }

        [Test]
        public async Task ThenItShouldReturnCorrectCountOfLearnersForProviderAndFundingModel()
        {
            var ukprn = 1234578;
            var fundingModel1 = 36;
            var fundingModel2 = 37;

            // Arrange
            _learners.Add(new Learner {Id = Guid.NewGuid(), Ukprn = ukprn});
            _learningDeliveries.Add(new LearningDelivery {LearnerId = _learners[0].Id, FundingModel = fundingModel1, CompletionStatus = 1}); // match
            _learningDeliveries.Add(new LearningDelivery {LearnerId = _learners[0].Id, FundingModel = fundingModel2, CompletionStatus = 1}); // match
            _learningDeliveries.Add(new LearningDelivery {LearnerId = _learners[0].Id, FundingModel = fundingModel2, CompletionStatus = 2});

            _learners.Add(new Learner {Id = Guid.NewGuid(), Ukprn = ukprn});
            _learningDeliveries.Add(new LearningDelivery {LearnerId = _learners[1].Id, FundingModel = fundingModel1, CompletionStatus = 1}); // match
            _learningDeliveries.Add(new LearningDelivery {LearnerId = _learners[1].Id, FundingModel = fundingModel2, CompletionStatus = 2});

            _learners.Add(new Learner {Id = Guid.NewGuid(), Ukprn = ukprn + 1000});
            _learningDeliveries.Add(new LearningDelivery {LearnerId = _learners[2].Id, FundingModel = fundingModel1, CompletionStatus = 1});

            // Act
            var actual = await _repository.GetCountOfContinuingLearnersAtProviderWithFundingModelsAsync(
                ukprn,
                new[] {fundingModel1, fundingModel2},
                CancellationToken.None);

            // Assert
            Assert.AreEqual(2, actual);
        }
    }
    public class WhenGettingCountOfContinuingLearnersAtProvider
    {
        private List<Learner> _learners;
        private List<LearningDelivery> _learningDeliveries;
        private Mock<IFeInterventionsDbContext> _dbContext;
        private LearnerRepository _repository;

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

            _repository = new LearnerRepository(_dbContext.Object);
        }

        [Test]
        public async Task ThenItShouldReturnCorrectCountOfLearnersForProvider()
        {
            var ukprn = 1234578;
            var fundingModel1 = 36;
            var fundingModel2 = 37;

            // Arrange
            _learners.Add(new Learner {Id = Guid.NewGuid(), Ukprn = ukprn});
            _learningDeliveries.Add(new LearningDelivery {LearnerId = _learners[0].Id, FundingModel = fundingModel1, CompletionStatus = 1}); // match
            _learningDeliveries.Add(new LearningDelivery {LearnerId = _learners[0].Id, FundingModel = fundingModel2, CompletionStatus = 1}); // match
            _learningDeliveries.Add(new LearningDelivery {LearnerId = _learners[0].Id, FundingModel = fundingModel2, CompletionStatus = 2});

            _learners.Add(new Learner {Id = Guid.NewGuid(), Ukprn = ukprn});
            _learningDeliveries.Add(new LearningDelivery {LearnerId = _learners[1].Id, FundingModel = fundingModel1, CompletionStatus = 1}); // match
            _learningDeliveries.Add(new LearningDelivery {LearnerId = _learners[1].Id, FundingModel = fundingModel2, CompletionStatus = 2});

            _learners.Add(new Learner {Id = Guid.NewGuid(), Ukprn = ukprn + 1000});
            _learningDeliveries.Add(new LearningDelivery {LearnerId = _learners[2].Id, FundingModel = fundingModel1, CompletionStatus = 1});

            // Act
            var actual = await _repository.GetCountOfContinuingLearnersAtProviderAsync(
                ukprn,
                CancellationToken.None);

            // Assert
            Assert.AreEqual(2, actual);
        }
    }
}