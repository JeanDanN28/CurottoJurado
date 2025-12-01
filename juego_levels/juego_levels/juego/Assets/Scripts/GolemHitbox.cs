using UnityEngine;

public class GolemHitbox : MonoBehaviour
{
    public int damage = 10;

    [Header("Audio")]
    public AudioClip hitSound;
    [Range(0f, 1f)] public float volume = 1f; // Control de volumen

    private bool hasHitPlayer = false;

    void OnEnable()
    {
        hasHitPlayer = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHitPlayer) return;

        if (other.CompareTag("Player"))
        {
            PlayerController playerHealth = other.GetComponent<PlayerController>();
            
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                
                // --- SONIDO FUERTE (MODO 2D) ---
                if (hitSound != null)
                {
                    // 1. Creamos un objeto temporal en la escena
                    GameObject soundObj = new GameObject("TempHitSound");
                    soundObj.transform.position = transform.position;

                    // 2. Le añadimos un AudioSource
                    AudioSource audioSource = soundObj.AddComponent<AudioSource>();
                    audioSource.clip = hitSound;
                    audioSource.volume = volume;
                    
                    // 3. ¡EL TRUCO! Ponemos Spatial Blend en 0 (Totalmente 2D)
                    // Esto hace que suene al máximo volumen, como la música de fondo.
                    audioSource.spatialBlend = 0f; 

                    // 4. Reproducir y programar destrucción
                    audioSource.Play();
                    Destroy(soundObj, hitSound.length);
                }

                hasHitPlayer = true; 
            }
        }
    }
}