using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1
{
    public class ConfigData
    {
        public string RabbitMqIp { get; set; }

        public string InputQueue { get; set; }

        public string OutputQueue { get; set; }

        public string Version { get; set; }
    }
}
