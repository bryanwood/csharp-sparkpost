﻿using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace SparkPost.RequestSenders
{
    public class AsyncRequestSender : IRequestSender
    {
        private readonly IClient client;
        private readonly IDataMapper dataMapper;
        private readonly Func<HttpClient> httpClientRetriever;

        public AsyncRequestSender(IClient client, IDataMapper dataMapper, Func<HttpClient> httpClientRetriever)
        {
            this.client = client;
            this.dataMapper = dataMapper;
            this.httpClientRetriever = httpClientRetriever;
        }

        public virtual async Task<Response> Send(Request request)
        {
                var httpClient = httpClientRetriever();

                var result = await GetTheResponse(request, httpClient);

                return new Response
                {
                    StatusCode = result.StatusCode,
                    ReasonPhrase = result.ReasonPhrase,
                    Content = await result.Content.ReadAsStringAsync()
                };
        }

        protected virtual async Task<HttpResponseMessage> GetTheResponse(Request request, HttpClient httpClient)
        {
            return await new RequestMethodFinder(httpClient, dataMapper)
                .FindFor(request)
                .Execute(request);
        }
    }
}