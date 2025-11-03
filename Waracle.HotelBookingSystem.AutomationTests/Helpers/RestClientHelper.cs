using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Waracle.HotelBookingSystem.AutomationTests.Helpers
{
    public class RestClientHelper
    {
        private readonly RestClient _client;

        public RestClientHelper(string baseUrl)
        {
            _client = new RestClient(baseUrl);
        }

        public async Task<RestResponse<T>> ExecuteGetAsync<T>(string endpoint, Dictionary<string, string> queryParams = null)
        {
            var request = new RestRequest(endpoint, Method.Get);
            if (queryParams != null)
            {
                foreach (var param in queryParams)
                {
                    request.AddQueryParameter(param.Key, param.Value);
                }
            }

            return await _client.ExecuteAsync<T>(request);
        }

        public async Task<RestResponse<T>> ExecutePostAsync<T>(string endpoint, object body)
        {
            var request = new RestRequest(endpoint, Method.Post);
            request.AddJsonBody(body);

            return await _client.ExecuteAsync<T>(request);
        }
    }
}
