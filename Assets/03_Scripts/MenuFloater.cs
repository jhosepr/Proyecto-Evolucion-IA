using UnityEngine;

public class MenuFloater : MonoBehaviour
{
    public GameObject cellPrefab; // Arrastra tu MenuCell aquí
    public float spawnTime = 1.0f; // Cada segundo aparece una

    void Start()
    {
        // Ejecuta la función Spawn repetidamente
        InvokeRepeating("Spawn", 0, spawnTime);
    }

    void Spawn()
    {
        // 1. Posición aleatoria abajo (eje Y = -6) para que floten hacia arriba
        Vector3 spawnPos = new Vector3(Random.Range(-10f, 10f), -6f, 0);
        GameObject cell = Instantiate(cellPrefab, spawnPos, Quaternion.identity);

        // 2. Tamaño aleatorio entre 0.2 y 2.0 (tus límites de clase)
        float s = Random.Range(0.2f, 2.0f);
        cell.transform.localScale = new Vector3(s, s, 1);

        // 3. Toque extra: ¡Hagámoslas de colores! 🎨
        // Así el menú se ve increíble con la música
        cell.GetComponent<SpriteRenderer>().color = new Color(Random.value, Random.value, Random.value);

        // 4. Les damos movimiento físico tipo "expulsado de Among Us"
        Rigidbody2D rb = cell.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0; // Sin gravedad para que floten
        rb.linearVelocity = new Vector2(Random.Range(-1.5f, 1.5f), Random.Range(2f, 5f)); // Hacia arriba y a los lados
        rb.angularVelocity = Random.Range(-90f, 90f); // Giro constante

        // 5. Autodestrucción después de 12 segundos para que no llenen la memoria de tu Asus TUF
        Destroy(cell, 12f);
    }
}