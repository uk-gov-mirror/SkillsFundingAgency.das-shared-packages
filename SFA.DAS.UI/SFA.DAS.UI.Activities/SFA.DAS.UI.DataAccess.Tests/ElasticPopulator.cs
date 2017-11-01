﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using NuGet;

namespace SFA.DAS.UI.DataAccess.Tests
{
    public class ElasticPopulator
    {
        private static ElasticClient _elasticClient;
        public ElasticPopulator()
        {
            var indexName = "activities";
            //var elasticSettings = new ConnectionSettings(new Uri(configuration.ElasticServerBaseUrl));
            var elasticSettings = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex(indexName);
            _elasticClient = new ElasticClient(elasticSettings);

            _elasticClient.DeleteIndex(indexName);

            CreateIndex(indexName);

            var activities = MakeSomeActivities();

            _elasticClient.IndexMany(activities, indexName);

            foreach (var activity in activities)
            {
                _elasticClient.Index(activity);
            }

        }

        private static void CreateIndex(string indexName)
        {
            _elasticClient.CreateIndex(indexName, i => i.Mappings(ms => ms.Map<Activity>(m => m.AutoMap())));
        }

        private static List<Activity> MakeSomeActivities()
        {
            var activities = new ActivitySource().TestActivities;
            return activities;
        }

      
    }
}