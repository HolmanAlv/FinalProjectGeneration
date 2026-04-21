using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image icon;
    private TextMeshProUGUI label;
    private Outline outline;
    private Vector3 originalScale;

    void Awake()
    {
        // Busca componentes automáticamente
        icon = GetComponent<Image>();
        label = GetComponentInChildren<TextMeshProUGUI>();
        originalScale = transform.localScale;

        // Crea el outline del sprite automáticamente
        outline = GetComponent<Outline>();
        if (outline == null)
            outline = gameObject.AddComponent<Outline>();

        outline.effectColor = Color.white;
        outline.effectDistance = new Vector2(3, -3);
        outline.enabled = false;

        // Outline del texto (empieza apagado)
        if (label != null)
        {
            label.outlineWidth = 0f;
            label.outlineColor = Color.white;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Activa outline del sprite
        outline.enabled = true;

        // Texto negro con outline blanco
        if (label != null)
        {
            label.color = Color.black;
            label.outlineWidth = 0.2f;
        }

        // Pequeño pop
        transform.localScale = originalScale * 1.05f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Desactiva outline del sprite
        outline.enabled = false;

        // Texto vuelve a blanco sin outline
        if (label != null)
        {
            label.color = Color.white;
            label.outlineWidth = 0f;
        }

        // Vuelve a escala original
        transform.localScale = originalScale;
    }
}