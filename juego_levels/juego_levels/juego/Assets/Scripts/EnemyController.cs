using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Objetivos")]
    [SerializeField] Transform playerTarget;

    [Header("Rango y Ataque")]
    [SerializeField] float attackRange = 8f;
    [SerializeField] float fireRate = 2f;
    private float nextFireTime;

    [Header("Proyectil")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] int projectileDamage = 1;

    private Animator anim;
    private int facing = -1;
    private string myTag = "Enemy";

    void Start()
    {
        anim = GetComponent<Animator>();
        if (playerTarget == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                playerTarget = playerObject.transform;
            }
        }
    }

    void Update()
    {
        if (playerTarget == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer <= attackRange)
        {
            LookAtPlayer();

            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    void LookAtPlayer()
    {
        if (playerTarget.position.x > transform.position.x)
            facing = 1;
        else
            facing = -1;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * facing;
        transform.localScale = scale;
    }

    void Shoot()
    {
        if (anim != null)
        {
            anim.SetTrigger("Shoot"); // el animation event o el Animator debe llamar a FireProjectile()
        }
        else
        {
            // si no hay animación, disparamos directamente
            FireProjectile();
        }
    }

    // Este método lo puedes llamar desde la animación (Animation Event) o se ejecuta directamente
    public void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogError("Projectile Prefab o Fire Point no asignados en el enemigo.");
            return;
        }

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        // Ajustar rotación/escala para que el sprite apunte donde toca
        Vector3 s = projectile.transform.localScale;
        s.x = Mathf.Abs(s.x) * (facing < 0 ? -1 : 1);
        projectile.transform.localScale = s;

        Projectile projScript = projectile.GetComponent<Projectile>();
        if (projScript != null)
        {
            projScript.Initialize(facing, projectileDamage, myTag);

            // Opcional: si quieres que el proyectil se mueva con una pequeña offset por la dirección:
            // projectile.transform.position = firePoint.position + Vector3.right * 0.1f * facing;
        }
    }
}
