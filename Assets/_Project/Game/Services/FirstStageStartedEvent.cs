public readonly struct FirstStageStartedEvent
{
    public FirstStageStartedEvent(string stageName)
    {
        StageName = stageName;
    }

    public string StageName { get; }
}
