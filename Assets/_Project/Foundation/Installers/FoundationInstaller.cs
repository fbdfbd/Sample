public sealed class FoundationInstaller : IGameInstaller
{
    public void Install(IServiceCollection services)
    {
        // Foundation 레이어에서 공통으로 쓰는 핵심 객체를 등록합니다.
        services.RegisterSingleton<IEventBus>(new EventBus());
        services.RegisterSingleton<IGameStateMachine, GameStateMachine>();
    }
}
