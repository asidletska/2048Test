using UnityEngine;

public sealed class AudioSettingsService : IAudioSettingsService
    {
        private const string MusicEnabledKey = "music_enabled";
        private const string SfxEnabledKey = "sfx_enabled";

        private readonly ISave _storage;
        private readonly IEventBus _bus;

        public float MusicVolume { get; private set; }
        public float SfxVolume { get; private set; }
        public bool MusicEnabled { get; private set; }
        public bool SfxEnabled { get; private set; }

        public AudioSettingsService(ISave storage, IEventBus bus)
        {
            _storage = storage;
            _bus = bus;

            MusicEnabled = _storage.GetInt(MusicEnabledKey, 1) == 1;
            SfxEnabled = _storage.GetInt(SfxEnabledKey, 1) == 1;

            Publish();
        }

        public void SetMusicEnabled(bool enabled)
        {
            MusicEnabled = enabled;
            _storage.SetInt(MusicEnabledKey, enabled ? 1 : 0);
            _storage.Flush();
            Publish();
        }

        public void SetSfxEnabled(bool enabled)
        {
            SfxEnabled = enabled;
            _storage.SetInt(SfxEnabledKey, enabled ? 1 : 0);
            _storage.Flush();
            Publish();
        }

        private void Publish()
        {
            _bus.Publish(new AudioSettingsChangedEvent(MusicEnabled, SfxEnabled));
        }
    }
