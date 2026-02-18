using UnityEngine;

public class PanelView : MonoBehaviour
{
    [SerializeField] private UiPanelId id;
    [SerializeField] private GameObject root;

    public UiPanelId Id => id;

    private void Reset()
    {
        root = gameObject;
    }

    public void Show()
    {
        if (root != null) root.SetActive(true);
        else gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (root != null) root.SetActive(false);
        else gameObject.SetActive(false);
    }

    public bool IsVisible => (root != null ? root.activeSelf : gameObject.activeSelf);
}
