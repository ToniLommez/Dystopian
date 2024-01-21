using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarraHUD : MonoBehaviour {
    public Slider barra;

    public void Init(int max) {
        SetValMax(max);
        SetVal(max);
    }

    public void SetValMax(int valMax) {
        barra.maxValue = valMax;
        barra.value = valMax;
    }

    public void SetVal(int val) {
        barra.value = val;
    }

}
