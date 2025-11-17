using UnityEngine;

public class GolemHitbox : MonoBehaviour
{
    // Puedes ajustar el daño desde el Inspector
    public int damage = 10;
    
    // Usamos esto para que un solo swing no golpee al jugador 10 veces
    private bool hasHitPlayer = false;

    // Esto se llama automáticamente cuando el objeto se activa
    void OnEnable()
    {
        // Reseteamos el "hit" cada vez que el arma empieza un swing
        hasHitPlayer = false;
    }

    // Esto se llama cuando el Trigger detecta a ALGO
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si ya golpeamos en este swing, salimos
        if (hasHitPlayer)
        {
            return;
        }

        // Comprobamos si lo que golpeamos tiene el tag "Player"
        if (other.CompareTag("Player"))
        {

            PlayerController playerHealth = other.GetComponent<PlayerController>();
            
            if (playerHealth != null)
            {
                // ¡Le hacemos daño!
                playerHealth.TakeDamage(damage);
                
                // Marcamos que ya golpeamos, para no repetir
                hasHitPlayer = true; 
            }
        }
    }
}