public sealed class GameStateMachine : IGameStateMachine
{
    private readonly IObjectResolver _resolver;
    private IGameState _currentState;

    public GameStateMachine(IObjectResolver resolver)
    {
        _resolver = resolver;
    }

    public void Enter<TState>() where TState : IGameState
    {
        // 이전 상태를 종료하고 새 상태를 컨테이너를 통해 생성합니다.
        _currentState?.Exit();

        TState nextState = _resolver.Resolve<TState>();
        _currentState = nextState;
        _currentState.Enter();
    }
}
