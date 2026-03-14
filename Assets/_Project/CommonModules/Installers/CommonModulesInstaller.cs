public sealed class CommonModulesInstaller : IGameInstaller
{
    public void Install(IServiceCollection services)
    {
        // 대부분의 프로젝트에서 거의 반복되는 공통 서비스를 등록합니다.
        services.RegisterSingleton<ISettingsService, SettingsService>();
        services.RegisterSingleton<IAudioService, AudioService>();
        services.RegisterSingleton<ISaveService, MemorySaveService>();
    }
}
