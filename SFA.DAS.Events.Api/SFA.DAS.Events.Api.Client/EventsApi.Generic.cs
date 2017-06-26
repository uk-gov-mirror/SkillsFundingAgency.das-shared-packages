﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Events.Api.Types;
using Newtonsoft.Json;

namespace SFA.DAS.Events.Api.Client
{
    public partial class EventsApi
    {
        /// <summary>
        /// Creates a new Generic Event
        /// </summary>
        /// <param name="genericEvent">GenericEvent to create</param>
        /// <returns></returns>
        public async Task CreateGenericEvent(GenericEvent genericEvent)
        {
            var url = $"{_configuration.BaseUrl}api/events/create";

            await PostEvent(url, genericEvent);
        }

        /// <summary>
        /// Creates a new Generic Event
        /// </summary>
        /// <typeparam name="T">The type of the payload</typeparam>
        /// <param name="event">The generic event to create</param>
        /// <returns></returns>
        public async Task CreateGenericEvent<T>(IGenericEvent<T> @event)
        {
            var genericEvent = GenericEventMapper.FromTyped<T>(@event);
            await CreateGenericEvent(genericEvent);
        }

        /// <summary>
        /// Get a list of GenericEvents starting from the supplied Id
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="fromEventId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns>List of GenericEvent</returns>
        public async Task<List<GenericEvent>> GetGenericEventsById(string eventType, long fromEventId = 0, int pageSize = 1000, int pageNumber = 1)
        {
            var url = $"{_configuration.BaseUrl}api/events/getSinceEvent?eventType={eventType}&fromEventId={fromEventId}&pageSize={pageSize}&pageNumber={pageNumber}";

            return await GetEvents<GenericEvent>(url);
        }

        /// <summary>
        /// Get a list of GenericEvents starting from the supplied Id
        /// </summary>
        /// <typeparam name="T">the type of the payload</typeparam>
        /// <param name="fromEventId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public async Task<List<IGenericEvent<T>>> GetGenericEventsById<T>(long fromEventId = 0, int pageSize = 1000,
            int pageNumber = 1)
        {
            var list = new List<IGenericEvent<T>>();
            var events = await GetGenericEventsById(typeof(T).FullName, fromEventId, pageSize, pageNumber);
            foreach (GenericEvent genericEvent in events)
            {
                var @event = await Task.Factory.StartNew(() => GenericEventMapper.ToTyped<T>(genericEvent));
                list.Add(@event);
            }

            return list;
        }

        /// <summary>
        /// Get a list of GenericEvent by date range
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="fromDate">If not supplied, will revert to start of time</param>
        /// <param name="toDate">If not supplied, will revert to end of time</param>
        /// <param name="pageSize">Maximum of 10,000</param>
        /// <param name="pageNumber"></param>
        /// <returns>List of GenericEvent</returns>
        public async Task<List<GenericEvent>> GetGenericEventsByDateRange(string eventType, DateTime? fromDate = null, DateTime? toDate = null, int pageSize = 1000, int pageNumber = 1)
        {
            var dateString = BuildDateQuery(fromDate, toDate);

            var url = $"{_configuration.BaseUrl}api/events/getByDateRange?eventType={eventType}&{dateString}pageSize={pageSize}&pageNumber={pageNumber}";

            return await GetEvents<GenericEvent>(url);
        }

        /// <summary>
        /// Get a list of GenericEvent by date range
        /// </summary>
        /// <typeparam name="T">the type of the payload</typeparam>
        /// <param name="fromDate">If not supplied, will revert to start of time</param>
        /// <param name="toDate">If not supplied, will revert to end of time</param>
        /// <param name="pageSize">Maximum of 10,000</param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public async Task<List<IGenericEvent<T>>> GetGenericEventsByDateRange<T>(DateTime? fromDate = null, DateTime? toDate = null, int pageSize = 1000, int pageNumber = 1)
        {
            var list = new List<IGenericEvent<T>>();
            var genericEvents = await GetGenericEventsByDateRange(typeof(T).FullName, fromDate, toDate, pageSize, pageNumber);
            foreach (var genericEvent in genericEvents)
            {
                var @event = await Task.Factory.StartNew(() => GenericEventMapper.ToTyped<T>(genericEvent));
                list.Add(@event);
            }

            return list;
        }

        public async Task<List<GenericEvent>> GetGenericEventsByResourceId(string resourceType, string resourceId, DateTime? fromDate = null, DateTime? toDate = null, int pageSize = 1000,
            int pageNumber = 1)
        {
            var dateString = BuildDateQuery(fromDate, toDate);

            var url = $"{_configuration.BaseUrl}api/events/getByResourceId?resourceType={resourceType}&resourceId={resourceId}&{dateString}pageSize={pageSize}&pageNumber={pageNumber}";

            return await GetEvents<GenericEvent>(url);
        }

        public async Task<List<GenericEvent>> GetGenericEventsByResourceUri(string resourceUri, DateTime? fromDate = null, DateTime? toDate = null, int pageSize = 1000, int pageNumber = 1)
        {
            var dateString = BuildDateQuery(fromDate, toDate);

            var url = $"{_configuration.BaseUrl}api/events/getByResourceUri?resourceUri={resourceUri}&{dateString}pageSize={pageSize}&pageNumber={pageNumber}";

            return await GetEvents<GenericEvent>(url);
        }

        public async Task<List<IGenericEvent<T>>> GetGenericEventsByResourceId<T>(string resourceType, string resourceId, DateTime? fromDate = null, DateTime? toDate = null, int pageSize = 1000,
            int pageNumber = 1)
        {
            var genericEvents = await GetGenericEventsByResourceId(resourceType, resourceId, fromDate, toDate, pageSize, pageNumber);
            return genericEvents.Select(GenericEventMapper.ToTyped<T>).ToList();
        }

        public async Task<List<IGenericEvent<T>>> GetGenericEventsByResourceUri<T>(string resourceUri, DateTime? fromDate = null, DateTime? toDate = null, int pageSize = 1000, int pageNumber = 1)
        {
            var genericEvents = await GetGenericEventsByResourceUri(resourceUri, fromDate, toDate, pageSize, pageNumber);
            return genericEvents.Select(GenericEventMapper.ToTyped<T>).ToList();
        }
    }
}
