using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Dfe.FE.Interventions.Data.FeProviders;
using Dfe.FE.Interventions.Data.Learners;
using Dfe.FE.Interventions.Domain.FeProviders;
using Dfe.FE.Interventions.Domain.Learners;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace Dfe.FE.Interventions.Data.UnitTests.LearnersTests.LearnerRepositoryTests
{
    public class WhenUpsertingALearner
    {
        private List<Learner> _learners;
        private Mock<IFeInterventionsDbContext> _dbContext;
        private LearnerRepository _repository;

        [SetUp]
        public void Arrange()
        {
            _learners = new List<Learner>();

            _dbContext = new Mock<IFeInterventionsDbContext>();
            _dbContext.Setup(db => db.Learners)
                .Returns(_learners.AsQueryable().BuildMockDbSet().Object);

            _repository = new LearnerRepository(_dbContext.Object);
        }


        [Test, AutoData]
        public async Task AndAnExistingLearnerDoesNotExistWithUkprnAndLearnRefNumberThenItShouldCreateLearner(Learner learner)
        {
            var mockDbSet = _learners.AsQueryable().BuildMockDbSet();
            _dbContext.Setup(db => db.Learners)
                .Returns(mockDbSet.Object);
            var cancellationToken = new CancellationToken();

            var upsertResult = await _repository.UpsertLearnerAsync(learner, cancellationToken);

            Assert.IsNotNull(upsertResult);
            Assert.IsTrue(upsertResult.Created);
            Assert.AreNotEqual(Guid.Empty, upsertResult.Key);
            mockDbSet.Verify(dbSet => dbSet.Add(learner), Times.Once);
            _dbContext.Verify(context => context.CommitAsync(cancellationToken), Times.Once);
        }

        [Test, AutoData]
        public async Task AndAnExistingLearnerDoesExistWithUkprnAndLearnRefNumberThenItShouldUpdateLearner(Learner updatedLearner, Learner existingLearner)
        {
            existingLearner.Ukprn = updatedLearner.Ukprn;
            existingLearner.LearnRefNumber = updatedLearner.LearnRefNumber;
            _learners.Add(existingLearner);
            var mockDbSet = _learners.AsQueryable().BuildMockDbSet();
            _dbContext.Setup(db => db.Learners)
                .Returns(mockDbSet.Object);
            var cancellationToken = new CancellationToken();

            var upsertResult = await _repository.UpsertLearnerAsync(updatedLearner, cancellationToken);

            Assert.IsNotNull(upsertResult);
            Assert.IsFalse(upsertResult.Created);
            Assert.AreEqual(existingLearner.Id, upsertResult.Key);
            Assert.AreEqual(updatedLearner.Uln, existingLearner.Uln);
            Assert.AreEqual(updatedLearner.FirstNames, existingLearner.FirstNames);
            Assert.AreEqual(updatedLearner.LastName, existingLearner.LastName);
            Assert.AreEqual(updatedLearner.DateOfBirth, existingLearner.DateOfBirth);
            Assert.AreEqual(updatedLearner.NationalInsuranceNumber, existingLearner.NationalInsuranceNumber);
            _dbContext.Verify(context => context.CommitAsync(cancellationToken), Times.Once);
        }
    }
}