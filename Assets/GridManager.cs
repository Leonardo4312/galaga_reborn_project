using UnityEngine;
using System.Collections;

public class GridManager : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject beePrefab;       
    public GameObject butterflyPrefab; 
    public GameObject bossPrefab;      

    [Header("Grid Settings")]
    private int rows = 5;
    private int cols = 8;
    public float spacingX = 1.5f;     
    public float spacingZ = 1.1f;     

    [Header("Movement Settings")]
    public float speed = 2f;          
    public float distance = 1.5f;     
    private Vector3 startPosition;

    [Header("Attack Settings")]
    public float attackInterval = 3f;  
    private float attackTimer;

    private bool isTransitioning = false; // Evita di far partire più ondate contemporaneamente
    private bool gridSpawnedAtLeastOnce = false;

    void Start()
    {
        startPosition = transform.position; 
        GenerateGrid();
        attackTimer = attackInterval; 
    }

    void Update()
    {
        // Movimento di oscillazione della griglia
        float newX = startPosition.x + Mathf.Sin(Time.time * speed) * distance;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        // Timer di attacco delle picchiate
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            LaunchRandomEnemy();
            // Più saliamo di livello, più gli attacchi diventano frequenti (fino a un massimo di ogni 0.8 secondi)
            float currentInterval = Mathf.Max(0.8f, attackInterval - (GameManager.instance.currentLevel - 1) * 0.3f);
            attackTimer = currentInterval;
        }

        // --- CONTROLLO CAMBIO LIVELLO ---
        if (gridSpawnedAtLeastOnce && !isTransitioning)
        {
            // Cerca se ci sono ancora script Enemy3D vivi nella scena (sia in griglia che in volo)
            Enemy3D[] remainingEnemies = Object.FindObjectsByType<Enemy3D>(FindObjectsSortMode.None);

            if (remainingEnemies.Length == 0)
            {
                StartCoroutine(NextLevelSequence());
            }
        }
    }

    IEnumerator NextLevelSequence()
    {
        isTransitioning = true;
        
        yield return new WaitForSeconds(2.0f); // 2 secondi di pausa drammatica dopo aver ucciso l'ultimo nemico

        // Comunica al GameManager di avanzare di livello
        GameManager.instance.AdvanceLevel();

        // Resetta la posizione del manager al centro per la nuova griglia
        transform.position = startPosition;

        // Fai nascere la nuova armata
        GenerateGrid();

        isTransitioning = false;
    }

    void LaunchRandomEnemy()
    {
        Enemy3D[] activeEnemies = GetComponentsInChildren<Enemy3D>();

        if (activeEnemies.Length > 0)
        {
            int randomIndex = Random.Range(0, activeEnemies.Length);
            activeEnemies[randomIndex].StartDive();
        }
    }

    void GenerateGrid()
    {
        gridSpawnedAtLeastOnce = false;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (row == 4 && (col < 2 || col > 5)) continue;

                GameObject prefabToSpawn = beePrefab; 

                if (row == 2 || row == 3) prefabToSpawn = butterflyPrefab;  
                else if (row == 4) prefabToSpawn = bossPrefab;       

                float posX = (col - (cols - 1) / 2f) * spacingX;
                float posZ = (row - (rows - 1) / 2f) * spacingZ;

                Vector3 spawnPosition = transform.position + new Vector3(posX, 0f, posZ);

                GameObject newEnemy = Instantiate(prefabToSpawn, spawnPosition, prefabToSpawn.transform.rotation);
                newEnemy.transform.parent = transform;

                // --- MODIFICA DIFFICOLTÀ DINAMICA ---
                Enemy3D enemyScript = newEnemy.GetComponent<Enemy3D>();
                if (enemyScript != null)
                {
                    int lvl = GameManager.instance.currentLevel;
                    // Aumenta la velocità di picchiata di +0.8 per ogni livello successivo al primo
                    enemyScript.speed += (lvl - 1) * 0.8f;
                    // Riduce il tempo di ricarica dello sparo (sparano più velocemente, fino a un tetto massimo di 0.4s)
                    enemyScript.fireRate = Mathf.Max(0.4f, enemyScript.fireRate - (lvl - 1) * 0.15f);
                }
            }
        }

        gridSpawnedAtLeastOnce = true;
    }
}
