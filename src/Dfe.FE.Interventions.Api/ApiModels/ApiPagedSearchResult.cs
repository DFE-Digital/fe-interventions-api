using System.Text.Json.Serialization;
using Dfe.FE.Interventions.Domain;

namespace Dfe.FE.Interventions.Api.ApiModels
{
    public class ApiPagedSearchResult<T> : PagedSearchResult<T>
    {
        [JsonPropertyName("_links")]
        public ApiPagedSearchResultLinks Links { get; set; }
    }
}