using System.Text.Json.Serialization;
using Dfe.FE.Interventions.Domain;

namespace Dfe.FE.Interventions.Api.ApiModels
{
    public class ApiPagedSearchResult<T> : PagedSearchResult<T>
    {
        [JsonPropertyName("_links")]
        public ApiPagedSearchResultLinks Links { get; set; }
    }
    
    public class ApiPagedSearchResultLinks
    {
        public string First { get; set; }
        public string Next { get; set; }
        public string Prev { get; set; }
        public string Last { get; set; }
    }
}