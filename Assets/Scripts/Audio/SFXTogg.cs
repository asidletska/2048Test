using UnityEngine;
using UnityEngine.UI;

public class SFXTogg : MonoBehaviour
{
    [SerializeField] private Toggle toggle;

    [Header("Checkmarks")]
    [SerializeField] private GameObject checkmarkOn;
    [SerializeField] private GameObject checkmarkOff;

    private IAudioSettingsService _audio;
    private bool _ignore;

    private void Awake()
    {
        if (toggle == null)
            toggle = GetComponent<Toggle>();
    }

    private void Start()
    {
        var app = AppBootstrapper.Instance != null ? AppBootstrapper.Instance : FindObjectOfType<AppBootstrapper>();

        if (app == null || app.Audio == null)
        {
          //  Debug.LogError("SFXTogg: AppBootstrapper or AudioSettingsService not found.");
            enabled = false;
            return;
        }

        _audio = app.Audio;

        bool savedState = _audio.SfxEnabled;

        _ignore = true;
        toggle.isOn = savedState;
        _ignore = false;

        UpdateCheckmarks(savedState);

        toggle.onValueChanged.AddListener(OnToggleChanged);
    }
    private void OnDestroy()
    {
        if (toggle != null)
            toggle.onValueChanged.RemoveListener(OnToggleChanged);
    }
    private void OnToggleChanged(bool value)
    {
        if (_ignore) return;

        _audio.SetSfxEnabled(value);
        UpdateCheckmarks(value);
    }
    private void UpdateCheckmarks(bool enabledState)
    {
        if (checkmarkOn != null) checkmarkOn.SetActive(enabledState);
        if (checkmarkOff != null) checkmarkOff.SetActive(!enabledState);
    }
}
