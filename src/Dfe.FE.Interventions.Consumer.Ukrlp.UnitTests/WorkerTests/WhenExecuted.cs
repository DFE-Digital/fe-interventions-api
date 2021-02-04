using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dfe.Edis.Kafka.Consumer;
using Dfe.FE.Interventions.Application.FeProviders;
using Dfe.FE.Interventions.Consumer.Ukrlp.Ukrlp;
using Dfe.FE.Interventions.Domain.Configuration;
using Dfe.FE.Interventions.Domain.FeProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Dfe.FE.Interventions.Consumer.Ukrlp.UnitTests.WorkerTests
{
    public class WhenExecuted
    {
        private Mock<IKafkaConsumer<string, Provider>> _ukrlpConsumerMock;
        private Mock<IFeProviderManager> _providerManagerMock;
        private IOptions<DataServicesPlatformConfiguration> _options;
        private Mock<IMapper> _mapperMock;
        private Mock<ILogger<Worker>> _loggerMock;
        private Worker _worker;

        [SetUp]
        public void Arrange()
        {
            _ukrlpConsumerMock = new Mock<IKafkaConsumer<string, Provider>>();

            _providerManagerMock = new Mock<IFeProviderManager>();

            _options = new OptionsWrapper<DataServicesPlatformConfiguration>(
                new DataServicesPlatformConfiguration
                {
                    KafkaBrokers = "localhost:9092",
                    UkrlpGroupId = "group-one",
                    UkrlpTopicName = "topic-name",
                });

            _mapperMock = new Mock<IMapper>();
            _mapperMock.Setup(m => m.Map<FeProvider>(It.IsAny<Provider>()))
                .Returns(new FeProvider());

            _loggerMock = new Mock<ILogger<Worker>>();

            _worker = new Worker(
                _ukrlpConsumerMock.Object,
                _providerManagerMock.Object,
                _options,
                _mapperMock.Object,
                _loggerMock.Object);
        }

        [Test]
        public void ThenItShouldSetMessageHandler()
        {
            // Act is construction in main setup

            _ukrlpConsumerMock.Verify(c => c.SetMessageHandler(It.IsAny<Func<ConsumedMessage<string, Provider>, CancellationToken, Task>>()),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldRunConsumer()
        {
            await _worker.StartAsync(CancellationToken.None);

            _ukrlpConsumerMock.Verify(c => c.RunAsync(_options.Value.UkrlpTopicName, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldMapProvider()
        {
            var cancellationToken = new CancellationToken();
            var message = new ConsumedMessage<string, Provider>
            {
                Topic = "topic-name",
                Partition = 91,
                Offset = 28379,
                Key = "12345678",
                Value = new Provider(),
            };

            var expectedFeProvider = new FeProvider();
            _mapperMock.Setup(m => m.Map<FeProvider>(It.IsAny<Provider>()))
                .Returns(expectedFeProvider);
            Func<ConsumedMessage<string, Provider>, CancellationToken, Task> messageHandler = null;
            _ukrlpConsumerMock.Setup(c => c.SetMessageHandler(It.IsAny<Func<ConsumedMessage<string, Provider>, CancellationToken, Task>>()))
                .Callback((Func<ConsumedMessage<string, Provider>, CancellationToken, Task> handler) => { messageHandler = handler; });
            _worker = new Worker(
                _ukrlpConsumerMock.Object,
                _providerManagerMock.Object,
                _options,
                _mapperMock.Object,
                _loggerMock.Object);


            var workerTask = _worker.StartAsync(CancellationToken.None);
            await messageHandler.Invoke(message, cancellationToken);
            await workerTask;

            _mapperMock.Verify(m => m.Map<FeProvider>(message.Value),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldUpsertProviderWhenMessageReceived()
        {
            var cancellationToken = new CancellationToken();
            var message = new ConsumedMessage<string, Provider>
            {
                Topic = "topic-name",
                Partition = 91,
                Offset = 28379,
                Key = "12345678",
                Value = new Provider(),
            };

            var expectedFeProvider = new FeProvider();
            _mapperMock.Setup(m => m.Map<FeProvider>(It.IsAny<Provider>()))
                .Returns(expectedFeProvider);
            Func<ConsumedMessage<string, Provider>, CancellationToken, Task> messageHandler = null;
            _ukrlpConsumerMock.Setup(c => c.SetMessageHandler(It.IsAny<Func<ConsumedMessage<string, Provider>, CancellationToken, Task>>()))
                .Callback((Func<ConsumedMessage<string, Provider>, CancellationToken, Task> handler) => { messageHandler = handler; });
            _worker = new Worker(
                _ukrlpConsumerMock.Object,
                _providerManagerMock.Object,
                _options,
                _mapperMock.Object,
                _loggerMock.Object);


            var workerTask = _worker.StartAsync(CancellationToken.None);
            await messageHandler.Invoke(message, cancellationToken);
            await workerTask;

            _providerManagerMock.Verify(m => m.UpsertProvider(expectedFeProvider, cancellationToken));
        }
    }
}