using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Hangfire;
using Hangfire.MemoryStorage;
using Consul;
using System.Text;
using WebApplication1.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace WebApplication1
{
    public class Startup
    {
        //IHostingEnvironment env
        public Startup(IConfigurationRoot configurationRoot)
        {
            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(env.ContentRootPath)
            //    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            //    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            //    .AddEnvironmentVariables();

            //Configuration = builder.Build();
            Configuration = configurationRoot;
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IMessageFactory, MessageFactory>();
            services.AddTransient<IMessageClient, MessageClient>();

            // Add framework services.
            services.AddMvc();
            services.AddOptions();
            services.AddHangfire(t => {
                t.UseMemoryStorage();
                //t.UseActivator<IMessageClient>(new MessageClient());

            });
            services.Configure<ConfigData>(Configuration);
            services.Configure<ConfigData>(config => {
                using (var client = new ConsulClient(conf=> { conf.Address = new Uri(@"http://172.17.0.2:8500"); }))
                {
                    client.Agent.ServiceRegister(new AgentServiceRegistration());
                    QueryResult<KVPair> getPair = null;
                    try
                    {                    
                        getPair = client.KV.Get("rabbitmqip").GetAwaiter().GetResult();
                        if (getPair.Response != null)
                        {
                            var serviceUrl = Encoding.UTF8.GetString(getPair.Response.Value, 0, getPair.Response.Value.Length);
                            Console.WriteLine("rabbitmqip: " + serviceUrl);
                            config.RabbitMqIp = serviceUrl;
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        config.RabbitMqIp = "localhost";
                        config.InputQueue = "Invalid";
                        config.OutputQueue = "Invalid";
                        config.Version = Configuration["version"];
                        return;
                    }

                    try
                    {
                        getPair = client.KV.Get(string.Format("{0}_inputqueue", Configuration["version"])).GetAwaiter().GetResult();
                        if (getPair.Response != null)
                        {
                            var serviceUrl = Encoding.UTF8.GetString(getPair.Response.Value, 0, getPair.Response.Value.Length);
                            config.InputQueue = serviceUrl;
                            Console.WriteLine("_inputqueue: " + serviceUrl);

                        }
                    }
                    catch (Exception)
                    {
                        config.InputQueue = "Invalid";
                    }

                    try
                    {
                        getPair = client.KV.Get(string.Format("{0}_outputqueue", Configuration["version"])).GetAwaiter().GetResult();
                        if (getPair.Response != null)
                        {
                            var serviceUrl = Encoding.UTF8.GetString(getPair.Response.Value, 0, getPair.Response.Value.Length);
                            config.OutputQueue = serviceUrl;

                        }
                    }
                    catch (Exception)
                    {
                        config.OutputQueue = "Invalid";
                    }

                    config.Version = Configuration["version"];
                }
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            GlobalConfiguration.Configuration.UseMemoryStorage();
            //app.UseHangfireServer();
            app.UseHangfireServer();
            app.UseHangfireDashboard();
            app.UseMvc();
            var urls = string.IsNullOrEmpty(Configuration["consul-urls"])?new[] { string.Empty } : Configuration["consul-urls"].Split(';');

            Register(new[] { @"http://172.17.0.2:8500" });
        }

        private void Register(string[] consulUrl) {
            foreach (var url in consulUrl)
            {
                if (RegisterInConsul(url))
                    return;
            }
        }

        private bool RegisterInConsul(string url)
        {
            try
            {

                using (var client = new ConsulClient(conf => { conf.Address = new Uri(url); }))// TODO should replace with array of address read from 
                {
                    var req = new AgentServiceRegistration()
                    {
                        Address = "",
                        Name = "",
                        Port = 80,

                    };
                    client.Agent.ServiceRegister(req).GetAwaiter().GetResult();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
