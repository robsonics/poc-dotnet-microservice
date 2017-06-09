using Microsoft.AspNetCore.Mvc;
using Root.Versioning.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Root.Versioning.Controllers
{
    [Route("api/[controller]")]
    public class RegisterController : Controller
    {
        private IServiceDiscovery _serviceDiscovery;
        private IServicePersistance _servicePersistance;

        public RegisterController(IServiceDiscovery serviceDiscovery, IServicePersistance servicePersistance)
        {
            _serviceDiscovery = serviceDiscovery;
            _servicePersistance = servicePersistance;
        }

        [HttpPost("service")]
        public string Service([FromBody] ServiceDescription serviceDesc)
        {
            try
            {
                _serviceDiscovery.RegisterService(serviceDesc);
                _servicePersistance.Add(serviceDesc);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "OK";
        }
    }
}
