using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace WebApplication1
{
   
    public class Program
    {
        public static void Main(string[] args)
        {
            foreach (var arg in args)
            {
                Console.WriteLine("Arg: " + arg);
            }
            var parsedArgs = args.Select(t => {
                var q = t.Split('=');
            return new KeyValuePair<string, string>(q[0],q[1]);
            });
            int port = 80;
            var kv =  parsedArgs.FirstOrDefault(t => string.Equals(t.Key, "port", StringComparison.OrdinalIgnoreCase));
            int.TryParse(kv.Value, out port);

            var config = new ConfigurationBuilder()
                                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                    .AddEnvironmentVariables()
                                    .AddInMemoryCollection(parsedArgs)
                                .Build();
            var url = "http://localhost:" + port;
            Console.WriteLine("Setting up url for listening: " + url);
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseIISIntegration()
                .ConfigureServices(t => {
                    t.Add(new Microsoft.Extensions.DependencyInjection.ServiceDescriptor(typeof(IConfigurationRoot), config));
                })
                .UseUrls(url)
                .CaptureStartupErrors(true)
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
