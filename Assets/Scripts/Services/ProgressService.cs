using UnityEngine;


public sealed class PrgressService : ISave
{
    public int GetInt(string key, int defaultValue = 0) => PlayerPrefs.GetInt(key, defaultValue);
    public void SetInt(string key, int value) => PlayerPrefs.SetInt(key, value);

    public float GetFloat(string key, float defaultValue = 0f) => PlayerPrefs.GetFloat(key, defaultValue);
    public void SetFloat(string key, float value) => PlayerPrefs.SetFloat(key, value);

    public string GetString(string key, string defaultValue = "") => PlayerPrefs.GetString(key, defaultValue);
    public void SetString(string key, string value) => PlayerPrefs.SetString(key, value);

    public void Flush() => PlayerPrefs.Save();
}
