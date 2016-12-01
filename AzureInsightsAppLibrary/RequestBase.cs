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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkHandler
{
    /// <summary>
    /// Class RequestBase.
    /// </summary>
    public class RequestBase
    {
        #region Properties
        /// <summary>
        /// Gets or sets the type of the media.
        /// </summary>
        /// <value>The type of the media.</value>
        protected string MediaType { get; set; }
        /// <summary>
        /// The request URL
        /// </summary>
        public string requestUrl;
        /// <summary>
        /// Theclient
        /// </summary>
        private HttpClient client;
        /// <summary>
        /// Thehandler
        /// </summary>
        private HttpClientHandler handler;
        /// <summary>
        /// Thecookiejar
        /// </summary>
        private CookieContainer cookiejar;

        /// <summary>
        /// Therequest timeout
        /// </summary>
        private TimeSpan requestTimeout;
        /// <summary>
        /// The cache timeout
        /// </summary>
        private TimeSpan cacheTimeout;
        /// <summary>
        /// The cache
        /// </summary>
        private Dictionary<string, Response> cache;
        /// <summary>
        /// Gets or sets the accept.
        /// </summary>
        /// <value>The accept.</value>
        private string Accept { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestBase" /> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="Headers">The headers.</param>
        /// <param name="timeout">The timeout.</param>
        public RequestBase(string url, Dictionary<string, string> Headers = null, double timeout = 60)
        {
            try
            {
                requestUrl = url;
                handler = new HttpClientHandler();
                cookiejar = new CookieContainer();
                client = new HttpClient(handler);
                if (Headers != null)
                {
                    foreach (var key in Headers.Keys)
                    {
                        if (!string.IsNullOrEmpty(Headers[key]))
                            client.DefaultRequestHeaders.Add(key, Headers[key]);
                    }
                }
                client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                client.DefaultRequestHeaders.Add("x-ms-date", DateTime.UtcNow.ToString("r"));
                requestTimeout = TimeSpan.FromSeconds(timeout);
                cacheTimeout = TimeSpan.FromSeconds(30);
                cache = new Dictionary<string, Response>();
            }
            catch (Exception)
            {
                //TODO:
            }

        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the async.
        /// </summary>
        /// <returns>Task{Response}.</returns>
        public async Task<Response> GetAsync()
        {
            return await Http("", HttpMethod.Get, "");

        }

        /// <summary>
        /// GetBinaryAsync
        /// </summary>
        /// <returns>Task&lt;System.Byte[]&gt;.</returns>
        public async Task<byte[]> GetBinaryAsync()
        {

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(requestTimeout);

            HttpResponseMessage httpResp = null;
            Stream httpRespContent = null;

            try
            {
                httpResp = await client.GetAsync(
                    new Uri(requestUrl, UriKind.RelativeOrAbsolute)
                );
                if (httpResp.IsSuccessStatusCode)
                {
                    httpRespContent = (await httpResp.Content.ReadAsStreamAsync());
                    return httpRespContent.ReadAsBytes();
                }
            }
            catch (Exception ex)
            {
            }


            return null;
        }

        /// <summary>
        /// Posts the async.
        /// </summary>
        /// <param name="postdata">The postdata.</param>
        /// <param name="contenttype">The contenttype.</param>
        /// <returns>Task{Response}.</returns>
        public async Task<Response> PostAsync(string postdata, string contenttype = "application/json")
        {
            return await Http(postdata, HttpMethod.Post, contenttype);

        }

        /// <summary>
        /// PutAsync
        /// </summary>
        /// <param name="postdata">The postdata.</param>
        /// <param name="contenttype">The contenttype.</param>
        /// <returns>Task&lt;Response&gt;.</returns>
        public async Task<Response> PutAsync(string postdata, string contenttype = "application/json")
        {
            return await Http(postdata, HttpMethod.Put, contenttype);

        }

        /// <summary>
        /// DeleteAsync
        /// </summary>
        /// <param name="postdata">The postdata.</param>
        /// <param name="contenttype">The contenttype.</param>
        /// <returns>Task&lt;Response&gt;.</returns>
        public async Task<Response> DeleteAsync(string postdata, string contenttype = "application/json")
        {

            return await Http(postdata, HttpMethod.Delete, contenttype);


        }

        /// <summary>
        /// PostFormAsync
        /// </summary>
        /// <param name="postdata">The postdata.</param>
        /// <returns>Task&lt;Response&gt;.</returns>
        public async Task<Response> PostFormAsync(Dictionary<string, string> postdata)
        {
            string cookieresult = "";
            FormUrlEncodedContent httpContent = new FormUrlEncodedContent(postdata);//, Windows.Storage.Streams.UnicodeEncoding.Utf8, contenttype);
                                                                                    //  FormatResource(ref resource);

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(requestTimeout);

            HttpResponseMessage httpResp = null;
            string httpRespContent = null;

            try
            {
                httpResp = await client.PostAsync(
                    new Uri(requestUrl, UriKind.RelativeOrAbsolute),
                    httpContent
                );

                if (httpResp.IsSuccessStatusCode)
                {
                    httpRespContent = await httpResp.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                httpRespContent = ex.Message;
            }
            // client.Dispose();
            return BuildResponse(httpRespContent, httpResp, cookieresult);

        }



        /// <summary>
        /// Http
        /// </summary>
        /// <param name="postdata">The postdata.</param>
        /// <param name="method">The method.</param>
        /// <param name="contenttype">The contenttype.</param>
        /// <returns>Task&lt;Response&gt;.</returns>
        public async Task<Response> Http(string postdata, HttpMethod method, string contenttype = "application/json")
        {
            string cookieresult = "";

            StringContent httpContent = null;
            httpContent = contenttype != "" ? new StringContent(postdata, Encoding.UTF8, contenttype) : new StringContent(postdata);

            //  FormatResource(ref resource);

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(requestTimeout);

            HttpResponseMessage httpResp = null;
            string httpRespContent = null;

            try
            {
                HttpRequestMessage req = new HttpRequestMessage();
                req.Method = method;
                if (method != HttpMethod.Get)
                    req.Content = httpContent;
                req.RequestUri = new Uri(requestUrl);
                httpResp = await client.SendAsync(req, cts.Token);//.AsTask(cts.Token);
                if (httpResp.IsSuccessStatusCode)
                {
                    httpRespContent = await httpResp.Content.ReadAsStringAsync();
                }
                else
                {
                    httpRespContent = await httpResp.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                httpRespContent = ex.Message;
            }
            return BuildResponse(httpRespContent, httpResp, cookieresult);

        }

        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        /// <value>The timeout.</value>
        public TimeSpan Timeout
        {
            get
            {
                return requestTimeout;
            }
            set
            {
                requestTimeout = value;
            }
        }

        /// <summary>
        /// Builds the response.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="httpResp">The HTTP resp.</param>
        /// <param name="cookie">The cookie.</param>
        /// <returns>Response.</returns>
        private Response BuildResponse(string content, HttpResponseMessage httpResp, string cookie = "")
        {
            if (httpResp != null)
            {
                return new Response(content, DateTime.Now, httpResp.StatusCode, httpResp.IsSuccessStatusCode, Response.SuccessStatus.ResponseOk, cookie);
            }
            else
            {
                // If the request fialed we use a fake status code. Is this best practice?
                return new Response(content, DateTime.Now, HttpStatusCode.RequestTimeout, false, Response.SuccessStatus.NetworkFailure, cookie);
            }
        }

        /// <summary>
        /// Determines whether the specified resource is cached.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns><c>true</c> if the specified resource is cached; otherwise, <c>false</c>.</returns>
        private bool IsCached(string resource)
        {
            if (cache.ContainsKey(resource))
            {
                if (DateTime.Now - cache[resource].Sent < cacheTimeout)
                {
                    return true;
                }
                cache.Remove(resource);
            }
            return false;
        }

        /// <summary>
        /// Formats the resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        private void FormatResource(ref string resource)
        {
            string queryParams = "";
            if (resource.Contains("?"))
            {
                var pieces = resource.Split(new char[] { '?' });
                resource = pieces[0];
                queryParams = pieces[1];
            }

            if (resource[0] == '/' && resource.Length > 1)
            {
                resource = resource.Substring(1, resource.Length - 1);
            }

            if (resource[resource.Length - 1] == '/' && resource.Length > 1)
            {
                resource = resource.Substring(0, resource.Length - 1);
            }

            resource = resource + ".json?" + queryParams;
        }

        /// <summary>
        /// Posts the async.
        /// </summary>
        /// <returns>Task{HttpResponse}.</returns>
        public virtual async Task<Response> GetDataAsync()
        {
            return await GetAsync();
        } 
        #endregion
    }

    /// <summary>
    /// Class Response
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Enum SuccessStatus
        /// </summary>
        public enum SuccessStatus
        {
            /// <summary>
            /// The response ok
            /// </summary>
            ResponseOk,
            /// <summary>
            /// The network failure
            /// </summary>
            NetworkFailure
        }

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        public Response()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response" /> class.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="sent">The sent.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="isSuccess">if set to <c>true</c> [is success].</param>
        public Response(
            String content,
            DateTime sent,
            HttpStatusCode statusCode,
            bool isSuccess)
        {
            this.Content = content;
            this.Sent = sent;
            this.StatusCode = statusCode;
            this.IsSuccess = isSuccess;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response" /> class.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="sent">The sent.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="isSuccess">if set to <c>true</c> [is success].</param>
        /// <param name="status">The status.</param>
        /// <param name="cookie">The cookie.</param>
        public Response(
            String content,
            DateTime sent,
            HttpStatusCode statusCode,
            bool isSuccess, SuccessStatus status, string cookie = "")
        {
            this.Content = content;
            this.Sent = sent;
            this.StatusCode = statusCode;
            this.IsSuccess = isSuccess;
            this.ResponseStatus = status;
            this.Cookie = cookie;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the cookie.
        /// </summary>
        /// <value>The cookie.</value>
        public string Cookie { get; set; }
        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>The content.</value>
        public string Content { get; private set; }
        /// <summary>
        /// Gets the sent.
        /// </summary>
        /// <value>The sent.</value>
        public DateTime Sent { get; private set; }
        /// <summary>
        /// Gets the status code.
        /// </summary>
        /// <value>The status code.</value>
        public HttpStatusCode StatusCode { get; private set; }
        /// <summary>
        /// Gets a value indicating whether this instance is success.
        /// </summary>
        /// <value><c>true</c> if this instance is success; otherwise, <c>false</c>.</value>
        public bool IsSuccess { get; private set; }
        /// <summary>
        /// Gets the response status.
        /// </summary>
        /// <value>The response status.</value>
        public SuccessStatus ResponseStatus { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Data.Core.Response:");
            sb.AppendFormat("- Sent: {0}\n", this.Sent);
            sb.AppendFormat("- StatusCode: {0}\n", this.StatusCode);
            sb.AppendFormat("- Content: {0}\n", this.Content);
            return sb.ToString();
        } 
        #endregion
    }

    /// <summary>
    /// Class Extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Reads as bytes.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] ReadAsBytes(this Stream input)
        {
            byte[] buffer = new byte[0x4000];
            using (MemoryStream stream = new MemoryStream())
            {
                int num;
                while ((num = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, num);
                }
                return stream.ToArray();
            }
        }
    }
}