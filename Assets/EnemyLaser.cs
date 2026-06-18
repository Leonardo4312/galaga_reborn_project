using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    public float speed = 8f; // Velocità del proiettile nemico

    void Update()
    {
        // Muove il proiettile verso il basso (Z decrescente)
        transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);

        // Auto-distruzione se esce dal fondo dello schermo per non intasare la memoria
        if (transform.position.z < -5f)
        {
            Destroy(gameObject);
        }
    }

    // Se il laser colpisce il Player
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);           // Il laser si distrugge
            
            //  Aggiunto null-check di sicurezza sul GameManager
            if (GameManager.instance != null)
            {
                GameManager.instance.PlayerHit();
            }
        }
    }
}