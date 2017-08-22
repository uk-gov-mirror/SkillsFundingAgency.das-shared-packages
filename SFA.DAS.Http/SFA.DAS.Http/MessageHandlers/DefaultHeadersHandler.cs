﻿using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Http.MessageHandlers
{
    public sealed class DefaultHeadersHandler : DelegatingHandler
    {
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage message, CancellationToken cancellationToken)
        {
            message.Headers.Add("accept", "application/json");

            return await base.SendAsync(message, cancellationToken);
        }
    }
}