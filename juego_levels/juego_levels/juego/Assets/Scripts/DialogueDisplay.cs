using UnityEngine;
using TMPro; 

public class DialogueDisplay : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI dialogueTextComponent; 
    
    
    public void SetText(string newText)
    {
        if (dialogueTextComponent != null)
        {
            dialogueTextComponent.text = newText;
        }
    }
}