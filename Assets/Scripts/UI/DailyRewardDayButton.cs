using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardDayButton : MonoBehaviour
{
    [Header("UI")] 
    [SerializeField] private Button button;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private TMP_Text claimedText;

    [Header("Sprites")] 
    [SerializeField] private Sprite blueSprite;
    [SerializeField] private Sprite yellowSprite;

    private int _dayIndex;
    private IEventBus _bus;

    public void Construct(IEventBus bus) => _bus = bus;

    public void Setup(int dayIndex, int rewardCoins, Sprite icon)
    {
        _dayIndex = dayIndex;

        if (rewardText != null) rewardText.text = rewardCoins.ToString();
        if (iconImage != null && icon != null) iconImage.sprite = icon;

        if (claimedText != null) claimedText.gameObject.SetActive(false);

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }
    }
    private void OnClick()
    {
        _bus?.Publish(new ClaimDailyDayRequestedEvent(_dayIndex));
    }
    public void SetState(bool isToday, bool canClaim, bool isClaimed)
    {
        if (backgroundImage != null)
            backgroundImage.sprite = isToday ? yellowSprite : blueSprite;

        if (button != null) button.interactable = !isClaimed;
        
        if (rewardText != null)
            rewardText.gameObject.SetActive(!isClaimed);

        if (claimedText != null)
            claimedText.gameObject.SetActive(isClaimed);
    }
}