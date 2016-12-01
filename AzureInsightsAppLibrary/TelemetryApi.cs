// ***********************************************************************
// Assembly         : AzureInsightsAppLibrary
// Author           : Jashif
// Created          : 12-01-2016
//
// Last Modified By : Jashif
// Last Modified On : 12-01-2016
// ***********************************************************************
// <copyright file="TelemetrySettings.cs" company="Jashif">
//     Copyright ©  2016
// </copyright>
// <summary></summary>
// ***********************************************************************

using NetworkHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureInsightsAppLibrary
{
    /// <summary>
    /// Class TelemetryApi.
    /// </summary>
    public class TelemetryApi
    {

        /// <summary>
        /// The baseurl
        /// </summary>
        string baseurl = "https://api.applicationinsights.io/beta/apps/";
        /// <summary>
        /// Gets the events.
        /// </summary>
        /// <param name="eventtype">The eventtype.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public async Task<string> GetEvents(string eventtype)
        {
            string url = baseurl + TelemetrySettings.APP_ID + "/events/" + eventtype;
            return await PerformRequest(url);
        }
        /// <summary>
        /// Ges the metrics.
        /// </summary>
        /// <param name="metricid">The metricid.</param>
        /// <param name="timespan">The timespan.</param>
        /// <param name="intrval">The intrval.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public async Task<string> GeMetrics(string metricid, string timespan = "PT12H", string intrval = "PT30M")
        {
            string url = baseurl + TelemetrySettings.APP_ID + "/metrics/" + metricid + "?timespan=" + timespan + "&interval=" + intrval;
            return await PerformRequest(url);
        }

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        private Dictionary<string, string> GetHeader()
        {
            return new Dictionary<string, string>() { { "x-api-key", TelemetrySettings.APP_KEY } };
        }

        /// <summary>
        /// Performs the request.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        private async Task<string> PerformRequest(string url)
        {
            RequestBase req = new RequestBase(url, GetHeader());

            var response = await req.GetAsync();

            return response.Content;

        }
    }
}
