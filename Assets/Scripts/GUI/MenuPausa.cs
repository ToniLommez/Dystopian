using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{

    public static bool isPausado = false;
    public GameObject menuPausa;

    private void Start()
    {
        menuPausa.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPausado)
            {
                Retomar();
            }
            else
            {
                Pausar();
            }
        }
    }

    public void Retomar()
    {
        menuPausa.SetActive(false);
        Time.timeScale = 1f;
        isPausado = false;
    }

    void Pausar()
    {
        menuPausa.SetActive(true);
        Time.timeScale = 0f;
        isPausado = true;
    }

    void PauseToggle()
    {
        if (isPausado)
        {
            Retomar();
        }
        else
        {
            Pausar();
        }
    }

    public void RestartJogo()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CarregarMenu()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1f;
    }

    public void FecharJogo()
    {
        Application.Quit();
    }

    public void VoltarMenu()
    {
        isPausado = false;
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
