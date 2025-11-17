using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float acceleration = 10f;

    [Header("Salto")]
    [SerializeField] float jumpForce = 10f;
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.12f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] int maxJumps = 2;
    private int jumpsRemaining;

    [Header("Dash")]
    [SerializeField] float dashForce = 18f;
    [SerializeField] float dashDuration = 0.05f;
    [SerializeField] float dashCooldown = 0.5f;
    private float lastDash = -10f;
    private bool isDashing = false;

    [Header("Salud del Jugador")]
    [SerializeField] int maxHealth = 8; // ajustado a 8 para los corazones
    private int currentHealth;
    private HealthUI healthUI; // ✅ Referencia guardada

    [Header("Ataque Cuerpo a Cuerpo")]
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackRange = 0.8f;
    [SerializeField] int attackDamage = 1;
    [SerializeField] float attackCooldown = 0.4f;
    [SerializeField] LayerMask enemyLayer;
    private float lastAttack = -10f;

    [Header("Defensa")]
    [SerializeField] float defenseReduction = 0.5f;
    private bool isDefending = false;
    [SerializeField] float defenseDelay = 0.1f;

    Rigidbody2D rb;
    Animator anim;
    bool isGrounded;
    float horizontal;
    int facing = 1;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        jumpsRemaining = maxJumps;
    }

    void Start()
    {
        // ✅ Buscar una vez el HealthUI y actualizar corazones al inicio
        healthUI = Object.FindFirstObjectByType<HealthUI>();
        healthUI?.UpdateHearts(currentHealth);
    }

    void Update()
    {
        // Solo detenemos el Update si está en Dash
        if (isDashing) return;

        // --- INICIO DE LA CORRECCIÓN ---

        // Solo procesamos inputs (movimiento, salto, ataque) si el jugador está VIVO
        if (currentHealth > 0)
        {
            horizontal = Input.GetAxisRaw("Horizontal");

            // Comprobar suelo
            bool wasGrounded = isGrounded;
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
            if (!wasGrounded && isGrounded)
                jumpsRemaining = maxJumps;

            // ------------------- DEFENSA -------------------
            if (Input.GetKeyDown(KeyCode.Q))
                StartCoroutine(StartDefense());
            if (Input.GetKeyUp(KeyCode.Q))
                StopDefense();
            // ------------------------------------------------

            // No permitir atacar ni hacer dash si está defendiendo
            if (!isDefending)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time - lastDash >= dashCooldown && isGrounded)
                {
                    SetFacing(horizontal >= 0 ? 1 : -1);
                    StartCoroutine(PerformDash());
                }

                if (Input.GetButtonDown("Jump") && jumpsRemaining > 0)
                {
                    jumpsRemaining--;
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                }

                if (Input.GetKeyDown(KeyCode.E) && Time.time - lastAttack >= attackCooldown)
                {
                    lastAttack = Time.time;
                    anim?.SetTrigger("MeleeAttack");
                }
            }

            // Dirección
            if (horizontal > 0.1f) SetFacing(1);
            else if (horizontal < -0.1f) SetFacing(-1);
        }
        else
        {
            // Si estamos muertos, nos aseguramos de que 'horizontal' sea 0
            // para que la animación de "running" no se quede pegada.
            horizontal = 0f;
        }
        // --- FIN DE LA CORRECCIÓN ---


        // Actualizar animaciones
        // ESTE BLOQUE AHORA ESTÁ AFUERA DEL 'IF' Y SIEMPRE SE EJECUTA
        if (anim != null)
        {
            // Al morir, 'horizontal' será 0 y 'running' se pondrá en false
            anim.SetBool("running", Mathf.Abs(horizontal) > 0.1f);
            anim.SetBool("isGrounded", isGrounded);
            anim.SetBool("isDashing", isDashing);
            anim.SetBool("isDefending", isDefending);
            anim.SetFloat("yVelocity", rb.linearVelocity.y);
        }
    }

    void FixedUpdate()
    {
        if (isDashing || currentHealth <= 0 || isDefending) return;

        float targetVx = horizontal * moveSpeed;
        float vx = Mathf.Lerp(rb.linearVelocity.x, targetVx, acceleration * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(vx, rb.linearVelocity.y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Spikes"))
            DieImmediately();
    }

    void DieImmediately()
    {
        currentHealth = 0;
        Die();
        Debug.Log("¡El héroe murió al tocar las púas!");
    }

    IEnumerator PerformDash()
    {
        lastDash = Time.time;
        isDashing = true;
        float originalGravity = rb.gravityScale;

        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        rb.linearVelocity = new Vector2(facing * dashForce, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.5f, rb.linearVelocity.y);
    }

    // ---------------- DEFENSA ----------------
    IEnumerator StartDefense()
    {
        yield return new WaitForSeconds(defenseDelay);
        isDefending = true;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("Jugador está defendiendo 🛡️");
    }

    void StopDefense()
    {
        isDefending = false;
        Debug.Log("Jugador dejó de defenderse");
    }
    // -----------------------------------------

    public void TakeDamage(int damage)
    // Reemplaza tu función TakeDamage con esta:
    {
        // Si ya estamos muertos, no hacer nada
        if (currentHealth <= 0) return;

        // Calcular daño final (con defensa)
        int finalDamage = isDefending ? Mathf.CeilToInt(damage * defenseReduction) : damage;
        currentHealth -= finalDamage;

        // Actualizar la UI de corazones
        healthUI?.UpdateHearts(currentHealth);
        
        Debug.Log($"Vida restante: {currentHealth}");

        // --- LÓGICA CORREGIDA AQUÍ ---
        // Revisamos la vida DESPUÉS de hacer el daño
        if (currentHealth <= 0)
        {
            // 1. Si la vida es 0 o menos, solo morimos.
            // ¡Ya NO llamamos a "Hurt"!
            Die();
        }
        else
        {
            // 2. Si sobrevivimos al golpe, entonces sí
            // reproducimos la animación de "Hurt" o "Block".
            if (isDefending)
                anim?.SetTrigger("BlockHit");
            else
                anim?.SetTrigger("Hurt");
        }
    }

    public int GetCurrentHealth() => currentHealth;

    void Die()
    {
        Debug.Log("El héroe ha muerto. Iniciando Game Over.");
        anim?.SetTrigger("Die");
        StartCoroutine(GameOverSequence(2f));
    }

    IEnumerator GameOverSequence(float delay)
    {
        yield return new WaitForSeconds(0.3f);
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (delay > 0.3f)
            yield return new WaitForSeconds(delay - 0.3f);

        gameObject.SetActive(false);

        if (GameOverManager.Instance != null)
            GameOverManager.Instance.ShowGameOver();
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void DoAttack()
    {
        if (attackPoint == null) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<EnemySimple>();
            if (enemy != null)
                enemy.TakeDamage(attackDamage);
        }
    }

    void SetFacing(int dir)
    {
        if (facing == dir) return;
        facing = dir;
        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * dir;
        transform.localScale = s;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    public bool IsDefending() => isDefending;
}
