using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dfe.Edis.Kafka.Consumer;
using Dfe.FE.Interventions.Application.Learners;
using Dfe.FE.Interventions.Application.LearningDeliveries;
using Dfe.FE.Interventions.Domain.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dfe.FE.Interventions.Consumer.SubmitLearnerData
{
    public class Worker : BackgroundService
    {
        private readonly IKafkaConsumer<string, Sld.Learner> _sldConsumer;
        private readonly ILearnerManager _learnerManager;
        private readonly ILearningDeliveryManager _learningDeliveryManager;
        private readonly IMapper _mapper;
        private readonly DataServicesPlatformConfiguration _configuration;
        private readonly ILogger<Worker> _logger;

        public Worker(
            IKafkaConsumer<string, Sld.Learner> sldConsumer,
            ILearnerManager learnerManager,
            ILearningDeliveryManager learningDeliveryManager,
            IOptions<DataServicesPlatformConfiguration> options,
            IMapper mapper,
            ILogger<Worker> logger)
        {
            _sldConsumer = sldConsumer;
            _learnerManager = learnerManager;
            _learningDeliveryManager = learningDeliveryManager;
            _mapper = mapper;
            _configuration = options.Value;
            _logger = logger;
            
            _sldConsumer.SetMessageHandler(ProcessMessageFromTopic);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _sldConsumer.RunAsync(_configuration.SubmitLearnerDataTopicName, stoppingToken);
        }

        private async Task ProcessMessageFromTopic(ConsumedMessage<string, Sld.Learner> message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received update for learner {LearnRefNumber} at provider {UKPRN} (topic: {Topic}, partition: {Partition}, offset: {Offset})", 
                message.Value.LearnRefNumber,
                message.Value.Ukprn,
                message.Topic,
                message.Partition,
                message.Offset);

            var learner = _mapper.Map<Domain.Learners.Learner>(message.Value);
            var learnerId = await _learnerManager.UpsertLearner(learner, cancellationToken);

            var learningDeliveries = _mapper.Map<Domain.LearningDeliveries.LearningDelivery[]>(message.Value.LearningDeliveries ?? new Sld.LearningDelivery[0]);
            foreach (var learningDelivery in learningDeliveries)
            {
                learningDelivery.Id = Guid.NewGuid();
                learningDelivery.LearnerId = learnerId;
            }
            await _learningDeliveryManager.UpdateLearnersLearningDeliveriesAsync(learningDeliveries, cancellationToken);
        }
    }
}