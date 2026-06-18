using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; 
    public float spawnRate = 2f;    // Ogni quanti secondi nasce un nemico
    public float xLimit = 7f;       // Limite destro/sinistro per non farli nascere fuori schermo

    private float nextSpawnTime;

    void Update()
    {
        // Se è passato abbastanza tempo, fa nascere un nuovo alieno
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnRate; // Programma il prossimo lancio
        }
    }

    void SpawnEnemy()
    {
        // Sceglie una coordinata X casuale tra i limiti dello schermo
        float randomX = Random.Range(-xLimit, xLimit);
        
        // I nemici nascono in fondo allo schermo (Z = 15) alla X casuale
        Vector3 spawnPosition = new Vector3(randomX, 0f, 15f);

        // Fa nascere fisicamente il nemico nella scena
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
