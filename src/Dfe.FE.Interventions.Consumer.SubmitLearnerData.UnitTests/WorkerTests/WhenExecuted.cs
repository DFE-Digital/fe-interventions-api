using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dfe.Edis.Kafka.Consumer;
using Dfe.FE.Interventions.Application.Learners;
using Dfe.FE.Interventions.Application.LearningDeliveries;
using Dfe.FE.Interventions.Domain.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Dfe.FE.Interventions.Consumer.SubmitLearnerData.UnitTests.WorkerTests
{
    public class WhenExecuted
    {
        private Mock<IKafkaConsumer<string, Sld.Learner>> _sldConsumerMock;
        private Mock<ILearnerManager> _learnerManagerMock;
        private Mock<ILearningDeliveryManager> _learningDeliveryManagerMock;
        private IOptions<DataServicesPlatformConfiguration> _options;
        private Mock<IMapper> _mapperMock;
        private Mock<ILogger<Worker>> _loggerMock;
        private Worker _worker;

        [SetUp]
        public void Arrange()
        {
            _sldConsumerMock = new Mock<IKafkaConsumer<string, Sld.Learner>>();

            _learnerManagerMock = new Mock<ILearnerManager>();
            _learnerManagerMock.Setup(m => m.UpsertLearner(It.IsAny<Domain.Learners.Learner>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());

            _learningDeliveryManagerMock = new Mock<ILearningDeliveryManager>();

            _options = new OptionsWrapper<DataServicesPlatformConfiguration>(
                new DataServicesPlatformConfiguration
                {
                    KafkaBrokers = "localhost:9092",
                    SubmitLearnerDataGroupId = "group-one",
                    SubmitLearnerDataTopicName = "topic-name",
                });

            _mapperMock = new Mock<IMapper>();
            _mapperMock.Setup(m => m.Map<Domain.Learners.Learner>(It.IsAny<Sld.Learner>()))
                .Returns(new Domain.Learners.Learner());
            _mapperMock.Setup(m => m.Map<Domain.LearningDeliveries.LearningDelivery>(It.IsAny<Sld.LearningDelivery>()))
                .Returns(new Domain.LearningDeliveries.LearningDelivery());

            _loggerMock = new Mock<ILogger<Worker>>();

            _worker = new Worker(
                _sldConsumerMock.Object,
                _learnerManagerMock.Object,
                _learningDeliveryManagerMock.Object,
                _options,
                _mapperMock.Object,
                _loggerMock.Object);
        }

        [Test]
        public void ThenItShouldSetMessageHandler()
        {
            // Act is construction in main setup

            _sldConsumerMock.Verify(c => c.SetMessageHandler(It.IsAny<Func<ConsumedMessage<string, Sld.Learner>, CancellationToken, Task>>()),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldRunConsumer()
        {
            await _worker.StartAsync(CancellationToken.None);

            _sldConsumerMock.Verify(c => c.RunAsync(_options.Value.SubmitLearnerDataTopicName, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldMapLearner()
        {
            var cancellationToken = new CancellationToken();
            var message = new ConsumedMessage<string, Sld.Learner>
            {
                Topic = "topic-name",
                Partition = 91,
                Offset = 28379,
                Key = "12345678-123",
                Value = new Sld.Learner(),
            };

            var expectedLearner = new Domain.Learners.Learner();
            _mapperMock.Setup(m => m.Map<Domain.Learners.Learner>(It.IsAny<Sld.Learner>()))
                .Returns(expectedLearner);
            Func<ConsumedMessage<string, Sld.Learner>, CancellationToken, Task> messageHandler = null;
            _sldConsumerMock.Setup(c => c.SetMessageHandler(It.IsAny<Func<ConsumedMessage<string, Sld.Learner>, CancellationToken, Task>>()))
                .Callback((Func<ConsumedMessage<string, Sld.Learner>, CancellationToken, Task> handler) => { messageHandler = handler; });
            _worker = new Worker(
                _sldConsumerMock.Object,
                _learnerManagerMock.Object,
                _learningDeliveryManagerMock.Object,
                _options,
                _mapperMock.Object,
                _loggerMock.Object);


            var workerTask = _worker.StartAsync(CancellationToken.None);
            await messageHandler.Invoke(message, cancellationToken);
            await workerTask;

            _mapperMock.Verify(m => m.Map<Domain.Learners.Learner>(message.Value),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldUpsertLearner()
        {
            var cancellationToken = new CancellationToken();
            var message = new ConsumedMessage<string, Sld.Learner>
            {
                Topic = "topic-name",
                Partition = 91,
                Offset = 28379,
                Key = "12345678-123",
                Value = new Sld.Learner(),
            };

            var expectedLearner = new Domain.Learners.Learner();
            _mapperMock.Setup(m => m.Map<Domain.Learners.Learner>(It.IsAny<Sld.Learner>()))
                .Returns(expectedLearner);
            Func<ConsumedMessage<string, Sld.Learner>, CancellationToken, Task> messageHandler = null;
            _sldConsumerMock.Setup(c => c.SetMessageHandler(It.IsAny<Func<ConsumedMessage<string, Sld.Learner>, CancellationToken, Task>>()))
                .Callback((Func<ConsumedMessage<string, Sld.Learner>, CancellationToken, Task> handler) => { messageHandler = handler; });
            _worker = new Worker(
                _sldConsumerMock.Object,
                _learnerManagerMock.Object,
                _learningDeliveryManagerMock.Object,
                _options,
                _mapperMock.Object,
                _loggerMock.Object);


            var workerTask = _worker.StartAsync(CancellationToken.None);
            await messageHandler.Invoke(message, cancellationToken);
            await workerTask;

            _learnerManagerMock.Verify(m => m.UpsertLearner(expectedLearner, cancellationToken));
        }

        [Test]
        public async Task ThenItShouldMapLearningDeliveries()
        {
            var cancellationToken = new CancellationToken();
            var message = new ConsumedMessage<string, Sld.Learner>
            {
                Topic = "topic-name",
                Partition = 91,
                Offset = 28379,
                Key = "12345678-123",
                Value = new Sld.Learner
                {
                    LearningDeliveries = new Sld.LearningDelivery[3]
                },
            };

            var expectedLearningDeliveries = Enumerable.Range(1, 3).Select(x => new Domain.LearningDeliveries.LearningDelivery()).ToArray();
            _mapperMock.Setup(m => m.Map<Domain.LearningDeliveries.LearningDelivery[]>(It.IsAny<Sld.LearningDelivery[]>()))
                .Returns(expectedLearningDeliveries);
            Func<ConsumedMessage<string, Sld.Learner>, CancellationToken, Task> messageHandler = null;
            _sldConsumerMock.Setup(c => c.SetMessageHandler(It.IsAny<Func<ConsumedMessage<string, Sld.Learner>, CancellationToken, Task>>()))
                .Callback((Func<ConsumedMessage<string, Sld.Learner>, CancellationToken, Task> handler) => { messageHandler = handler; });
            _worker = new Worker(
                _sldConsumerMock.Object,
                _learnerManagerMock.Object,
                _learningDeliveryManagerMock.Object,
                _options,
                _mapperMock.Object,
                _loggerMock.Object);


            var workerTask = _worker.StartAsync(CancellationToken.None);
            await messageHandler.Invoke(message, cancellationToken);
            await workerTask;

            _mapperMock.Verify(m => m.Map<Domain.LearningDeliveries.LearningDelivery[]>(message.Value.LearningDeliveries),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldUpdateLearnersLearningDeliveriesAfterSettingADeliverIdAndLearnerId()
        {
            var cancellationToken = new CancellationToken();
            var message = new ConsumedMessage<string, Sld.Learner>
            {
                Topic = "topic-name",
                Partition = 91,
                Offset = 28379,
                Key = "12345678-123",
                Value = new Sld.Learner
                {
                    LearningDeliveries = new Sld.LearningDelivery[3]
                },
            };
            var learnerId = Guid.NewGuid();
            var expectedLearningDeliveries = Enumerable.Range(1, 3).Select(x => new Domain.LearningDeliveries.LearningDelivery()).ToArray();

            _learnerManagerMock.Setup(m => m.UpsertLearner(It.IsAny<Domain.Learners.Learner>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(learnerId);
            _mapperMock.Setup(m => m.Map<Domain.LearningDeliveries.LearningDelivery[]>(It.IsAny<Sld.LearningDelivery[]>()))
                .Returns(expectedLearningDeliveries);
            Func<ConsumedMessage<string, Sld.Learner>, CancellationToken, Task> messageHandler = null;
            _sldConsumerMock.Setup(c => c.SetMessageHandler(It.IsAny<Func<ConsumedMessage<string, Sld.Learner>, CancellationToken, Task>>()))
                .Callback((Func<ConsumedMessage<string, Sld.Learner>, CancellationToken, Task> handler) => { messageHandler = handler; });
            _worker = new Worker(
                _sldConsumerMock.Object,
                _learnerManagerMock.Object,
                _learningDeliveryManagerMock.Object,
                _options,
                _mapperMock.Object,
                _loggerMock.Object);


            var workerTask = _worker.StartAsync(CancellationToken.None);
            await messageHandler.Invoke(message, cancellationToken);
            await workerTask;

            _learningDeliveryManagerMock.Verify(m => m.UpdateLearnersLearningDeliveriesAsync(
                    It.IsAny<Domain.LearningDeliveries.LearningDelivery[]>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            _learningDeliveryManagerMock.Verify(m => m.UpdateLearnersLearningDeliveriesAsync(
                    It.Is<Domain.LearningDeliveries.LearningDelivery[]>(ld => ld.Length == 3),
                    cancellationToken),
                Times.Once);
            _learningDeliveryManagerMock.Verify(m => m.UpdateLearnersLearningDeliveriesAsync(
                    It.Is<Domain.LearningDeliveries.LearningDelivery[]>(ld => ld[0].LearnerId == learnerId && ld[0].Id != Guid.Empty),
                    cancellationToken),
                Times.Once);
            _learningDeliveryManagerMock.Verify(m => m.UpdateLearnersLearningDeliveriesAsync(
                    It.Is<Domain.LearningDeliveries.LearningDelivery[]>(ld => ld[1].LearnerId == learnerId && ld[1].Id != Guid.Empty),
                    cancellationToken),
                Times.Once);
            _learningDeliveryManagerMock.Verify(m => m.UpdateLearnersLearningDeliveriesAsync(
                    It.Is<Domain.LearningDeliveries.LearningDelivery[]>(ld => ld[2].LearnerId == learnerId && ld[2].Id != Guid.Empty),
                    cancellationToken),
                Times.Once);
        }
    }
}