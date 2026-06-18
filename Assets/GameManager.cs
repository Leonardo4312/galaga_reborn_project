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
    public GameObject loginPanel;         // Pannello 3: MODULO PLAYFAB

    [Header("Dynamic Sub-Menu Elements")]
    public GameObject volumeSlider;       
    public GameObject languageFlags;      

    [Header("Story Cutscene UI")]
    public GameObject storyPanel;        

    [Header("Loading Screen Settings")]
    public GameObject loadingPanel;       
    public TMP_Text loadingText;          
    public float loadingDuration = 5f;    

    [Header("Gameplay UI Master Container")]
    public GameObject gameplayUIPanel;    

    [Header("Gameplay UI Elements")]
    public TMP_Text scoreText; 
    public TMP_Text stageText; 
    public TMP_Text comboText;            
    
    [Tooltip("Espandi a 4 elementi nell'Inspector per includere Cuore_4 dell'Easter Egg!")]
    public GameObject[] heartImages;             

    [Header("Combo System Settings (ELITE HARDCORE)")]
    public float comboDuration = 1.5f;     
    private int currentMultiplier = 1;     
    private int comboCount = 0;            
    private Coroutine comboTimerCoroutine;   

    [Header("Game Over UI")]
    public GameObject gameOverPanel; 
    public RectTransform vortexTransform; 
    public TMP_Text mainMenuHighScoreText; 

    [Header("Localization - Main Menu")]
    public TMP_Text playBtnText;       
    public TMP_Text storyBtnText;      
    public TMP_Text optionsBtnText;    
    public TMP_Text volumeBtnText;     
    public TMP_Text languageBtnText;   
    public TMP_Text exitBtnText;       

    [Header("Localization - Story")]
    public TMP_Text storyContentText;  
    public TMP_Text storyBackBtnText;  // 🟢 NUOVO: Il testo del pulsante "INDIETRO" della storia!

    [Header("Localization - Login Panel (Aggiornato)")]
    public TMP_Text loginBtnText;          
    public TMP_Text signUpBtnText;         
    public TMP_Text loginBackBtnText;      
    public TMP_Text loginStatusText;       
    public TMP_Text usernamePlaceholder;   
    public TMP_Text passwordPlaceholder;   

    [Header("Localization - Game Over Panel")]
    public TMP_Text gameOverTitleText;     
    public TMP_Text retryBtnText;          
    public TMP_Text gameOverBackBtnText;   

    [HideInInspector] public bool isItalian = false;    

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
        
        if (comboText != null) comboText.gameObject.SetActive(false);

        // Imposta la lingua iniziale di default (Inglese)
        SetLanguageToEnglish();

        // --- GESTIONE DEI PANNELLI ALL'AVVIO ---
        if (startFromGameplay)
        {
            Time.timeScale = 1f; 
            SetAllPanelsInactive();
            
            if (gameplayUIPanel != null) gameplayUIPanel.SetActive(true); 
            if (playerInstance != null) playerInstance.SetActive(true);   
            
            StartCoroutine(MostraAnnuncioLivello());
        }
        else
        {
            Time.timeScale = 0f; 
            SetAllPanelsInactive();
            
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
            if (gameplayUIPanel != null) gameplayUIPanel.SetActive(false); 
            if (playerInstance != null) playerInstance.SetActive(false);   
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
        if (loadingPanel != null) loadingPanel.SetActive(false);
        
        if (volumeSlider != null) volumeSlider.SetActive(false);
        if (languageFlags != null) languageFlags.SetActive(false);
    }

    // --- SISTEMA DI TRADUZIONE DINAMICA ---

    public void SetLanguageToItalian()
    {
        isItalian = true;

        // Main Menu
        if (playBtnText != null) playBtnText.text = "GIOCA";
        if (storyBtnText != null) storyBtnText.text = "STORIA";
        if (optionsBtnText != null) optionsBtnText.text = "OPZIONI";
        if (volumeBtnText != null) volumeBtnText.text = "VOL. MUSICA";
        if (languageBtnText != null) languageBtnText.text = "LINGUA";
        if (exitBtnText != null) exitBtnText.text = "ESCI";

        // Storia background
        if (storyContentText != null)
        {
            storyContentText.text = "L'anno è il 2026. Lo spazio profondo è sotto l'attacco di incessanti ondate di sciami alieni ostili. Come avanguardia della Flotta Stellare, controlli la navicella da combattimento definitiva della galassia. La tua missione è semplice: distruggi le formazioni nemiche, schiva i letali attacchi kamikaze e conquista il punteggio record!\n\nMa tieni gli occhi aperti, pilota... La matrice di questo settore è altamente instabile. Gira voce di glitch di sistema nascosti e segreti sepolti nel profondo...\n\nRiuscirai a sopravvivere alla più grande minaccia della galassia?";
        }
        if (storyBackBtnText != null) storyBackBtnText.text = "INDIETRO"; // 🟢 Tradotto!

        // Login Panel
        if (loginBtnText != null) loginBtnText.text = "ACCEDI";
        if (signUpBtnText != null) signUpBtnText.text = "REGISTRATI";
        if (loginBackBtnText != null) loginBackBtnText.text = "INDIETRO";
        if (usernamePlaceholder != null) usernamePlaceholder.text = "NOME UTENTE";
        if (passwordPlaceholder != null) passwordPlaceholder.text = "PASSWORD";
        if (loginStatusText != null) loginStatusText.text = "In attesa di credenziali...";

        // Game Over Panel
        if (gameOverTitleText != null) gameOverTitleText.text = "PARTITA FINITA";
        if (retryBtnText != null) retryBtnText.text = "RIPROVA";
        if (gameOverBackBtnText != null) gameOverBackBtnText.text = "MENU PRINCIPALE";

        UpdateScoreUI();
        UpdateMainMenuHighScoreUI();
    }

    public void SetLanguageToEnglish()
    {
        isItalian = false;

        // Main Menu
        if (playBtnText != null) playBtnText.text = "PLAY";
        if (storyBtnText != null) storyBtnText.text = "STORY";
        if (optionsBtnText != null) optionsBtnText.text = "OPTIONS";
        if (volumeBtnText != null) volumeBtnText.text = "MUSIC VOLUME";
        if (languageBtnText != null) languageBtnText.text = "LANGUAGE";
        if (exitBtnText != null) exitBtnText.text = "EXIT";

        // Storia background
        if (storyContentText != null)
        {
            storyContentText.text = "The year is 2026. Deep space is under attack by relentless waves of hostile alien swarms. As the vanguard of the Starfleet, you control the galaxy's ultimate fighter ship. Your mission is simple: blast through enemy formations, dodge deadly kamikaze dives, and claim the ultimate high score!\n\nBut keep your eyes peeled, pilot... This sector matrix is highly unstable. Rumors speak of hidden system glitches and secrets buried deep within.\n\nCan you survive the galaxy's greatest secret?";
        }
        if (storyBackBtnText != null) storyBackBtnText.text = "BACK"; 

        // Login Panel
        if (loginBtnText != null) loginBtnText.text = "LOGIN";
        if (signUpBtnText != null) signUpBtnText.text = "SIGN-UP";
        if (loginBackBtnText != null) loginBackBtnText.text = "BACK";
        if (usernamePlaceholder != null) usernamePlaceholder.text = "USERNAME";
        if (passwordPlaceholder != null) passwordPlaceholder.text = "PASSWORD";
        if (loginStatusText != null) loginStatusText.text = "Awaiting credentials...";

        // Game Over Panel
        if (gameOverTitleText != null) gameOverTitleText.text = "GAME OVER";
        if (retryBtnText != null) retryBtnText.text = "RETRY";
        if (gameOverBackBtnText != null) gameOverBackBtnText.text = "MAIN MENU";

        UpdateScoreUI();
        UpdateMainMenuHighScoreUI();
    }

    // --- FLUSSO DI NAVIGAZIONE CON LOADING ---

    public void ClickPlayInMainMenu()
    {
        StartCoroutine(LoadingRoutine(loginPanel));
    }

    public void BackToMainMenuFromLogin()
    {
        if (loginPanel != null) loginPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void OnLoginVerified()
    {
        if (loginPanel != null) loginPanel.SetActive(false);
        
        if (gameplayUIPanel != null) gameplayUIPanel.SetActive(true); 
        if (playerInstance != null) playerInstance.SetActive(true);   

        Time.timeScale = 1f; 
        StartCoroutine(MostraAnnuncioLivello()); 
    }

    public void ClickOptionsInMainMenu()
    {
        StartCoroutine(LoadingRoutine(optionsMenuPanel));
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

    public void ClickStoryInMainMenu()
    {
        StartCoroutine(LoadingRoutine(storyPanel));
    }

    public void BackToMainMenuFromStory()
    {
        if (storyPanel != null) storyPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    private IEnumerator LoadingRoutine(GameObject targetPanel)
    {
        SetAllPanelsInactive();
        if (loadingPanel != null) loadingPanel.SetActive(true);

        float elapsed = 0f;
        float dotTimer = 0f;
        int dotCount = 0;

        string baseText = isItalian ? "CARICAMENTO" : "LOADING";
        if (loadingText != null) loadingText.text = baseText;

        while (elapsed < loadingDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            dotTimer += Time.unscaledDeltaTime;

            if (dotTimer >= 0.4f)
            {
                dotTimer = 0f;
                dotCount = (dotCount + 1) % 4;
                string dots = new string('.', dotCount);
                if (loadingText != null) loadingText.text = baseText + dots;
            }

            yield return null;
        }

        if (loadingPanel != null) loadingPanel.SetActive(false);
        if (targetPanel != null) targetPanel.SetActive(true);

        if (targetPanel == optionsMenuPanel)
        {
            if (volumeSlider != null) volumeSlider.SetActive(false);
            if (languageFlags != null) languageFlags.SetActive(false);
        }
    }

    // --- CONTROLLO AUDIO SLIDER ---
    public void SetMasterVolume(float value)
    {
        AudioListener.volume = value;
    }

    // --- SISTEMA DI PUNTEGGIO ---
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
            string label = isItalian ? "PUNTI" : "SCORE";
            scoreText.text = label + "\n" + score.ToString("D9"); 
        }
    }

    void UpdateMainMenuHighScoreUI()
    {
        if (mainMenuHighScoreText != null) 
        {
            string label = isItalian ? "RECORD  " : "HIGH-SCORE  ";
            mainMenuHighScoreText.text = label + highScore.ToString("D9");
        }
    }

    // --- SISTEMA DI COMBO ---
    public void RegisterEnemyKill(int basePoints)
    {
        comboCount++; 

        if (comboCount >= 30) currentMultiplier = 5;       
        else if (comboCount >= 20) currentMultiplier = 3;  
        else if (comboCount >= 10) currentMultiplier = 2;  
        else currentMultiplier = 1;                       

        if (comboText != null)
        {
            if (currentMultiplier > 1)
            {
                comboText.gameObject.SetActive(true);
                comboText.text = "X" + currentMultiplier + " COMBO!";
                StartCoroutine(FlashComboTextUI());
            }
            else
            {
                if (comboCount >= 3)
                {
                    comboText.gameObject.SetActive(true);
                    comboText.text = "STREAK x" + comboCount; 
                }
                else
                {
                    comboText.gameObject.SetActive(false); 
                }
            }
        }

        AddScore(basePoints * currentMultiplier);

        if (comboTimerCoroutine != null) StopCoroutine(comboTimerCoroutine);
        comboTimerCoroutine = StartCoroutine(ComboTimeoutRoutine());
    }

    IEnumerator FlashComboTextUI()
    {
        if (comboText == null) yield break;
        comboText.color = Color.white; 
        yield return new WaitForSeconds(0.08f);
        comboText.color = new Color(0f, 1f, 1f); 
    }

    IEnumerator ComboTimeoutRoutine()
    {
        yield return new WaitForSeconds(comboDuration);

        comboCount = 0;
        currentMultiplier = 1;
        
        if (comboText != null)
        {
            comboText.text = "";
            comboText.gameObject.SetActive(false);
        }
    }

    public void AddLife()
    {
        if (lives < 4) {
            lives++;
            int indiceCuore = lives - 1;
            if (heartImages != null && indiceCuore < heartImages.Length && heartImages[indiceCuore] != null)
            {
                heartImages[indiceCuore].SetActive(true);
            }
        }
    }

    public void PlayerHit()
    {
        if (CameraShake.instance != null)
        {
            CameraShake.instance.TriggerShake(0.4f, 0.5f);
        }

        lives--;
        if (heartImages != null && lives >= 0 && lives < heartImages.Length) StartCoroutine(FlashAndDisableHeart(heartImages[lives]));
        if (lives <= 0) { 
            if (gameplayUIPanel != null) gameplayUIPanel.SetActive(false); 
            if (playerInstance != null) Destroy(playerInstance); 
            TriggerGameOver(); 
        }
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

    void ResetHeartsUI() 
    { 
        if (heartImages == null) return; 
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] != null)
            {
                heartImages[i].SetActive(i < 3);
            }
        }
    }
}
