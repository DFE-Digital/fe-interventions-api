namespace Dfe.FE.Interventions.Domain
{
    public class UpsertResult<TKey>
    {
        public bool Created { get; set; }
        public TKey Key { get; set; }
    }
}