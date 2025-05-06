﻿using MedVoll.Web.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace MedVoll.Web.Services
{
    delegate Task<HttpResponseMessage> HttpVerbMethodUri(Uri requestUri, HttpContent content);
    delegate Task<HttpResponseMessage> HttpVerbMethodString(string requestUri, HttpContent content);

    public abstract class BaseHttpService : IService, IBaseHttpService
    {
        protected readonly IConfiguration _configuration;
        protected readonly IHttpClientFactory _httpClientFactory;
        protected HttpContext _httpContext = null;

        public BaseHttpService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public abstract string Scope { get; }

        protected async Task<T> GetAsync<T>(string uri, params object[] param)
        {
            var requestUri = string.Format(uri, param);

            foreach (var par in param)
            {
                requestUri += string.Format($"/{par}");
            }

            using HttpClient httpClient = await GetHttpClientAsync();

            var json = await httpClient.GetStringAsync(requestUri);
            return JsonConvert.DeserializeObject<T>(json);
        }

        protected async Task<T> PutOrPostAsync<T>(string uri, object content)
        {
            using HttpClient httpClient = await GetHttpClientAsync();

            var httpVerbMethod = new HttpVerbMethodString(httpClient.PutAsync);
            return await PutOrPostAsync<T>(uri, content, httpVerbMethod);
        }

        protected async Task DeleteAsync<T>(string uri, params object[] param)
        {
            var requestUri = string.Format(uri, param);

            foreach (var par in param)
            {
                requestUri += string.Format($"/{par}");
            }

            using HttpClient httpClient = await GetHttpClientAsync();

            var json = await httpClient.DeleteAsync(requestUri);
        }

        private async Task<T> PutOrPostAsync<T>(string uri, object content, HttpVerbMethodString httpVerbMethod)
        {
            var jsonIn = JsonConvert.SerializeObject(content);
            var stringContent = new StringContent(jsonIn, Encoding.UTF8, "application/json");

            HttpResponseMessage httpResponse = await httpVerbMethod(uri, stringContent);
            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync();
                throw new HttpRequestException(errorContent);
            }
            var jsonOut = await httpResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonOut);
        }

        private async Task<HttpClient> GetHttpClientAsync()
        {
            HttpClient httpClient = _httpClientFactory.CreateClient(_configuration["MedVoll.WebApi.Name"] ?? "");
            return httpClient;
        }
    }
}

