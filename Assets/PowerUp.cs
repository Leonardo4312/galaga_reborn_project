using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public float speed = 3f; // Velocità di caduta del bonus

    void Update()
    {
        // Fa cadere il bonus verso il basso (-Z)
        transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);

        // Si distrugge se esce dallo schermo senza essere preso
        if (transform.position.z < -5f)
        {
            Destroy(gameObject);
        }
    }

    // Usiamo OnTriggerEnter così il bonus passa attraverso il player senza spostarlo fisicamente
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            var playerScript = other.GetComponent<PlayerGalaga>();
            
            if (playerScript != null)
            {
                playerScript.ActivateDoubleLaser(); // Attiva il doppio cannone!
            }

            Destroy(gameObject); // Il bonus scompare perché è stato raccolto
        }
    }
}
