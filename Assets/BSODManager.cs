using System.Collections;
using UnityEngine;

public class BSODManager : MonoBehaviour
{
    [Header("UI Element")]
    public GameObject bsodPanel; // Trascina qui il tuo BSOD_Panel dall'Inspector

    [Header("Configurazione Trappola")]
    [Range(0f, 1f)] 
    public float probabilitaAltriLivelli = 0.4f; // 0.4 = 40% di possibilità dal livello 3 in poi

    private int ultimoLivelloControllato = 0;

    void Start()
    {
        // Ci assicuriamo che il pannello sia spento all'inizio del gioco
        if (bsodPanel != null)
            bsodPanel.SetActive(false);
    }

    void Update()
    {
        // Il sensore controlla costantemente il GameManager
        if (GameManager.instance != null)
        {
            int livelloAttualeGameManager = GameManager.instance.currentLevel;

            // Se il livello è cambiato rispetto all'ultimo controllo, attiviamo la logica!
            if (livelloAttualeGameManager != ultimoLivelloControllato)
            {
                ultimoLivelloControllato = livelloAttualeGameManager;
                GestisciTrappolaPerNuovoLivello(livelloAttualeGameManager);
            }
        }
    }

    void GestisciTrappolaPerNuovoLivello(int livello)
    {
        // Cancelliamo eventuali finti crash rimasti in coda dai livelli precedenti
        CancelInvoke("AttivaIlCrash");

        // LIVELLO 2: Appare SEMPRE (Garantito)
        if (livello == 2)
        {
            // Sceglie un secondo casuale (es. tra i 5 e i 15 secondi da quando inizia il Livello 2)
            float tempoCasuale = Random.Range(5f, 15f);
            Invoke("AttivaIlCrash", tempoCasuale);
            Debug.Log("Trappola BSOD piazzata per il Livello 2! Scatterà tra " + tempoCasuale + " secondi.");
        }
        // LIVELLI SUCCESSIVI (Dal 3 in poi): Casuale
        else if (livello > 2)
        {
            if (Random.value <= probabilitaAltriLivelli)
            {
                float tempoCasuale = Random.Range(10f, 25f);
                Invoke("AttivaIlCrash", tempoCasuale);
                Debug.Log("Trappola BSOD piazzata per il Livello " + livello + "! Scatterà tra " + tempoCasuale + " secondi.");
            }
        }
    }

    void AttivaIlCrash()
    {
        StartCoroutine(BSODRoutine());
    }

    IEnumerator BSODRoutine()
    {
        AudioSource musica = FindObjectOfType<AudioSource>();

        // 1. Togliamo la vita e attiviamo il respawn nativo del tuo GameManager
        if (GameManager.instance != null)
        {
            GameManager.instance.PlayerHit(); 
        }

        // 2. Stoppiamo la musica synthwave
        if (musica != null) musica.Pause();

        // 3. Spariamo la schermata blu a tutto schermo
        if (bsodPanel != null) bsodPanel.SetActive(true);

        // 4. Teniamo il blackout per 2 secondi (il gioco sotto continua!)
        yield return new WaitForSeconds(2f);

        // 5. Spegniamo lo schermo blu
        if (bsodPanel != null) bsodPanel.SetActive(false);

        // 6. Facciamo ripartire la musica a cannone
        if (musica != null) musica.UnPause();
    }
}
