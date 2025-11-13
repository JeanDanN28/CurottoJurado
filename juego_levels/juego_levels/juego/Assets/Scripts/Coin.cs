using UnityEngine;

public class Coin : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player"))
        {
            
            Debug.Log("Moneda recogida!");

            
            Destroy(gameObject);
        }
    }
}
