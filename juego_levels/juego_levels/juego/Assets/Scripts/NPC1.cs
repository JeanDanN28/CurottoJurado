using UnityEngine;

public class NPC1 : MonoBehaviour
{
    [Header("Configuración de Interacción")]
    [SerializeField] GameObject dialoguePanel; 
    [SerializeField] DialogueDisplay displayScript; 
    
    [Header("Indicador UI")]
    [SerializeField] GameObject interactIndicator; 

    [Header("Textos Secuenciales")]
    [TextArea(3, 5)]
    [SerializeField] string[] dialoguePages = {
        "Portador, tu viaje será largo. Aquí está tu guía de supervivencia.",
        "🔸 DASH (Esquiva): Presiona 'Left Shift'. Te da un impulso rápido para esquivar ataques o cruzar huecos cortos.",
        "🔸 DOBLE SALTO: Presiona 'Espacio' una segunda vez en el aire. Útil para alcanzar plataformas altas.",
        "🔸 ATAQUE: Presiona 'E' para tu espadazo cuerpo a cuerpo. Sincroniza bien el golpe para hacer daño."
    };
    
    private int currentPageIndex = 0; 
    private bool playerInRange = false;

    void Start()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        if (interactIndicator != null)
        {
            interactIndicator.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactIndicator != null)
            {
                interactIndicator.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            HideDialogue();
            if (interactIndicator != null)
            {
                interactIndicator.SetActive(false);
            }
        }
    }
    
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F)) 
        {
            AdvanceDialogue(); 
        }
    }

    void AdvanceDialogue()
    {
        if (dialoguePages.Length == 0) 
        {
            Debug.LogError("Error: El array Dialogue Pages está vacío en el Inspector. ¡Llénalo!");
            return;
        }

        if (dialoguePanel.activeSelf)
        {
            if (currentPageIndex < dialoguePages.Length - 1)
            {
                currentPageIndex++;
                DisplayCurrentPage();
            }
            else
            {
                HideDialogue();
            }
        }
        else
        {
            dialoguePanel.SetActive(true);
            currentPageIndex = 0;
            DisplayCurrentPage();
        }
    }

    void DisplayCurrentPage()
    {
        if (displayScript != null && currentPageIndex < dialoguePages.Length)
        {
            displayScript.SetText(dialoguePages[currentPageIndex]);
        }
        else if (displayScript == null)
        {
            Debug.LogError("Error: DialogueDisplay script no está asignado en el Inspector del NPC.");
        }
    }
    
    void HideDialogue()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        currentPageIndex = 0; 
    }
}