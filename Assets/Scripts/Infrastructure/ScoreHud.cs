using TMPro;
using UnityEngine;

public sealed class ScoreHud : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    public void SetScore(int score)
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }
}