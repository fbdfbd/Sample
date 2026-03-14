using UnityEngine;

public sealed class AudioService : IAudioService
{
    public void PlayBgm(string bgmId)
    {
        Debug.Log($"AudioService.PlayBgm : {bgmId}");
    }
}
