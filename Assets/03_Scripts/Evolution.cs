using UnityEngine;
using TMPro;

public class Evolution : MonoBehaviour
{
    // Singleton para que las células puedan avisar cuando mueren
    public static Evolution Instance;

    [Header("UI")]
    public TextMeshProUGUI timeUI;
    public TextMeshProUGUI scoreUI;
    public TextMeshProUGUI generationUI;

    [Header("Configuración")]
    public GameObject cellPrefab;
    public float roundDuration = 30f;
    public int countPerRound = 10;

    private int score = 0;
    private int generation = 1;
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
        // Lógica de "Spawn en Esquinas" (Requerimiento del Usuario)
        // 30% de probabilidad de spawnear específicamente cerca de una esquina
        Vector3 pos;
        if (Random.value < 0.3f)
        {
            float x = Random.value < 0.5f ? -6.5f : 6.5f;
            float y = Random.value < 0.5f ? -3f : 3f;
            pos = new Vector3(x + Random.Range(-0.5f, 0.5f), y + Random.Range(-0.5f, 0.5f), 0);
        }
        else
        {
            pos = new Vector3(Random.Range(-7f, 7f), Random.Range(-3.5f, 3.5f), 0);
        }

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
        // Encontrar sobrevivientes (los más aptos de la generación)
        Cell[] survivors = Object.FindObjectsByType<Cell>(FindObjectsSortMode.None);

        if (survivors.Length > 0)
        {
            // MEJOR SELECCIÓN: Entre los que sobrevivieron toda la ronda,
            // vamos a buscar al que mejor se camufló (mejoró su ADN en tiempo real)
            Cell bestSurvivor = survivors[0];
            float bestFitness = -1f;

            foreach (Cell c in survivors)
            {
                SpriteRenderer sr = c.GetComponent<SpriteRenderer>();
                float colorDiff = (Mathf.Abs(sr.color.r - backgroundColor.r) +
                                  Mathf.Abs(sr.color.g - backgroundColor.g) +
                                  Mathf.Abs(sr.color.b - backgroundColor.b)) / 3.0f;
                
                // Fitness: Menor diferencia de color + menor tamaño = mejor sobreviviente
                float currentFitness = (1.0f - colorDiff) + (1.0f - c.transform.localScale.x);

                if (currentFitness > bestFitness)
                {
                    bestFitness = currentFitness;
                    bestSurvivor = c;
                }
            }

            // Guardamos el "ADN" del mejor para la siguiente generación
            bestColor = bestSurvivor.GetComponent<SpriteRenderer>().color;
            bestScale = bestSurvivor.transform.localScale.x;

            Debug.Log($"Generación terminada. Mejor fitness: {bestFitness}. Escalando evolución...");
        }

        // Limpiar para la siguiente ronda
        foreach (Cell c in survivors)
        {
            Destroy(c.gameObject);
        }

        generation++;
        ActualizarInterfaz();
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
        if (timeUI != null) timeUI.text = "Tiempo: " + Mathf.Ceil(timer).ToString() + "s";
        if (scoreUI != null) scoreUI.text = "Puntos: " + score.ToString();
        if (generationUI != null) generationUI.text = "Gen: " + generation.ToString();
    }
}