using UnityEngine;
using TMPro;
using Unity.MLAgents; // Necesario para detener la IA

public class Evolution : MonoBehaviour
{
    public static Evolution Instance;

    [Header("UI")]
    public TextMeshProUGUI timeUI;
    public TextMeshProUGUI scoreUI;

    [Header("Configuración")]
    public GameObject cellPrefab;
    public float roundDuration = 30f;
    public int countPerRound = 10;

    [Header("Meta de la Defensa")]
    public int maxScore = 200; 

    private int score = 0;
    private float timer;

    [Header("Estado de Evolución")]
    public Color bestColor = Color.gray;
    public float bestScale = 1.0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
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
        Vector3 pos = new Vector3(Random.Range(-7f, 7f), Random.Range(-3.5f, 3.5f), 0);
        GameObject go = Instantiate(cellPrefab, pos, Quaternion.identity);

        float mutation = 0.15f;
        float newScale = Mathf.Clamp(bestScale + Random.Range(-mutation, mutation), 0.2f, 2.0f);
        Color newColor = new Color(
            Mathf.Clamp01(bestColor.r + Random.Range(-mutation, mutation)),
            Mathf.Clamp01(bestColor.g + Random.Range(-mutation, mutation)),
            Mathf.Clamp01(bestColor.b + Random.Range(-mutation, mutation))
        );

        Cell cellScript = go.GetComponent<Cell>();
        if (cellScript != null)
        {
            cellScript.Init(newColor, newScale);
        }
    }

    void EndRound()
    {
        Cell[] survivors = Object.FindObjectsByType<Cell>(FindObjectsSortMode.None);

        if (survivors.Length > 0)
        {
            int randomIndex = Random.Range(0, survivors.Length);
            SpriteRenderer sr = survivors[randomIndex].GetComponent<SpriteRenderer>();

            if (sr != null)
            {
                bestColor = sr.color;
                bestScale = survivors[randomIndex].transform.localScale.x;
            }
        }

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

        // --- LÓGICA DE PARADA PARA EL EXAMEN ---
        if (score >= maxScore)
        {
            Debug.Log("Meta de puntos alcanzada.");

            // Detiene la comunicación con la terminal de Git Bash
            Academy.Instance.Dispose();

            // Detiene el modo Play en Unity
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }

    private void ActualizarInterfaz()
    {
        if (timeUI != null) timeUI.text = "Tiempo: " + roundDuration.ToString() + "s";
        if (scoreUI != null) scoreUI.text = "Puntos: " + score.ToString();
    }
}