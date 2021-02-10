using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Dfe.FE.Interventions.Application.UnitTests.ServiceCollectionExtensionsTests
{
    public class WhenAddingManagers
    {
        [Test]
        public void ThenItShouldRegisterAllManagersAsScoped()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddFeInterventionsManagers();

            var expectedManagers = typeof(ServiceCollectionExtensions).Assembly
                .GetTypes()
                .Where(t => t.Name.EndsWith("Manager"))
                .Select(t =>
                {
                    var interfaceName = $"I{t.Name}";
                    return new
                    {
                        ImplementationType = t,
                        InterfaceType = t.GetInterface(interfaceName),
                    };
                })
                .Where(x => x.InterfaceType != null)
                .ToArray();
            Assert.AreEqual(expectedManagers.Length, serviceCollection.Count);
            foreach (var expectedManager in expectedManagers)
            {
                var registration = serviceCollection
                    .Where(sd => sd.ServiceType == expectedManager.InterfaceType)
                    .ToArray();
                Assert.AreEqual(1, registration.Length, 
                    $"Expected 1 registration for {expectedManager.InterfaceType.Name}");
                Assert.AreEqual(expectedManager.ImplementationType, registration.Single().ImplementationType, 
                    $"Expected registration for {expectedManager.InterfaceType.Name} to be implemented by {expectedManager.ImplementationType.Name}");
            }
        }
    }
}