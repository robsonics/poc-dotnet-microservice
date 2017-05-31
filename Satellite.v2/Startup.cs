using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Consul;
using System.Net;
using System.Net.Sockets;

namespace Satellite.v2
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
            app.UseDeveloperExceptionPage();
            using (var consul = new ConsulClient(t =>
            {
                t.Address = new Uri("http://172.17.0.2:8500");
            }))
            {
                var ip = GetLocalIPAddress();
                Console.WriteLine("Local IP address: " +ip);
                var resgisterResult = consul.Agent.ServiceRegister(new AgentServiceRegistration
                {
                    ID = "Satellitev2",
                    Name = "Satellite",
                    Address = "http://"+ip,
                    Port = 80,
                    Tags = new[] { "v2" }
                }).GetAwaiter().GetResult();
                Console.WriteLine("resgisterResult " + resgisterResult);
            };
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntryAsync(Dns.GetHostName()).GetAwaiter().GetResult();
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
    }
}
