﻿using NServiceBus;
using NServiceBus.Logging;

namespace SFA.DAS.NServiceBus.NLog
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration UseNLogFactory(this EndpointConfiguration config)
        {
            LogManager.Use<NLogFactory>();

            return config;
        }
    }
}