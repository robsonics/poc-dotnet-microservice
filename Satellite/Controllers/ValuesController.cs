using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using Consul;

namespace Satellite.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private IHostingEnvironment _env;

        public ValuesController(IHostingEnvironment env)
        {
            _env = env;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var env = string.Empty;
            var address = string.Empty;
            if (_env.IsDevelopment())
            {
                env = "DEV";
                address = "http://localhost:5000";
            }
            else
            {
                Console.WriteLine("PROD env processing...");
                env = "PROD";
                using (var consul = new ConsulClient(t =>
                {
                    t.Address = new Uri("http://172.17.0.2:8500");
                }))
                {
                    var services = consul.Agent.Services().GetAwaiter().GetResult().Response;
                    var satteliteService = services.FirstOrDefault(t => string.Equals(t.Key, "Main", StringComparison.OrdinalIgnoreCase));
                    address = satteliteService.Value == null ? string.Empty : satteliteService.Value.Address + ":" + satteliteService.Value.Port;
                    foreach (var item in services)
                    {
                        Console.WriteLine("key" + item.Key + " val " + item.Value.Address);
                    }
                    Console.WriteLine("address " + address);
                };
            }

            var client = new HttpClient();
            var response = client.GetAsync(address + "/api/values/message").GetAwaiter().GetResult();
            var messageBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return new string[] { messageBody, env, "SATTELITE" };

        }

        [HttpGet("message")]
        public string GetMessage()
        {
            return "Message from SATELLITE V1 service. Env on SATELLITE is " + (_env.IsDevelopment() ? "Dev" : "Prod");
        }
    }
}
