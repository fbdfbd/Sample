using UnityEngine;

public sealed class BootState : IGameState
{
    private readonly IGameStateMachine _stateMachine;
    private readonly ISettingsService _settingsService;
    private readonly ISaveService _saveService;

    public BootState(
        IGameStateMachine stateMachine,
        ISettingsService settingsService,
        ISaveService saveService)
    {
        _stateMachine = stateMachine;
        _settingsService = settingsService;
        _saveService = saveService;
    }

    public void Enter()
    {
        // 부팅 단계에서는 공통 서비스의 초기 상태를 준비합니다.
        Debug.Log("BootState.Enter");

        _settingsService.Load();
        Debug.Log($"Save Exists : {_saveService.HasSaveData()}");

        _stateMachine.Enter<TitleState>();
    }

    public void Exit()
    {
        Debug.Log("BootState.Exit");
    }
}
