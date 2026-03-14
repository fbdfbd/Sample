public interface IGameStateMachine
{
    void Enter<TState>() where TState : IGameState;
}
