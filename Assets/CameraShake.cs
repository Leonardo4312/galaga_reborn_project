using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    private Vector3 originalLocalPosition;

    void Awake()
    {
        
        instance = this;
    }

    void Start()
    {
        // Registriamo la posizione iniziale della telecamera
        originalLocalPosition = transform.localPosition;
    }

    
    public void TriggerShake(float duration, float magnitude)
    {
        // Ferma eventuali shake precedenti ancora in corso per non glitchare
        StopAllCoroutines(); 
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // Genera uno spostamento casuale in 3D usando una sfera immaginaria
            Vector3 randomOffset = Random.insideUnitSphere * magnitude;

            // Applica lo spostamento mantenendo la telecamera ancorata alla sua base
            transform.localPosition = originalLocalPosition + randomOffset;

            elapsed += Time.deltaTime;

            // Aspetta il frame successivo
            yield return null;
        }

        // Finito il tempo, rimetti la telecamera esattamente dove si trovava
        transform.localPosition = originalLocalPosition;
    }
}
