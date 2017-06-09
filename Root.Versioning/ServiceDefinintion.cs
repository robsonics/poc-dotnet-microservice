using System.Collections.Generic;

namespace Root.Versioning
{
    public class ServiceDefinintion
    {
        public string ServicName { get; set; }

        public string Version { get; set; }

        public IDictionary<string, string> Dependencies { get; set; }

        public override string ToString()
        {
            return string.Format("ServicName: {0}, Version: {1}, Dependencies: [{2}]", ServicName, Version, string.Join(", ", Dependencies));
        }
    }
}
