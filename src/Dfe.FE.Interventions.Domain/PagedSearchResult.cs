namespace Dfe.FE.Interventions.Domain
{
    public class PagedSearchResult<T>
    {
        public T[] Results { get; set; }
        
        public int CurrentPage { get; set; }
        public int TotalNumberOfPages { get; set; }
        public int TotalNumberOfRecords { get; set; }
        public int PageStartIndex { get; set; }
        public int PageFinishIndex { get; set; }
    }
}