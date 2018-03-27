﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Messaging.AzureServiceBus
{
    public class AzureServiceBusMessage<T> : Message<T>
    {
        private readonly BrokeredMessage _brokeredMessage;
        private readonly ILog _logger;
        private CancellationTokenSource _keepConnectionAliveCancellationToken;

        public AzureServiceBusMessage(BrokeredMessage brokeredMessage, ILog logger, bool keepConnectionAlive = false)
            : base(brokeredMessage.GetBody<T>())
        {
            _brokeredMessage = brokeredMessage;
            _logger = logger;
            if (keepConnectionAlive)
            {
                KeepConnectionAlive();
            }
        }

        public override Task CompleteAsync()
        {
            _keepConnectionAliveCancellationToken.Cancel();
            return _brokeredMessage.CompleteAsync();
        }

        public override Task AbortAsync()
        {
            _keepConnectionAliveCancellationToken.Cancel();
            return _brokeredMessage.AbandonAsync();
        }

        private void KeepConnectionAlive()
        {
            _keepConnectionAliveCancellationToken = new CancellationTokenSource();

            Task.Factory.StartNew(() =>
            {
                while (!_keepConnectionAliveCancellationToken.Token.IsCancellationRequested)
                {
                    if (DateTime.UtcNow > _brokeredMessage.LockedUntilUtc.AddSeconds(-10))
                    {
                        _brokeredMessage.RenewLock();
                        _logger.Debug("Message lock renewed");
                    }

                    Task.Delay(500);
                }
            }, _keepConnectionAliveCancellationToken.Token);
        }
    }
}
