using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;

namespace Root.Versioning.Services
{
    public class ConsulServiceDiscovery : IServiceDiscovery
    {
        public const string VersionTokenPrefix = "version:";
        public const string VersionTokenString = VersionTokenPrefix + @"{0}";

        public static string ToVersionTag(string version)
        {
            return string.Format(VersionTokenString, version);
        }

        public static string FromVersionTag(string[] tags)
        {
            return tags.FirstOrDefault(t => t.StartsWith(VersionTokenPrefix));
        }

        public IList<ServiceDescription> GetService(ServiceDefinintion serviceDefinition)
        {
            using (var consul = new ConsulClient(t =>
            {
                t.Address = new Uri("http://172.17.0.2:8500");
            }))
            {
                var services = consul.Agent.Services().Result.Response;
                return services.Select(ToServiceDescription).ToList();
            };
        }

        public static ServiceDescription ToServiceDescription(KeyValuePair<string ,AgentService> input)
        {
            var serviceName = input.Key;
            var service = input.Value;

            return new ServiceDescription
            {
                Address = service.Address,
                Port = service.Port.ToString(),
                ServiceDefinition = new ServiceDefinintion
                {
                    ServicName=service.Service,
                    Version = FromVersionTag(service.Tags)
                }
            };
        }

        public void RegisterService(ServiceDescription serviceDescription)
        {
            try
            {
                using (var consul = new ConsulClient(t =>
                {
                    t.Address = new Uri("http://172.17.0.2:8500"); // TODO make it configurable
                }))
                {

                    var resgisterResult = consul.Agent.ServiceRegister(new AgentServiceRegistration
                    {
                        ID = string.Format("{0}_{1}_{2}_{3}",
                                    serviceDescription.ServiceDefinition.ServicName,
                                    serviceDescription.ServiceDefinition.Version,
                                    serviceDescription.Address,
                                    serviceDescription.Port),
                        Name = serviceDescription.ServiceDefinition.ServicName,
                        Address = serviceDescription.Address,
                        Port = string.IsNullOrEmpty(serviceDescription.Port) ? 0 : int.Parse(serviceDescription.Port),// TODO replace with tryparse
                        Tags = new[] { serviceDescription.ServiceDefinition.Version }
                    }).GetAwaiter().GetResult();

                    Console.WriteLine("resgisterResult " + resgisterResult); // TODO add logger
                };
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
