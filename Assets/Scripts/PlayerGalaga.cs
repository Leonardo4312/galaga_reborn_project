using UnityEngine;

public class PlayerGalaga : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 8f;              
    public float xLimit = 7.5f;          

    [Header("Mobile Touch Settings")]
    [Tooltip("Regola la sensibilità del trascinamento su smartphone (es. tra 0.02 e 0.06)")]
    public float touchSensitivity = 0.04f;

    [Header("Shooting Settings")]
    public GameObject laserPrefab;       
    public float fireRate = 0.25f;        
    private float nextFireTime = 0f;

    [Header("Power-Up Status")]
    public bool hasDoubleLaser = false;  

    private float originalZ; 
    private Rigidbody rb; 

    void Start()
    {
        // Memorizziamo la Z di partenza per evitare che la fisica ci spinga indietro
        originalZ = transform.position.z;

        // Recuperiamo il Rigidbody attaccato alla navicella
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 1. GESTIONE MOVIMENTO (Touch per Xiaomi / Tastiera per Mac Editor)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            // Se il dito si muove sullo schermo, sposta la navicella di conseguenza
            if (touch.phase == TouchPhase.Moved)
            {
                float touchInput = touch.deltaPosition.x * touchSensitivity;
                transform.Translate(new Vector3(touchInput, 0f, 0f), Space.World);
            }
        }
        else
        {
            // Se non tocchi lo schermo, ripiega sui comandi da tastiera del Mac
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            Vector3 direction = new Vector3(horizontalInput, 0f, 0f);
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }

        // Azzeriamo all'istante qualsiasi forza fisica accumulata da urti o esplosioni
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 2. BLOCCO DEI BORDI (X) E ANCORAGGIO FISSO (Z)
        float clampedX = Mathf.Clamp(transform.position.x, -xLimit, xLimit);
        
        // Forza la navicella a stare sulla sua X controllata e SEMPRE sulla Z originale
        transform.position = new Vector3(clampedX, transform.position.y, originalZ);

        // 3. INPUT DI SPARO (Spazio su Mac / Auto-Sparo quando tocchi lo schermo sullo Xiaomi!)
        if ((Input.GetButton("Jump") || Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0) && Time.time > nextFireTime)
        {
            nextFireTime = Time.time + fireRate; 
            Shoot();
        }
    }

    void Shoot()
    {
        if (laserPrefab != null)
        {
            if (hasDoubleLaser)
            {
                Vector3 leftSpawn = transform.position + new Vector3(-0.3f, 0f, 0.5f);
                Vector3 rightSpawn = transform.position + new Vector3(0.3f, 0f, 0.5f);

                Instantiate(laserPrefab, leftSpawn, Quaternion.identity);
                Instantiate(laserPrefab, rightSpawn, Quaternion.identity);
            }
            else
            {
                Vector3 singleSpawn = transform.position + new Vector3(0f, 0f, 0.5f);
                Instantiate(laserPrefab, singleSpawn, Quaternion.identity);
            }
        }
    }

    // Questa è la funzione esatta cercata da PowerUp.cs - Errore Console Risolto!
    public void ActivateDoubleLaser()
    {
        hasDoubleLaser = true;
    }
}
