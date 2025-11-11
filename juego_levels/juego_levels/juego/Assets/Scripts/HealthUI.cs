using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HealthUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject heartPrefab;   // Prefab que debe contener un componente Image (UI)
    [SerializeField] private Transform heartContainer; // Contenedor (ej: objeto con Horizontal Layout Group)
    [SerializeField] private Sprite fullHeart;         // Sprite corazón lleno
    [SerializeField] private Sprite emptyHeart;        // Sprite corazón vacío
    [SerializeField] private int maxHearts = 8;        // Cuántos corazones queremos mostrar (independiente del jugador)

    private List<Image> hearts = new List<Image>();

    void Start()
    {
        // Validaciones básicas:
        if (heartPrefab == null) { Debug.LogError("[HealthUI] Falta Heart Prefab"); return; }
        if (heartContainer == null) { Debug.LogError("[HealthUI] Falta Heart Container"); return; }
        if (fullHeart == null || emptyHeart == null) { Debug.LogWarning("[HealthUI] Falta full/empty sprite (se usarán los que tenga el prefab)"); }

        // Forzar creación según maxHearts (más fiable que usar la vida del jugador)
        CreateHearts(maxHearts);

        // Log para debug
        Debug.Log($"[HealthUI] Corazones creados: {hearts.Count}  (maxHearts = {maxHearts})");
    }

    // Crea exactamente 'count' corazones
    public void CreateHearts(int count)
    {
        // Limpia cualquier hijo anterior
        for (int i = heartContainer.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(heartContainer.GetChild(i).gameObject);
        }
        hearts.Clear();

        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(heartPrefab, heartContainer);
            go.name = "Heart_" + i;

            // Asegurarnos que el prefab tenga Image
            Image img = go.GetComponent<Image>();
            if (img == null)
            {
                Debug.LogError($"[HealthUI] El prefab '{heartPrefab.name}' no contiene componente Image. Añade un Image al prefab.");
                continue;
            }

            // Si hay sprites asignados en el inspector, úsalos
            if (fullHeart != null) img.sprite = fullHeart;

            // Asegurar escala y recttransform correcta
            RectTransform rt = go.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.localScale = Vector3.one;
                rt.anchoredPosition = Vector2.zero;
            }

            hearts.Add(img);
        }
    }

    // Actualiza visualmente los corazones (no cambia la cantidad)
    public void UpdateHearts(int currentHealth)
    {
        if (hearts == null || hearts.Count == 0)
        {
            Debug.LogWarning("[HealthUI] UpdateHearts llamado pero no hay corazones creados.");
            return;
        }

        // Safety clamp
        currentHealth = Mathf.Clamp(currentHealth, 0, hearts.Count);

        for (int i = 0; i < hearts.Count; i++)
        {
            if (hearts[i] == null) continue;
            hearts[i].sprite = (i < currentHealth && fullHeart != null) ? fullHeart : (emptyHeart != null ? emptyHeart : hearts[i].sprite);
            hearts[i].enabled = true;
        }

        Debug.Log($"[HealthUI] UpdateHearts -> currentHealth = {currentHealth}");
    }
}
