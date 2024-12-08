namespace ApiDiariosOficiais.Factory
{
    public class GenericServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public GenericServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T Create<T>()
        {
            return _serviceProvider.GetRequiredService<T>();
        }
    }
}
