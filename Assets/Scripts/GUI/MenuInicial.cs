using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicial : MonoBehaviour {
    public GameObject gameManagerPrefab; // referência ao prefab do GameManager

    private void Start() 
    {
        SetupGameManager();
    }

    private void SetupGameManager() 
    {
        // Verifique se já existe uma instância do GameManager
        if (GameManager.Instance == null) 
        {
            Instantiate(gameManagerPrefab);
        }
    }
    
    public void IniciarJogo() { // botao de "Jogar"
        // troca para a próxima cena (início do jogo)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void FecharJogo() { // botao de "Fechar"
        Debug.Log("Fechando...");
        
        // fechar aplicação
        Application.Quit();
    }

}
