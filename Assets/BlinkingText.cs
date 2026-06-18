using System.Collections;
using UnityEngine;
using TMPro; // Obbligatorio per gestire TextMeshPro

public class BlinkingText : MonoBehaviour
{
    [Header("Velocità Lampeggio")]
    public float blinkInterval = 0.4f; // Tempo in secondi tra acceso e spento (più basso = più veloce)

    private TextMeshProUGUI textComponent;
    private Coroutine blinkCoroutine;

    void Awake()
    {
        // Prendo il componente di testo attaccato all'oggetto
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        // Ogni volta che il BSOD appare, resetto il testo visibile e faccio partire il lampeggio
        if (textComponent != null)
        {
            textComponent.enabled = true;
            blinkCoroutine = StartCoroutine(BlinkRoutine());
        }
    }

    void OnDisable()
    {
        // Quando la schermata blu scompare, fermo il timer per non sprecare memoria
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
    }

    IEnumerator BlinkRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(blinkInterval);
            if (textComponent != null)
            {
                // Inverte lo stato visivo del testo: se è acceso lo spegne, se è spento lo accende
                textComponent.enabled = !textComponent.enabled;
            }
        }
    }
}
