using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float lifetime = 2f;
    [SerializeField] bool canBounceWhenBlocked = false; // si true, rebota y pasa a ser "del jugador"

    int damage;
    int direction = 1;
    string ownerTag = "Enemy";

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Llamar desde quien instancia para inicializar dirección, daño y dueño
    public void Initialize(int dir, int dmg, string tag)
    {
        direction = dir;
        damage = dmg;
        ownerTag = tag;

        // Ajuste visual para que el sprite apunte hacia la dirección correcta
        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * (dir < 0 ? -1 : 1);
        transform.localScale = s;
    }

    void Update()
    {
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Ignorar colisiones con el dueño (por ejemplo enemigo con sus propios proyectiles)
        if (other.CompareTag(ownerTag))
            return;

        // Si el dueño es un enemigo y colisiona con el jugador
        if (ownerTag == "Enemy" && other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // Comprueba si el jugador está defendiendo (asegúrate que PlayerController exponga IsDefending())
                bool isDef = false;
                // intentando llamar a IsDefending() de manera segura:
                try
                {
                    isDef = player.IsDefending();
                }
                catch
                {
                    // Si no existe IsDefending() en la clase, asumimos false — cambia a tu nombre de método si hace falta.
                    Debug.LogWarning("PlayerController no expone IsDefending(). Añade 'public bool IsDefending()' o ajusta el nombre en Projectile.cs.");
                }

                if (isDef)
                {
                    Debug.Log("🛡️ Proyectil bloqueado por el jugador.");
                    if (canBounceWhenBlocked)
                    {
                        // Rebota: ahora pertenece al Player y cambia direccion
                        ownerTag = "Player";
                        direction = -direction;
                        // opcional: multiplicar velocidad al rebotar
                        speed *= 1.0f;
                        // ajustar escala visual
                        Vector3 s = transform.localScale;
                        s.x = Mathf.Abs(s.x) * (direction < 0 ? -1 : 1);
                        transform.localScale = s;
                        return; // no destruirlo ahora
                    }
                    else
                    {
                        Destroy(gameObject);
                        return;
                    }
                }

                // Si no está defendiendo, aplica daño directo
                // Asegúrate de que PlayerController tenga TakeDamage(int)
                try
                {
                    player.TakeDamage(damage);
                }
                catch
                {
                    Debug.LogWarning("PlayerController no tiene TakeDamage(int) público. Ajusta según tu implementación.");
                }
            }

            Destroy(gameObject);
            return;
        }

        // Si el dueño es el jugador y choca contra enemigo
        if (ownerTag == "Player" && other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<EnemySimple>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }
        }

        // Si choca con el suelo
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
