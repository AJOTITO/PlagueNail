using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText;
    private int currentScore = 0;

    private void OnEnable()
    {
        Target.OnScoreChanged += AddScore;
    }

    private void OnDisable()
    {
        Target.OnScoreChanged -= AddScore;
    }

    private void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore;
        }
    }
}