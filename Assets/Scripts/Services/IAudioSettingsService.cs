public interface IAudioSettingsService
{
    float MusicVolume { get; }
    float SfxVolume { get; }
    bool MusicEnabled { get; }
    bool SfxEnabled { get; }

    void SetMusicVolume(float v);
    void SetSfxVolume(float v);
    void SetMusicEnabled(bool enabled);
    void SetSfxEnabled(bool enabled);
}