using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DeathMenu : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    public void IniciarJogo()
    { // botao de "Jogar"
        // troca para a próxima cena (início do jogo)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void FecharJogo()
    { // botao de "Fechar (MainMenu e DeathMenu)"
        Debug.Log("Fechando...");
        // fechar aplicação
        Application.Quit();
    }

    public void RestartJogo()
    { // Botao de "Reiniciar"
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnMenu()
    { // Botao de "Menu"
        // Retorna à cena anterior
        SceneManager.LoadScene("Menu");
    }

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
