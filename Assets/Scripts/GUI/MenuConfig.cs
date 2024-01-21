using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MenuConfig : MonoBehaviour {
    
    public AudioMixer mixer;

    public void SetVolume(float volume) { // aumentar/diminuir volume master
        mixer.SetFloat("masterVolume", volume);
    }

    public void SetGraficos(int indexQualidade) { // trocar qualidade dos graficos
        QualitySettings.SetQualityLevel(indexQualidade);
    }

    public void SetFullscreen(bool isFullscreen) { // entrar/sair de tela cheia
        Screen.fullScreen = isFullscreen;
    }
    
}
