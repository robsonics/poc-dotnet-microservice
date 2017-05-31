using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using Consul;

namespace ConsulABTest.Controllers
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
                address = "http://localhost:5001";
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
                    var satteliteService = services.Where(t => string.Equals(t.Value.Service, "Satellite", StringComparison.OrdinalIgnoreCase)).OrderByDescending(t =>
                    {
                        var version = t.Value.Tags.FirstOrDefault(q => q.Contains("v"));
                        return Int32.Parse(version.Replace("v", string.Empty));
                    }).FirstOrDefault(t => string.Equals(t.Value.Service, "Satellite"));

                    Console.WriteLine(satteliteService);

                    address = satteliteService.Value == null ? string.Empty : satteliteService.Value.Address + ":" + satteliteService.Value.Port;
                    foreach (var item in services)
                    {
                        Console.WriteLine("key" + item.Key + " val " + item.Value.Address + " tags: " + string.Join(", ",item.Value.Tags ) + " service: " + item.Value.Service);
                    }
                };
            }
            Console.WriteLine(address + "/api/values/message");

            var client = new HttpClient();
            var response = client.GetAsync(address + "/api/values/message").GetAwaiter().GetResult();
            var messageBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return new string[] { messageBody, env, "MAIN" };
        }

        [HttpGet("message")]
        public string GetMessage()
        {
            return "Message from MAIN service. Env on MAIN is " + (_env.IsDevelopment() ? "Dev" : "Prod"); 
        }


        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
