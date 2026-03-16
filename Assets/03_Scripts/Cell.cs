using UnityEngine;
using UnityEngine.EventSystems; // OBLIGATORIO para detectar clics modernos

public class Cell : MonoBehaviour, IPointerDownHandler
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // El manager llamará a esta función para pasarle los "genes" (color y escala)
    public void Init(Color dnaColor, float dnaScale)
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.color = dnaColor;
        transform.localScale = new Vector3(dnaScale, dnaScale, 1f);
    }

    // Este método reemplaza a OnMouseDown y es detectado por el EventSystem
    // Modifica tu método OnPointerDown en Cell.cs
    public void OnPointerDown(PointerEventData eventData)
    {
        // Buscamos el componente de IA
        CellAgent agent = GetComponent<CellAgent>();
        if (agent != null)
        {
            agent.OnDeath(); // <--- Aquí le avisamos a la IA que "murió"
        }

        if (Evolution.Instance != null)
        {
            Evolution.Instance.RecordDeath();
        }

        Destroy(gameObject);
    }
}