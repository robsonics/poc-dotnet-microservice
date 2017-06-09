using System.Collections;
using System.Collections.Generic;

namespace Root.Versioning.Services
{
    public interface IServiceDiscovery
    {
        void RegisterService(ServiceDescription serviceDescription);

        IList<ServiceDescription> GetService(ServiceDefinintion serviceDefinition);
    }
}