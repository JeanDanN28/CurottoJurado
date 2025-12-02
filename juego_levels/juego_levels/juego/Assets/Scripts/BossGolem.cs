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
    public AudioClip attackSound; // Opcional: Sonido Swing
    public float waitTimeBeforeLevel = 4f;
    
    public string nextLevelName; // Escribe el nombre exacto de la siguiente escena aquí
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
        // Solo camina si NO está atacando Y está lejos del jugador
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

        // --- Fase especial (al 50% de vida) ---
        if (!specialUsed && currentHP <= maxHP / 2)
        {
            specialUsed = true;
            anim.SetTrigger("SpecialATrigger");
        }
    }

    void CheckGround() 
    { 
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer); 
    }

    private void OnDrawGizmosSelected() 
    { 
        if (groundCheck != null) 
        { 
            Gizmos.color = Color.green; 
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius); 
        } 
    }

    void MoveTowardsPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)dir * speed * Time.deltaTime;
        
        Vector3 currentScale = transform.localScale;
        if (dir.x > 0) currentScale.x = -Mathf.Abs(currentScale.x);
        else if (dir.x < 0) currentScale.x = Mathf.Abs(currentScale.x);
        transform.localScale = currentScale;
    }

    void StartAttack() 
    { 
        isAttacking = true; 
        anim.SetTrigger("AttackTrigger"); 
    }

    // Llamado por Animation Event al final del ataque
    public void EndAttack() 
    { 
        isAttacking = false; 
    }

    // Llamado por Animation Event en el frame del golpe
    public void EnableAttackHitbox() 
    { 
        if (handHitbox != null) 
            handHitbox.SetActive(true); 
            
        // Sonido de Swing/Ataque (Opcional)
        if (audioSource != null && attackSound != null)
            audioSource.PlayOneShot(attackSound);
    }

    public void DisableAttackHitbox() 
    { 
        if (handHitbox != null) 
            handHitbox.SetActive(false); 
    }

    // ---------------------------------------------------------
    // CORRECCIÓN PRINCIPAL AQUÍ:
    // ---------------------------------------------------------
    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        // 1. SI ESTABA ATACANDO, EL GOLPE LO INTERRUMPE
        if (isAttacking)
        {
            isAttacking = false; // "Olvidamos" que estaba atacando
            DisableAttackHitbox(); // Apagamos el daño de la mano inmediatamente
        }

        currentHP -= dmg;
        anim.SetTrigger("StunedTrigger"); // Reproducir animación de dolor

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        anim.SetTrigger("DeathTrigger");
        
        // Limpieza de componentes físicos
        Destroy(GetComponent<Collider2D>());
        Destroy(GetComponent<Rigidbody2D>());

        // Sonido de muerte
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
        
        if (!string.IsNullOrEmpty(nextLevelName))
        {
            SceneManager.LoadScene(nextLevelName);
        }
        else
        {
            Debug.LogError("¡ERROR! No has escrito el nombre del siguiente nivel en el Inspector del Boss.");
        }
    }

    // Opcionales
    public void Jump() { if (!isDead) anim.SetTrigger("JumpTrigger"); }
    public void Climb() { if (!isDead) anim.SetTrigger("ClimbTrigger"); }
    public void Talk() { anim.SetTrigger("TalkTrigger"); }
}