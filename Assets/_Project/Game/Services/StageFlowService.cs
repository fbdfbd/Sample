using UnityEngine;

public sealed class StageFlowService : IStageFlowService
{
    private readonly GameRuleConfig _config;
    private readonly IRunSessionService _runSessionService;
    private readonly IEventBus _eventBus;

    public StageFlowService(
        GameRuleConfig config,
        IRunSessionService runSessionService,
        IEventBus eventBus)
    {
        _config = config;
        _runSessionService = runSessionService;
        _eventBus = eventBus;
    }

    public void StartFirstStage()
    {
        // 게임 전용 흐름도 다른 서비스와 설정값을 주입받아 동작합니다.
        _runSessionService.StartNewRun(_config.StartingLives);
        _eventBus.Publish(new FirstStageStartedEvent(_config.FirstStageName));

        Debug.Log($"First stage started: {_config.FirstStageName}");
    }
}
