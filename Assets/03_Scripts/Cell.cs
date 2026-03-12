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
    public void OnPointerDown(PointerEventData eventData)
    {
        // 1. Mensaje de confirmación en consola
        Debug.Log("¡Clic detectado en la célula!");

        // 2. Notificar al manager que esta célula fue eliminada (puntos)
        if (Evolution.Instance != null)
        {
            Evolution.Instance.RecordDeath();
        }

        // 3. Destruir el objeto
        Destroy(gameObject);
    }
}