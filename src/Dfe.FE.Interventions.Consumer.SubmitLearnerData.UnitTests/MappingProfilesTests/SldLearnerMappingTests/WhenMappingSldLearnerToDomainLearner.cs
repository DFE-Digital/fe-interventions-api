using System;
using AutoMapper;
using Dfe.FE.Interventions.Consumer.SubmitLearnerData.MappingProfiles;
using NUnit.Framework;

namespace Dfe.FE.Interventions.Consumer.SubmitLearnerData.UnitTests.MappingProfilesTests.SldLearnerMappingTests
{
    public class WhenMappingSldLearnerToDomainLearner
    {
        private IMapper _mapper;

        [SetUp]
        public void Arrange()
        {
            var cfg = new MapperConfiguration(configure => { configure.AddProfile<SldLearnerMapping>(); });
            _mapper = cfg.CreateMapper();
        }

        [Test]
        public void ThenItShouldMapUkprnFromUkprn()
        {
            var provider = MakeLearner();

            var actual = _mapper.Map<Domain.Learners.Learner>(provider);

            Assert.AreEqual(provider.Ukprn, actual.Ukprn);
        }

        [Test]
        public void ThenItShouldMapLearnRefNumberFromLearnRefNumber()
        {
            var provider = MakeLearner();

            var actual = _mapper.Map<Domain.Learners.Learner>(provider);

            Assert.AreEqual(provider.LearnRefNumber, actual.LearnRefNumber);
        }

        [Test]
        public void ThenItShouldMapUlnFromUln()
        {
            var provider = MakeLearner();

            var actual = _mapper.Map<Domain.Learners.Learner>(provider);

            Assert.AreEqual(provider.Uln, actual.Uln);
        }

        [Test]
        public void ThenItShouldMapFirstNamesFromGivenNames()
        {
            var provider = MakeLearner();

            var actual = _mapper.Map<Domain.Learners.Learner>(provider);

            Assert.AreEqual(provider.GivenNames, actual.FirstNames);
        }

        [Test]
        public void ThenItShouldMapLastNameFromFamilyName()
        {
            var provider = MakeLearner();

            var actual = _mapper.Map<Domain.Learners.Learner>(provider);

            Assert.AreEqual(provider.FamilyName, actual.LastName);
        }

        [Test]
        public void ThenItShouldMapDateOfBirthFromDateOfBirth()
        {
            var provider = MakeLearner();

            var actual = _mapper.Map<Domain.Learners.Learner>(provider);

            Assert.AreEqual(provider.DateOfBirth, actual.DateOfBirth);
        }

        [Test]
        public void ThenItShouldMapNationalInsuranceNumberFromNiNumber()
        {
            var provider = MakeLearner();

            var actual = _mapper.Map<Domain.Learners.Learner>(provider);

            Assert.AreEqual(provider.NiNumber, actual.NationalInsuranceNumber);
        }


        private Sld.Learner MakeLearner()
        {
            var random = new Random();
            return new Sld.Learner
            {
                Ukprn = random.Next(10000000, 99999999),
                LearnRefNumber = Guid.NewGuid().ToString(),
                Uln = random.Next(123456789, 987654321),
                FamilyName = Guid.NewGuid().ToString(),
                GivenNames = Guid.NewGuid().ToString(),
                DateOfBirth = new DateTime(random.Next(1995, 2005), random.Next(1, 12), random.Next(1, 28)),
                NiNumber = $"AB{random.Next(100000, 999999)}A",
                LearningDeliveries = new Sld.LearningDelivery[0],
            };
        }
    }
}