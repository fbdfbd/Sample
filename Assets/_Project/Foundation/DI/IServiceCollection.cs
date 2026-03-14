public interface IServiceCollection
{
    void RegisterSingleton<TService>(TService instance);

    void RegisterSingleton<TService, TImplementation>()
        where TImplementation : TService;
}
