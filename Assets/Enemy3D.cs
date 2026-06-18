using UnityEngine;

public class Enemy3D : MonoBehaviour
{
    private Rigidbody rb;
    
    [Header("Movement Settings")]
    public float speed = 4f;              // Velocità di discesa in picchiata
    public float zigZagSpeed = 5f;        // Velocità dello zig-zag
    public float zigZagWidth = 2f;        // Larghezza dello zig-zag

    [Header("Shooting Settings")]
    public GameObject enemyLaserPrefab;   // Prefab del proiettile dell'alieno
    public float fireRate = 1.5f;         // Frequenza di sparo in picchiata
    private float fireTimer;

    [Header("Score Settings")]
    public int scoreValue = 100;          // Punti assegnati al GameManager

    [Header("Effects Settings")]
    public GameObject explosionPrefab;    // Sistema particellare dell'esplosione

    [Header("Power-Up Settings")]
    public GameObject powerUpPrefab;      // Il cubetto verde del Bonus da rilasciare

    // Variabili interne per la gestione dello stacco dalla griglia
    private bool isDiving = false;
    private Transform originalParent;
    private Vector3 originalLocalPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 🟢 MODIFICATO: Congela completamente la rotazione fisica.
        // Impedisce all'alieno di girarsi su se stesso o ribaltarsi se urta qualcosa
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    public void StartDive()
    {
        if (isDiving) return; 

        isDiving = true;
        originalParent = transform.parent;
        originalLocalPosition = transform.localPosition;
        
        // stacco dalla griglia per essere indipendenti nei movimenti
        transform.parent = null;

        // Inizializza il timer di sparo con un leggero ritardo casuale
        fireTimer = Random.Range(0.2f, fireRate);
    }

    void Update()
    {
        if (isDiving)
        {
            // Movimento matematico a zig-zag ondulatorio
            float xMovement = Mathf.Sin(Time.time * zigZagSpeed) * zigZagWidth;
            Vector3 movement = new Vector3(xMovement, 0f, -speed);
            transform.Translate(movement * Time.deltaTime, Space.World);

            // Gestione della ricarica delle armi aliene
            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0f)
            {
                Shoot();
                fireTimer = fireRate; 
            }

            // Se supera la linea difensiva del giocatore rientra in alto nella formazione
            if (transform.position.z < -2f)
            {
                ReturnToGrid();
            }
        }
    }

    void Shoot()
    {
        if (enemyLaserPrefab != null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0f, 0f, -0.6f);
            Instantiate(enemyLaserPrefab, spawnPosition, Quaternion.identity);
        }
    }

    void ReturnToGrid()
    {
        isDiving = false;
        transform.parent = originalParent;
        transform.localPosition = originalLocalPosition;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // CASO 1: L'alieno viene polverizzato dal laser del giocatore
        if (collision.gameObject.CompareTag("Laser")) 
        {
            // 1. Chiama RegisterEnemyKill per svegliare le combo e il moltiplicatore
            if (GameManager.instance != null)
            {
                GameManager.instance.RegisterEnemyKill(scoreValue);
            }

            // SCREEN SHAKE: Telecamera scossa leggermente per l'esplosione aliena
            if (CameraShake.instance != null)
            {
                CameraShake.instance.TriggerShake(0.12f, 0.15f); 
            }

            // 2. Genera la fiammata di particelle
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }

            // 3. MECCANICA DROP: cade sempre se il prefab è assegnato
            if (powerUpPrefab != null) 
            {
                Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
            }

            // 4. Distruzione fisica dei vecchi elementi
            Destroy(collision.gameObject); 
            Destroy(gameObject);           
        }

        // CASO 2: L'alieno si schianta frontalmente contro il Player
        if (collision.gameObject.CompareTag("Player"))
        {
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);           
            
            if (GameManager.instance != null)
            {
                GameManager.instance.PlayerHit();
            }
        }
    }
}