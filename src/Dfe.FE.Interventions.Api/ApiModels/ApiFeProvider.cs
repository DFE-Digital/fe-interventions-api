using System.Text.Json.Serialization;
using Dfe.FE.Interventions.Domain.FeProviders;

namespace Dfe.FE.Interventions.Api.ApiModels
{
    public class ApiFeProvider : FeProvider
    {
        [JsonPropertyName("_links")]
        public ApiFeProviderLinks Links { get; set; }
    }

    public class ApiFeProviderLinks
    {
        [JsonPropertyName("_self")]
        public string Self { get; set; }
    }
}