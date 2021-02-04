using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dfe.Edis.Kafka.Consumer;
using Dfe.FE.Interventions.Application.FeProviders;
using Dfe.FE.Interventions.Consumer.Ukrlp.Ukrlp;
using Dfe.FE.Interventions.Domain.Configuration;
using Dfe.FE.Interventions.Domain.FeProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dfe.FE.Interventions.Consumer.Ukrlp
{
    public class Worker : BackgroundService
    {
        private readonly IKafkaConsumer<string, Provider> _ukrlpConsumer;
        private readonly IFeProviderManager _providerManager;
        private readonly IMapper _mapper;
        private readonly DataServicesPlatformConfiguration _configuration;
        private readonly ILogger<Worker> _logger;

        public Worker(
            IKafkaConsumer<string, Provider> ukrlpConsumer,
            IFeProviderManager providerManager,
            IOptions<DataServicesPlatformConfiguration> options,
            IMapper mapper,
            ILogger<Worker> logger)
        {
            _ukrlpConsumer = ukrlpConsumer;
            _providerManager = providerManager;
            _mapper = mapper;
            _configuration = options.Value;
            _logger = logger;
            
            _ukrlpConsumer.SetMessageHandler(ProcessMessageFromTopic);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _ukrlpConsumer.RunAsync(_configuration.UkrlpTopicName, stoppingToken);
        }

        private async Task ProcessMessageFromTopic(ConsumedMessage<string, Provider> message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received update for provider {UKPRN} - {ProviderName} (topic: {Topic}, partition: {Partition}, offset: {Offset})", 
                message.Value.UnitedKingdomProviderReferenceNumber,
                message.Value.ProviderName,
                message.Topic,
                message.Partition,
                message.Offset);
            
            var feProvider = _mapper.Map<FeProvider>(message.Value);

            await _providerManager.UpsertProvider(feProvider, cancellationToken);
        }
    }
}