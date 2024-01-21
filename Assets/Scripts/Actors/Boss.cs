using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// script p/testar barra de vida do Boss
public class Boss : MonoBehaviour {
    
    public int vidaMax = 100;
    public int vidaAtual;  
    public BarraHUD barraVida;

    void Start() {
        vidaAtual = vidaMax;
        barraVida.SetValMax(vidaMax);
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            TomarDano(10);
        }
    }

    void TomarDano(int dano) {
        vidaAtual -= dano;
        barraVida.SetVal(vidaAtual);
    }

}
