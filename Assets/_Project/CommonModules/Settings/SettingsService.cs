using UnityEngine;

public sealed class SettingsService : ISettingsService
{
    public SettingsData Current { get; } = new();

    public void Load()
    {
        // 지금은 기본값만 사용하는 최소 구현입니다.
        Debug.Log("SettingsService.Load");
    }

    public void Save()
    {
        Debug.Log("SettingsService.Save");
    }
}
