using UnityEngine;

[CreateAssetMenu(menuName = "Prototype2048/Game Config", fileName = "GameConfig")]
public sealed class GameConfig : ScriptableObject
{
    [Header("Spawn")]
    [Range(0f, 1f)] public float probability2 = 0.75f;

    [Header("Aim (drag)")]
    public float dragSensitivity = 0.015f;  
    public float aimMoveSpeed = 20f;      

    [Header("Launch")]
    public float launchImpulse = 12f;

    [Header("Merge")]
    public float minMergeImpulse = 2.5f;  
    public float pairCooldown = 0.10f;     
    public float absorbDuration = 0.14f;    

    [Header("Resolve / Settle")]
    public float settleSpeedThreshold = 0.15f;
    public float settleTime = 0.35f;

    [Header("Pool")]
    public int cubePrewarm = 24;
    public int fxPrewarm = 8;

    [Header("Juice")]
    public float popScale = 1.12f;
    public float popDuration = 0.10f;
    
    [Header("RewardGame")]
    public int rewardCoinsPerGame = 10;
    
}
