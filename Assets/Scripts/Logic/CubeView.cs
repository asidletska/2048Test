using TMPro;
using UnityEngine;

[System.Serializable]
public struct CubeColorEntry
{
    public int value;
    public Color cubeColor;
    public Color textColor;
}

public sealed class CubeView : MonoBehaviour
{
    [SerializeField] private TMP_Text[] valueTexts;
    [SerializeField] private MeshRenderer cubeRenderer;
    [SerializeField] private CubeColorEntry[] colors;

    private Material _material;

    private void Awake()
    {
        if (cubeRenderer != null)
            _material = cubeRenderer.material;
    }

    public void SetValue(int value)
    {
        string str = value.ToString();
        
        for (int i = 0; i < valueTexts.Length; i++)
        {
            if (valueTexts[i] != null)
                valueTexts[i].text = str;
        }

        ApplyColor(value);
    }

    private void ApplyColor(int value)
    {
        CubeColorEntry entry = GetColorEntry(value);

        if (_material != null)
            _material.color = entry.cubeColor;

        for (int i = 0; i < valueTexts.Length; i++)
        {
            if (valueTexts[i] != null)
                valueTexts[i].color = entry.textColor;
        }
    }

    private CubeColorEntry GetColorEntry(int value)
    {
        for (int i = 0; i < colors.Length; i++)
        {
            if (colors[i].value == value)
                return colors[i];
        }

        return new CubeColorEntry
        {
            cubeColor = Color.white,
            textColor = Color.black
        };
    }
}