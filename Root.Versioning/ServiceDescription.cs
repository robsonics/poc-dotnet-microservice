namespace Root.Versioning
{
    public class ServiceDescription
    {
        public string Address { get; set; }

        public string Port { get; set; }

        public ServiceDefinintion ServiceDefinition { get; set; }

        public override string ToString()
        {
            return string.Format("Address: {0}, Port:{1}, ServiceDefinition:{2}", Address, Port, ServiceDefinition);
        }
    }
}
