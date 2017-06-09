namespace Root.Versioning.Services
{
    public interface IServicePersistance
    {
        void Add(ServiceDescription serviceDescription);

        void Update(ServiceDescription serviceDescription);

        void Remove(ServiceDescription serviceDescription);
    }
}