using Microsoft.AspNetCore.Mvc;
using Root.Versioning.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Root.Versioning.Controllers
{
    [Route("api/[controller]")]
    public class ResolveController : Controller
    {
        private IRoutingService _routingService;
        private IServiceDiscovery _serviceDiscovery;

        public ResolveController(IServiceDiscovery serviceDiscovery, IRoutingService routingService)
        {
            _serviceDiscovery = serviceDiscovery;
            _routingService = routingService;
        }

        [HttpPost("service")]
        public ServiceDescription Service([FromBody] ServiceDefinintion serviceDefinition)
        {
            var services = _serviceDiscovery.GetService(serviceDefinition);

            var matchService = _routingService.SelectService(serviceDefinition, services);

            return matchService;
        }
    }
}
