using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GUI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class Wave : MonoBehaviour
    {
        private TextMeshProUGUI waveText;
    
        private void Awake()
        {
            waveText = GetComponent<TextMeshProUGUI>();
        }

        public void SetWaveText(int wave)
        {
            waveText.text = $"Wave: {wave}";
        }
    }
}
