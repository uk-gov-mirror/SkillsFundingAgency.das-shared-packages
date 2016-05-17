﻿using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace SFA.DAS.Messaging.AzureServiceBus
{
    public class AzureServiceBusMessageSubSystem : IMessageSubSystem
    {
        private readonly string _connectionString;
        private readonly string _queueName;

        public AzureServiceBusMessageSubSystem(string connectionString, string queueName)
        {
            _connectionString = connectionString;
            _queueName = queueName;
        }

        public async Task Enqueue(string message)
        {
            var client = QueueClient.CreateFromConnectionString(_connectionString, _queueName);
            var brokeredMessage = new BrokeredMessage(message)
            {
                MessageId = Guid.NewGuid().ToString()
            };

            await client.SendAsync(brokeredMessage);
        }

        public async Task<SubSystemMessage> Dequeue()
        {
            var client = QueueClient.CreateFromConnectionString(_connectionString, _queueName);
            var brokeredMessage = await client.ReceiveAsync();
            if (brokeredMessage == null)
            {
                return null;
            }

            return new AzureServiceBusMessage(brokeredMessage);
        }
    }
}
