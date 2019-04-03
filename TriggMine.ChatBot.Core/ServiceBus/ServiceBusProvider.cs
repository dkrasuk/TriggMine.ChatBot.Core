using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TriggMine.ChatBot.Core.ServiceBus
{
    public class ServiceBusProvider : IServiceBusProvider
    {
        private readonly ILogger<ServiceBusProvider> _logger;
        private readonly string _serviceBusConnectionString;
        private readonly string _serviceBusQueueName;
        private readonly IQueueClient _queueClient;

        public ServiceBusProvider(IConfiguration configuration, ILogger<ServiceBusProvider> logger)
        {
            _logger = logger;
            _serviceBusConnectionString = configuration["ServiceBusConnectionString"];
            _serviceBusQueueName = configuration["ServiceBusQueueName"];
            _queueClient = _queueClient = new QueueClient(_serviceBusConnectionString, _serviceBusQueueName);
        }

        public async Task SendMessageToServiceBus(string message)
        {
            try
            {               
                var messageBody = new Message(Encoding.UTF8.GetBytes(message));
                await _queueClient.SendAsync(messageBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }            
        }

        public void RegisterOnMessageHandlerAndReceiveMessages()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                AutoComplete = false,
                MaxConcurrentCalls = 1
            };
            _queueClient.RegisterMessageHandler(ReadMessagesHandlerAsync, messageHandlerOptions);
        }

        private async Task ReadMessagesHandlerAsync(Message message, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

            await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            _logger.LogError(exceptionReceivedEventArgs.Exception.Message);
            return Task.CompletedTask;
        }
    }
}
