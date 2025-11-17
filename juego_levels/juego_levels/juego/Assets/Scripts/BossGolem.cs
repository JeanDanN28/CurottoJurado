using UnityEngine;

public class BossGolem : MonoBehaviour
{
    [Header("Referencias")]
    public Animator anim;
    public Transform player;

    [Header("Movimiento")]
    public float speed = 1.5f;

    [Header("Check Ground")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.12f;
    public LayerMask groundLayer;
    public bool isGrounded;

    [Header("Combate")]
    public GameObject handHitbox; // <-- NUEVO: Arrastra tu "HandHitbox" aquí
    public float attackDistance = 2f;
    public int maxHP = 100;
    private int currentHP;

    private bool isDead = false;
    private bool isAttacking = false;
    private bool specialUsed = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        currentHP = maxHP;
        // <-- NUEVO: Desactivamos el hitbox al empezar, por si acaso
        if (handHitbox != null)
            handHitbox.SetActive(false); 
    }

    void Update()
    {
        if (isDead) return;

        CheckGround();
        anim.SetBool("isGrounded", isGrounded);

        float dist = Vector2.Distance(transform.position, player.position);

        // --- LÓGICA DE MOVIMIENTO ---
        bool shouldWalk = !isAttacking && (dist > attackDistance);
        anim.SetBool("isWalking", shouldWalk);
        if (shouldWalk)
        {
            MoveTowardsPlayer();
        }

        // --- Ataque normal ---
        if (dist <= attackDistance && !isAttacking && isGrounded)
        {
            StartAttack();
        }

        // --- Fase especial ---
        if (!specialUsed && currentHP <= maxHP / 2)
        {
            specialUsed = true;
            anim.SetTrigger("SpecialATrigger");
        }
    }

    // -------------------------
    // GROUND CHECK
    // -------------------------
    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    // -------------------------
    // MOVIMIENTO
    // -------------------------
    void MoveTowardsPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;

        transform.position += (Vector3)dir * speed * Time.deltaTime;

        // ---------- LÓGICA DE GIRO CORREGIDA ----------

        // 1. Obtenemos la escala actual que pusiste en el editor
        Vector3 currentScale = transform.localScale;
        
        // 2. Si el jugador está a la DERECHA (dir.x > 0)...
        if (dir.x > 0)
        {
            // ...ponemos la 'x' en negativo (para que mire a la DERECHA)
            // Usamos Mathf.Abs para asegurarnos de que el valor sea siempre positivo
            // antes de volverlo negativo.
            currentScale.x = -Mathf.Abs(currentScale.x);
        }
        // 3. Si el jugador está a la IZQUIERDA (dir.x < 0)...
        else if (dir.x < 0)
        {
            // ...ponemos la 'x' en positivo (para que mire a la IZQUIERDA)
            currentScale.x = Mathf.Abs(currentScale.x);
        }

        // 4. Aplicamos la nueva escala
        transform.localScale = currentScale;
    }

    // -------------------------
    // ATAQUE
    // -------------------------
    void StartAttack()
    {
        isAttacking = true;
        anim.SetTrigger("AttackTrigger");
    }

    // Llamar desde Animation Event
    public void EndAttack()
    {
        isAttacking = false;
    }

    // ----------------------------------------------------
    // <-- NUEVAS FUNCIONES PARA EVENTOS DE ANIMACIÓN -->
    // ----------------------------------------------------
    
    // Esta función la llamarás desde el Evento de Animación
    public void EnableAttackHitbox()
    {
        if (handHitbox != null)
            handHitbox.SetActive(true);
    }

    // Esta también la llamarás desde el Evento de Animación
    public void DisableAttackHitbox()
    {
        if (handHitbox != null)
            handHitbox.SetActive(false);
    }

    // -------------------------
    // DAÑO
    // -------------------------
    public void TakeDamage(int dmg)
    {
        if (isDead) return;
        currentHP -= dmg;
        anim.SetTrigger("StunedTrigger");
        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        anim.SetTrigger("DeathTrigger");
        Destroy(GetComponent<Collider2D>());
    }

    // -------------------------
    // OPCIONALES (Sin cambios)
    // -------------------------
    public void Jump()
    {
        if (isDead) return;
        anim.SetTrigger("JumpTrigger");
    }

    public void Climb()
    {
        if (isDead) return;
        anim.SetTrigger("ClimbTrigger");
    }

    public void Talk()
    {
        anim.SetTrigger("TalkTrigger");
    }
}