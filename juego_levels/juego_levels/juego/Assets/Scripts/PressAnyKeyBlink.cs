using UnityEngine;
using TMPro;

public class PressAnyKeyBlink : MonoBehaviour
{
    public TextMeshProUGUI pressKeyText;
    public float blinkSpeed = 1f;

    void Update()
    {
        // Efecto de parpadeo: cambia la opacidad
        float alpha = Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed));
        pressKeyText.alpha = alpha;
    }
}
