﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus.Persistence;

namespace SFA.DAS.NServiceBus.ClientOutbox
{
    public interface IClientOutboxStorageV2
    {
        Task<IClientOutboxTransaction> BeginTransactionAsync();
        Task<ClientOutboxMessageV2> GetAsync(Guid messageId, SynchronizedStorageSession synchronizedStorageSession);
        Task<IEnumerable<IClientOutboxMessageAwaitingDispatch>> GetAwaitingDispatchAsync();
        Task SetAsDispatchedAsync(Guid messageId);
        Task SetAsDispatchedAsync(Guid messageId, SynchronizedStorageSession synchronizedStorageSession);
        Task StoreAsync(ClientOutboxMessageV2 clientOutboxMessage, IClientOutboxTransaction clientOutboxTransaction);
    }
}