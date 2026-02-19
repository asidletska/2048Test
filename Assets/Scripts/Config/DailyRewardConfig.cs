using UnityEngine;

[CreateAssetMenu(menuName = "", fileName = "")]
public sealed class DailyRewardsConfig : ScriptableObject
{
    public DailyRewardDay[] days;
    public int cooldownHours = 24;
}

[System.Serializable]
public struct DailyRewardDay
{
    public int coins;
    public Sprite icon;
}
