using UnityEngine;
using TMPro;

public class Evolution : MonoBehaviour
{
    // Singleton para que las células puedan avisar cuando mueren
    public static Evolution Instance;

    [Header("UI")]
    public TextMeshProUGUI timeUI;
    public TextMeshProUGUI scoreUI;

    [Header("Configuración")]
    public GameObject cellPrefab;
    public float roundDuration = 30f;
    public int countPerRound = 10;

    private int score = 0;
    private float timer;
    [Header("Estado de Evolución")]
    public Color bestColor = Color.gray; 
    public float bestScale = 1.0f;

    private void Awake()
    {
        // Configuración del Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Inicializar textos de la interfaz
        ActualizarInterfaz();
        StartRound();
    }

    private void Update()
    {
        timer -= Time.unscaledDeltaTime;

        if (timeUI != null && timer >= 0)
        {
            timeUI.text = "Tiempo: " + Mathf.Ceil(timer).ToString() + "s";
        }

        if (timer <= 0)
        {
            EndRound();
        }
    }

    void StartRound()
    {
        timer = roundDuration;
        for (int i = 0; i < countPerRound; i++)
        {
            Spawn();
        }
    }

    void Spawn()
    {
        // Posición aleatoria
        Vector3 pos = new Vector3(Random.Range(-7f, 7f), Random.Range(-3.5f, 3.5f), 0);
        GameObject go = Instantiate(cellPrefab, pos, Quaternion.identity);

        // Lógica de herencia y mutación
        float mutation = 0.15f;
        float newScale = Mathf.Clamp(bestScale + Random.Range(-mutation, mutation), 0.2f, 1.2f);
        Color newColor = new Color(
            Mathf.Clamp01(bestColor.r + Random.Range(-mutation, mutation)),
            Mathf.Clamp01(bestColor.g + Random.Range(-mutation, mutation)),
            Mathf.Clamp01(bestColor.b + Random.Range(-mutation, mutation))
        );

        // Inicializar la célula
        Cell cellScript = go.GetComponent<Cell>();
        if (cellScript != null)
        {
            cellScript.Init(newColor, newScale);
        }
    }

    void EndRound()
    {
        // Encontrar sobrevivientes (los que NO clickeaste)
        Cell[] survivors = Object.FindObjectsByType<Cell>(FindObjectsSortMode.None);

        if (survivors.Length > 0)
        {
            // La siguiente generación evoluciona basada en un sobreviviente aleatorio
            int randomIndex = Random.Range(0, survivors.Length);
            SpriteRenderer sr = survivors[randomIndex].GetComponent<SpriteRenderer>();

            if (sr != null)
            {
                bestColor = sr.color;
                bestScale = survivors[randomIndex].transform.localScale.x;
            }
        }

        // Limpiar mesa para la siguiente ronda
        foreach (Cell c in survivors)
        {
            Destroy(c.gameObject);
        }

        StartRound();
    }

    public void RecordDeath()
    {
        score++;
        if (scoreUI != null)
        {
            scoreUI.text = "Puntos: " + score;
        }
    }

    private void ActualizarInterfaz()
    {
        if (timeUI != null) timeUI.text = "Tiempo: " + roundDuration.ToString() + "s";
        if (scoreUI != null) scoreUI.text = "Puntos: " + score.ToString();
    }
}