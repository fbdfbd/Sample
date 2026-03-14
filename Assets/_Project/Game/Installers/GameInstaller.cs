public sealed class GameInstaller : IGameInstaller
{
    public void Install(IServiceCollection services)
    {
        // 게임 전용 설정값은 인스턴스로 바로 등록합니다.
        services.RegisterSingleton(new GameRuleConfig
        {
            StartingLives = 3,
            FirstStageName = "Stage_01"
        });

        // 게임 전용 서비스는 인터페이스와 구현체를 연결해 등록합니다.
        services.RegisterSingleton<IRunSessionService, RunSessionService>();
        services.RegisterSingleton<IStageFlowService, StageFlowService>();
    }
}
