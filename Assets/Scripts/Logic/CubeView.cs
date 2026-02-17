using TMPro;
using UnityEngine;

public sealed class CubeView : MonoBehaviour
{
    [SerializeField] private TMP_Text valueText;

    public void SetValue(int value)
    {
        if (valueText != null) valueText.text = value.ToString();
    }
}