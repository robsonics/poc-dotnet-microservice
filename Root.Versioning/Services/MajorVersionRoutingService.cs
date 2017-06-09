using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Root.Versioning.Services
{
    public class MajorVersionRoutingService : IRoutingService
    {
        public ServiceDescription SelectService(ServiceDefinintion queringServices, IEnumerable<ServiceDescription> avalibleServices)
        {
            var queringServiceVersion = new Version(queringServices.Version);
            return avalibleServices.Where(w=>w.ServiceDefinition!=null && queringServices.ServicName == w.ServiceDefinition.ServicName && new Version(w.ServiceDefinition.Version).Major == queringServiceVersion.Major)
                .OrderByDescending(q=>new Version(q.ServiceDefinition.Version)).FirstOrDefault();
        }
    }

  
}
