using UnityEngine;

public sealed class TitleState : IGameState
{
    private readonly IAudioService _audioService;

    public TitleState(IAudioService audioService)
    {
        _audioService = audioService;
    }

    public void Enter()
    {
        // 타이틀 상태에서는 최소한의 연출만 시작합니다.
        Debug.Log("TitleState.Enter");
        _audioService.PlayBgm("Title");
    }

    public void Exit()
    {
        Debug.Log("TitleState.Exit");
    }
}
