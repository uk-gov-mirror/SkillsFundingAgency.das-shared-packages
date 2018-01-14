﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;

namespace SFA.DAS.Elastic
{
    public class ElasticClientFactory : IElasticClientFactory
    {
        public IConnectionSettingsValues ConnectionSettings { get; }
        public string EnvironmentName { get; }
        public IEnumerable<IIndexMapper> IndexMappers { get; }

        public ElasticClientFactory(string environmentName, string url, string username, string password, Action<IApiCallDetails> onRequestCompleted, IEnumerable<IIndexMapper> indexMappers)
        {
            var connectionSettings = new ConnectionSettings(new SingleNodeConnectionPool(new Uri(url))).ThrowExceptions();

            if (onRequestCompleted != null)
            {
                connectionSettings.OnRequestCompleted(onRequestCompleted);
            }

            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                connectionSettings.BasicAuthentication(username, password);
            }

            ConnectionSettings = connectionSettings;
            EnvironmentName = environmentName;
            IndexMappers = indexMappers;
        }

        public IElasticClient CreateClient()
        {
            var client = new ElasticClient(ConnectionSettings);

            if (IndexMappers != null)
            {
                Task.WaitAll(IndexMappers.Select(m => m.EnureIndexExistsAsync(EnvironmentName, client)).ToArray());
            }

            return client;
        }

        public void Dispose()
        {
            ConnectionSettings.Dispose();

            foreach (var indexMapper in IndexMappers)
            {
                indexMapper.Dispose();
            }
        }
    }
}