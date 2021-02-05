using Microsoft.Extensions.Hosting;

namespace Dfe.FE.Interventions.Consumer.Ukrlp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var startup = new Startup();
            
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) => startup.ConfigureServices(services));
        }
    }
}