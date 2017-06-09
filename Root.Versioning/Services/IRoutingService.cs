using System.Collections.Generic;

namespace Root.Versioning.Services
{
    public interface IRoutingService
    {
        ServiceDescription SelectService(ServiceDefinintion queringServices, IEnumerable<ServiceDescription> avalibleServices);
    }
}