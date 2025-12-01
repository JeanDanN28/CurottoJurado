using UnityEngine;
using System.Collections; // Necesario para Corrutinas
using UnityEngine.SceneManagement; // Necesario para cambiar de nivel

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
    public GameObject handHitbox;
    public float attackDistance = 2f;
    public int maxHP = 100;
    private int currentHP;

    // --- AUDIO Y NIVEL ---
    [Header("Audio y Nivel")]
    public AudioSource audioSource; 
    public AudioClip deathSound;    
    public float waitTimeBeforeLevel = 4f;
    
    public string nextLevelName; // <-- AQUI LA PUSE: Escribe el nombre exacto en el Inspector
    // ----------------------------

    private bool isDead = false;
    private bool isAttacking = false;
    private bool specialUsed = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        currentHP = maxHP;
        
        if (handHitbox != null)
            handHitbox.SetActive(false); 

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
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

    void CheckGround() { isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer); }
    private void OnDrawGizmosSelected() { if (groundCheck != null) { Gizmos.color = Color.green; Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius); } }

    void MoveTowardsPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)dir * speed * Time.deltaTime;
        Vector3 currentScale = transform.localScale;
        if (dir.x > 0) currentScale.x = -Mathf.Abs(currentScale.x);
        else if (dir.x < 0) currentScale.x = Mathf.Abs(currentScale.x);
        transform.localScale = currentScale;
    }

    void StartAttack() { isAttacking = true; anim.SetTrigger("AttackTrigger"); }
    public void EndAttack() { isAttacking = false; }

    public void EnableAttackHitbox() { if (handHitbox != null) handHitbox.SetActive(true); }
    public void DisableAttackHitbox() { if (handHitbox != null) handHitbox.SetActive(false); }

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
        Destroy(GetComponent<Rigidbody2D>());

        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        StartCoroutine(LevelTransitionRoutine());
    }

    IEnumerator LevelTransitionRoutine()
    {
        Debug.Log("El Boss ha muerto. Esperando para cambiar de nivel...");
        
        yield return new WaitForSeconds(waitTimeBeforeLevel);

        Debug.Log("Cambiando al nivel: " + nextLevelName);
        
        // --- CAMBIO AQUI ---
        // Ahora usamos el nombre que escribiste en la variable, no el índice.
        if (!string.IsNullOrEmpty(nextLevelName))
        {
            SceneManager.LoadScene(nextLevelName);
        }
        else
        {
            Debug.LogError("¡ERROR! No has escrito el nombre del siguiente nivel en el Inspector del Boss.");
        }
    }

    public void Jump() { if (!isDead) anim.SetTrigger("JumpTrigger"); }
    public void Climb() { if (!isDead) anim.SetTrigger("ClimbTrigger"); }
    public void Talk() { anim.SetTrigger("TalkTrigger"); }
}