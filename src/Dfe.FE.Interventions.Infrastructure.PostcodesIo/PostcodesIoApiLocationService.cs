using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dfe.FE.Interventions.Domain.Locations;
using Microsoft.Extensions.Logging;

namespace Dfe.FE.Interventions.Infrastructure.PostcodesIo
{
    public class PostcodesIoApiLocationService : ILocationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PostcodesIoApiLocationService> _logger;

        public PostcodesIoApiLocationService(
            HttpClient httpClient,
            ILogger<PostcodesIoApiLocationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            _httpClient.BaseAddress = new Uri("https://api.postcodes.io", UriKind.Absolute);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        
        public async Task<Location> GetByPostcodeAsync(string postcode, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(new Uri($"/postcodes/{postcode}", UriKind.Relative), cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("No postcode data to see here"); // TODO: Throw proper error
            }

            var json = await response.Content.ReadAsStringAsync();
            var apiResult = JsonSerializer.Deserialize<ApiResult<PostcodeEntity>>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

            return new Location
            {
                Postcode = apiResult.Result.Postcode,
                Region = apiResult.Result.Region,
            };
        }
    }
}