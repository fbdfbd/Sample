public sealed class RunSessionService : IRunSessionService
{
    public int CurrentLives { get; private set; }

    public void StartNewRun(int startingLives)
    {
        CurrentLives = startingLives;
    }
}
