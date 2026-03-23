using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CellAgent : Agent
{
    private SpriteRenderer sr;

    [Header("Configuración de Entorno")]
    public Color backgroundColor;

    [Header("Límites de Adaptación (Requerimiento Docente)")]
    public float minScale = 0.2f; // Ajustado a tus nuevos límites
    public float maxScale = 2.0f; // Ajustado a tus nuevos límites

    public override void Initialize()
    {
        sr = GetComponent<SpriteRenderer>();

        if (Camera.main != null)
        {
            backgroundColor = Camera.main.backgroundColor;
            Debug.Log("Célula vinculada al fondo de la cámara: " + backgroundColor);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(sr.color.r);
        sensor.AddObservation(sr.color.g);
        sensor.AddObservation(sr.color.b);
        sensor.AddObservation(transform.localScale.x);

        sensor.AddObservation(backgroundColor.r);
        sensor.AddObservation(backgroundColor.g);
        sensor.AddObservation(backgroundColor.b);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float changeR = actions.ContinuousActions[0];
        float changeG = actions.ContinuousActions[1];
        float changeB = actions.ContinuousActions[2];
        float changeSize = actions.ContinuousActions[3];

        // Aplicar Color
        Color newColor = sr.color;
        newColor.r = Mathf.Clamp01(newColor.r + changeR * 0.005f);
        newColor.g = Mathf.Clamp01(newColor.g + changeG * 0.005f);
        newColor.b = Mathf.Clamp01(newColor.b + changeB * 0.005f);
        sr.color = newColor;

        // Aplicar Tamaño con suavizado
        float currentScale = transform.localScale.x;
        float nextScale = Mathf.Clamp(currentScale + (changeSize * 0.05f * Time.fixedDeltaTime), minScale, maxScale);
        transform.localScale = new Vector3(nextScale, nextScale, 1f);

        // --- Lógica de Recompensas (BALANCEADA 60/40) ---

        // 1. Puntaje de Color (Camuflaje)
        float colorDiff = (Mathf.Abs(sr.color.r - backgroundColor.r) +
                          Mathf.Abs(sr.color.g - backgroundColor.g) +
                          Mathf.Abs(sr.color.b - backgroundColor.b)) / 3.0f;
        float colorScore = 1.0f - colorDiff;

        // 2. Puntaje de Tamaño (Supervivencia)
        // 1.0 = Tamaño mínimo (bueno), 0.0 = Tamaño máximo (malo)
        float sizeScore = 1.0f - ((nextScale - minScale) / (maxScale - minScale));

        // 3. Recompensa Total
        // Le damos un 60% de importancia al tamaño y un 40% al color
        float totalReward = (colorScore * 0.4f) + (sizeScore * 0.6f);

        AddReward(totalReward * 0.01f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
        // Barra espaciadora para achicar manualmente en pruebas
        continuousActionsOut[3] = Input.GetKey(KeyCode.Space) ? -1.0f : 1.0f;
    }

    public void OnDeath()
    {
        SetReward(-1.0f);
        EndEpisode();
    }
}