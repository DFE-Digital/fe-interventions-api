using System;
using AutoMapper;
using Dfe.FE.Interventions.Consumer.SubmitLearnerData.MappingProfiles;
using NUnit.Framework;

namespace Dfe.FE.Interventions.Consumer.SubmitLearnerData.UnitTests.MappingProfilesTests.SldLearningDeliveryMappingTests
{
    public class WhenMappingSldLearningDeliveryToDomainLearningDeliver
    {
        private IMapper _mapper;

        [SetUp]
        public void Arrange()
        {
            var cfg = new MapperConfiguration(configure => { configure.AddProfile<SldLearningDeliveryMapping>(); });
            _mapper = cfg.CreateMapper();
        }

        [Test]
        public void ThenItShouldMapAimTypeFromAimType()
        {
            var provider = MakeLearningDelivery();

            var actual = _mapper.Map<Domain.LearningDeliveries.LearningDelivery>(provider);

            Assert.AreEqual(provider.AimType, actual.AimType);
        }

        [Test]
        public void ThenItShouldMapStartDateFromLearnStartDate()
        {
            var provider = MakeLearningDelivery();

            var actual = _mapper.Map<Domain.LearningDeliveries.LearningDelivery>(provider);

            Assert.AreEqual(provider.LearnStartDate, actual.StartDate);
        }

        [Test]
        public void ThenItShouldMapPlannedEndDateFromLearnPlanEndDate()
        {
            var provider = MakeLearningDelivery();

            var actual = _mapper.Map<Domain.LearningDeliveries.LearningDelivery>(provider);

            Assert.AreEqual(provider.LearnPlanEndDate, actual.PlannedEndDate);
        }

        [Test]
        public void ThenItShouldMapActualEndDateFromLearnActEndDate()
        {
            var provider = MakeLearningDelivery();

            var actual = _mapper.Map<Domain.LearningDeliveries.LearningDelivery>(provider);

            Assert.AreEqual(provider.LearnActEndDate, actual.ActualEndDate);
        }

        [Test]
        public void ThenItShouldMapFundingModelFromFundModel()
        {
            var provider = MakeLearningDelivery();

            var actual = _mapper.Map<Domain.LearningDeliveries.LearningDelivery>(provider);

            Assert.AreEqual(provider.FundModel, actual.FundingModel);
        }

        [Test]
        public void ThenItShouldMapStandardCodeFromStdCode()
        {
            var provider = MakeLearningDelivery();

            var actual = _mapper.Map<Domain.LearningDeliveries.LearningDelivery>(provider);

            Assert.AreEqual(provider.StdCode, actual.StandardCode);
        }

        [Test]
        public void ThenItShouldMapCompletionStatusFromCompStatus()
        {
            var provider = MakeLearningDelivery();

            var actual = _mapper.Map<Domain.LearningDeliveries.LearningDelivery>(provider);

            Assert.AreEqual(provider.CompStatus, actual.CompletionStatus);
        }

        [Test]
        public void ThenItShouldMapOutcomeFromOutcome()
        {
            var provider = MakeLearningDelivery();

            var actual = _mapper.Map<Domain.LearningDeliveries.LearningDelivery>(provider);

            Assert.AreEqual(provider.Outcome, actual.Outcome);
        }

        [Test]
        public void ThenItShouldMapOutcomeGradeFromOutGrade()
        {
            var provider = MakeLearningDelivery();

            var actual = _mapper.Map<Domain.LearningDeliveries.LearningDelivery>(provider);

            Assert.AreEqual(provider.OutGrade, actual.OutcomeGrade);
        }

        [Test]
        public void ThenItShouldMapWithdrawalReasonFromWithdrawReason()
        {
            var provider = MakeLearningDelivery();

            var actual = _mapper.Map<Domain.LearningDeliveries.LearningDelivery>(provider);

            Assert.AreEqual(provider.WithdrawReason, actual.WithdrawalReason);
        }

        [Test]
        public void ThenItShouldMapDeliveryLocationPostcodeFromDelLocPostCode()
        {
            var provider = MakeLearningDelivery();

            var actual = _mapper.Map<Domain.LearningDeliveries.LearningDelivery>(provider);

            Assert.AreEqual(provider.DelLocPostCode, actual.DeliveryLocationPostcode);
        }

        private Sld.LearningDelivery MakeLearningDelivery()
        {
            var random = new Random();
            return new Sld.LearningDelivery
            {
                AimType = random.Next(1,99),
                LearnStartDate = new DateTime(random.Next(2019, 2021), random.Next(1,12), random.Next(1,28)),
                LearnPlanEndDate = new DateTime(random.Next(2022, 2024), random.Next(1,12), random.Next(1,28)),
                FundModel = random.Next(1,40),
                StdCode = random.Next(1,99),
                DelLocPostCode = Guid.NewGuid().ToString(),
                EpaOrgId = Guid.NewGuid().ToString(),
                CompStatus = random.Next(1,19),
                LearnActEndDate = new DateTime(random.Next(2022, 2024), random.Next(1,12), random.Next(1,28)),
                WithdrawReason = random.Next(1,19),
                Outcome = random.Next(1,19),
                AchDate = new DateTime(random.Next(2022, 2024), random.Next(1,12), random.Next(1,28)),
                OutGrade = Guid.NewGuid().ToString(),
                ProgType = random.Next(1,19),
            };
        }
    }
}