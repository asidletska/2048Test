public interface IAudioSettingsService
{
    bool MusicEnabled { get; }
    bool SfxEnabled { get; }

    void SetMusicEnabled(bool enabled);
    void SetSfxEnabled(bool enabled);
}