using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; 
using TMPro; 

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // --- VARIABILI STATICHE (Persistenza dei dati) ---
    public static bool startFromGameplay = false; 
    public static int highScore = 0; 

    [Header("Gameplay Settings")]
    public int lives = 3;                  
    public int currentLevel = 1; 
    private Vector3 originalSpawnPosition; 
    private int score = 0; 

    [Header("Menu Navigation Panels")]
    public GameObject mainMenuPanel;      // Pannello 1: PLAY, STORY, OPTIONS
    public GameObject optionsMenuPanel;   // Pannello 2: MUSIC VOLUME, LANGUAGE, EXIT
    public GameObject loginPanel;         // Pannello 3: MODULO VIOLA PLAYFAB

    [Header("Dynamic Sub-Menu Elements")]
    public GameObject volumeSlider;       
    public GameObject languageFlags;      

    [Header("Story Cutscene UI")]
    public GameObject storyPanel;        

    [Header("Gameplay UI")]
    public TMP_Text scoreText; 
    public TMP_Text stageText; 
    public GameObject[] heartImages;             

    [Header("Game Over UI")]
    public GameObject gameOverPanel; 
    public RectTransform vortexTransform; 
    public TMP_Text mainMenuHighScoreText; 

    [Header("Localization Texts (Trascina i figli Text TMP qui)")]
    public TMP_Text playBtnText;       // Il testo dentro il bottone PLAY
    public TMP_Text storyBtnText;      // Il testo dentro il bottone STORY
    public TMP_Text optionsBtnText;    // Il testo dentro il bottone OPTIONS
    public TMP_Text volumeBtnText;     // Il testo dentro il bottone MUSIC VOLUME
    public TMP_Text languageBtnText;   // Il testo dentro il bottone LANGUAGE
    public TMP_Text exitBtnText;       // Il testo dentro il bottone EXIT

    private bool isItalian = false;    // Di default il gioco parte in Inglese

    private GameObject playerInstance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        playerInstance = GameObject.FindGameObjectWithTag("Player");
        if (playerInstance != null)
        {
            originalSpawnPosition = playerInstance.transform.position;
        }

        ResetHeartsUI(); 
        UpdateScoreUI(); 
        UpdateMainMenuHighScoreUI(); 
        
        // --- LOGICA DI GESTIONE DEI PANNELLI ALL'AVVIO ---
        if (startFromGameplay)
        {
            Time.timeScale = 1f; 
            SetAllPanelsInactive();
            StartCoroutine(MostraAnnuncioLivello());
        }
        else
        {
            Time.timeScale = 0f; 
            SetAllPanelsInactive();
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        }
    }

    void SetAllPanelsInactive()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (optionsMenuPanel != null) optionsMenuPanel.SetActive(false);
        if (loginPanel != null) loginPanel.SetActive(false);
        if (storyPanel != null) storyPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (stageText != null) stageText.gameObject.SetActive(false);
        
        if (volumeSlider != null) volumeSlider.SetActive(false);
        if (languageFlags != null) languageFlags.SetActive(false);
    }

    // --- 🌍 SISTEMA DI TRADUZIONE DINAMICA ---

    // Funzione associata alla Bandierina Italiana
    public void SetLanguageToItalian()
    {
        isItalian = true;

        if (playBtnText != null) playBtnText.text = "GIOCA";
        if (storyBtnText != null) storyBtnText.text = "STORIA";
        if (optionsBtnText != null) optionsBtnText.text = "OPZIONI";
        if (volumeBtnText != null) volumeBtnText.text = "VOL. MUSICA";
        if (languageBtnText != null) languageBtnText.text = "LINGUA";
        if (exitBtnText != null) exitBtnText.text = "ESCI";

        // Aggiorna istantaneamente le scritte dei punteggi a schermo
        UpdateScoreUI();
        UpdateMainMenuHighScoreUI();
        Debug.Log("Lingua impostata: ITALIANO");
    }

    // Funzione associata alla Bandierina Inglese
    public void SetLanguageToEnglish()
    {
        isItalian = false;

        if (playBtnText != null) playBtnText.text = "PLAY";
        if (storyBtnText != null) storyBtnText.text = "STORY";
        if (optionsBtnText != null) optionsBtnText.text = "OPTIONS";
        if (volumeBtnText != null) volumeBtnText.text = "MUSIC VOLUME";
        if (languageBtnText != null) languageBtnText.text = "LANGUAGE";
        if (exitBtnText != null) exitBtnText.text = "EXIT";

        UpdateScoreUI();
        UpdateMainMenuHighScoreUI();
        Debug.Log("Language set to: ENGLISH");
    }


    // --- 🕹️ FLUSSO DI NAVIGAZIONE ---

    public void ClickPlayInMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (loginPanel != null) loginPanel.SetActive(true);
    }

    public void BackToMainMenuFromLogin()
    {
        if (loginPanel != null) loginPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void OnLoginVerified()
    {
        if (loginPanel != null) loginPanel.SetActive(false);
        
        Time.timeScale = 1f; 
        StartCoroutine(MostraAnnuncioLivello()); 
    }

    public void ClickOptionsInMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (optionsMenuPanel != null) optionsMenuPanel.SetActive(true);
        
        if (volumeSlider != null) volumeSlider.SetActive(false);
        if (languageFlags != null) languageFlags.SetActive(false);
    }

    public void BackToMainMenuFromOptions()
    {
        if (optionsMenuPanel != null) optionsMenuPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void ToggleVolume()
    {
        if (volumeSlider != null) volumeSlider.SetActive(!volumeSlider.activeSelf); 
    }

    public void ToggleLanguage()
    {
        if (languageFlags != null) languageFlags.SetActive(!languageFlags.activeSelf);
    }

    // --- CORE GAMEPLAY & RUNTIME UI ---
    public void ClickStoryInMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (storyPanel != null) storyPanel.SetActive(true);
    }

    public void BackToMainMenuFromStory()
    {
        if (storyPanel != null) storyPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreUI();
        if (score > highScore) { highScore = score; UpdateMainMenuHighScoreUI(); }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) 
        {
            // Traduzione dinamica della scritta Score durante la partita
            string label = isItalian ? "PUNTI" : "SCORE";
            scoreText.text = label + "\n" + score.ToString("D6"); 
        }
    }

    void UpdateMainMenuHighScoreUI()
    {
        if (mainMenuHighScoreText != null) 
        {
            // Traduzione dinamica del Record nei Menu
            string label = isItalian ? "RECORD  " : "HI-SCORE  ";
            mainMenuHighScoreText.text = label + highScore.ToString("D6");
        }
    }

    public void PlayerHit()
    {
        lives--;
        if (heartImages != null && lives >= 0 && lives < heartImages.Length) StartCoroutine(FlashAndDisableHeart(heartImages[lives]));
        if (lives <= 0) { if (playerInstance != null) Destroy(playerInstance); TriggerGameOver(); }
        else StartCoroutine(RespawnPlayer());
    }

    IEnumerator FlashAndDisableHeart(GameObject heart)
    {
        if (heart == null) yield break;
        for (int i = 0; i < 3; i++) { heart.SetActive(false); yield return new WaitForSecondsRealtime(0.12f); heart.SetActive(true); yield return new WaitForSecondsRealtime(0.12f); }
        heart.SetActive(false);
    }

    IEnumerator RespawnPlayer()
    {
        if (playerInstance != null) { playerInstance.SetActive(false); playerInstance.transform.position = originalSpawnPosition; Rigidbody rb = playerInstance.GetComponent<Rigidbody>(); if (rb != null) { rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero; } }
        yield return new WaitForSeconds(1.5f); 
        if (playerInstance != null && lives > 0) playerInstance.SetActive(true); 
    }

    public void AdvanceLevel() { currentLevel++; StartCoroutine(MostraAnnuncioLivello()); }

    IEnumerator MostraAnnuncioLivello()
    {
        if (stageText == null) yield break;
        stageText.gameObject.SetActive(true);
        
        // Traduzione dinamica della scritta Level a inizio livello
        string prefix = isItalian ? "LIVELLO " : "LEVEL ";
        stageText.text = prefix + currentLevel.ToString("D2");
        
        for (int i = 0; i < 4; i++) { stageText.enabled = !stageText.enabled; yield return new WaitForSeconds(0.2f); }
        stageText.enabled = true; yield return new WaitForSeconds(1.0f); stageText.gameObject.SetActive(false);
    }

    public void TriggerGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true); Time.timeScale = 0f; 
        if (vortexTransform != null) { vortexTransform.gameObject.SetActive(true); StartCoroutine(AnimaVorticeNero()); }
    }

    public void RetryGame() { startFromGameplay = true; Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void GoToMainMenu() { startFromGameplay = false; Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }

    IEnumerator AnimaVorticeNero()
    {
        vortexTransform.localScale = Vector3.zero; float tempo = 0f;
        while (tempo < 1.5f) { tempo += Time.unscaledDeltaTime; float p = tempo / 1.5f; vortexTransform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(50f, 50f, 50f), p); vortexTransform.Rotate(0f, 0f, 600f * Time.unscaledDeltaTime); yield return null; }
    }

    void ResetHeartsUI() { if (heartImages == null) return; foreach (GameObject h in heartImages) { if (h != null) h.SetActive(true); } }
}