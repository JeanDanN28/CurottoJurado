using UnityEngine;

public class GolemHitbox : MonoBehaviour
{
    
    public int damage = 10;
    
    
    private bool hasHitPlayer = false;

    
    void OnEnable()
    {
        
        hasHitPlayer = false;
    }

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (hasHitPlayer)
        {
            return;
        }

        
        if (other.CompareTag("Player"))
        {

            PlayerController playerHealth = other.GetComponent<PlayerController>();
            
            if (playerHealth != null)
            {
               
                playerHealth.TakeDamage(damage);
                
                
                hasHitPlayer = true; 
            }
        }
    }
}