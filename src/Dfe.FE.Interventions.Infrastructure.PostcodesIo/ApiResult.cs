namespace Dfe.FE.Interventions.Infrastructure.PostcodesIo
{
    public class ApiResult<T>
    {
        public int Status { get; set; }
        public T Result { get; set; }
    }
}