using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.Telemetry.RedactionService
{
    public class UriRedactionService : IUriRedactionService
    {
        private readonly UriRedactionOptions _options;

        public UriRedactionService(UriRedactionOptions options)
        {
            _options = options;
        }

        public Uri GetRedactedUri(Uri uri)
        {
            if (string.IsNullOrEmpty(uri.Query) || uri.Query == "?*")
            {
                // the default MS redactor in .Net 9+ will reduce the query to ?*
                return uri;
            }

            var components = HttpUtility.ParseQueryString(uri.Query);

            var keysToRedact = _options.RedactionList
                .Where(key => !string.IsNullOrWhiteSpace(key))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var redactionList = components.AllKeys
                .Where(key => !string.IsNullOrWhiteSpace(key) && keysToRedact.Contains(key))
                .ToList();

            foreach (var redaction in redactionList)
            {
                components[redaction] = _options.RedactionString;
            }

            var uriBuilder = new UriBuilder(uri)
            {
                Query = components.ToString()
            };

            var newUri = uriBuilder.Uri;
            return newUri;
        }
    }
}