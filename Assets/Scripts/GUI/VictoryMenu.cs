using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class VictoryMenu : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    public void SetScore(int finalScore)
    {
        StartCoroutine(AnimateScore(0, finalScore, 1.0f)); // Anima de 0 até finalScore em 1 segundo
    }

    private IEnumerator AnimateScore(int startScore, int finalScore, float duration)
    {
        float currentTime = 0;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            int currentScore = (int)Mathf.Lerp(startScore, finalScore, currentTime / duration);
            scoreText.text = "Score: " + currentScore;
            yield return null;
        }
        scoreText.text = "Score: " + finalScore; // Garante que o valor final seja exato
    }
}
