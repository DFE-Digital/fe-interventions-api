using System.Threading;
using System.Threading.Tasks;

namespace Dfe.FE.Interventions.Domain.Locations
{
    public interface ILocationService
    {
        Task<Location> GetByPostcodeAsync(string postcode, CancellationToken cancellationToken);
    }
}