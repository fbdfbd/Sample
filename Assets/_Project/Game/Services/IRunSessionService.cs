public interface IRunSessionService
{
    int CurrentLives { get; }
    void StartNewRun(int startingLives);
}
