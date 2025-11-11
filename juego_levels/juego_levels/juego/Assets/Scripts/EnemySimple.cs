using UnityEngine;

public class EnemySimple : MonoBehaviour
{
    [SerializeField] int health = 3; 
    [SerializeField] float deathDuration = 3.0f; 

    public void TakeDamage(int damage)
    {
        if (health <= 0) return;
        
        health -= damage;
        
        Debug.Log(gameObject.name + " tomó daño. Salud restante: " + health); 

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " ha sido derrotado.");
        
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Die"); 
        }
        
        EnemyController controller = GetComponent<EnemyController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        Destroy(gameObject, deathDuration); 
    }
}