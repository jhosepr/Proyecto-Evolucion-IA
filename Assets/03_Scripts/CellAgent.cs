using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CellAgent : Agent
{
    private SpriteRenderer sr;
    public Color backgroundColor;

    [Header("Límites de Adaptación (Requerimiento Docente)")]
    public float minScale = 0.4f;
    public float maxScale = 1.2f;

    public override void Initialize()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //  Color (R, G, B)
        sensor.AddObservation(sr.color.r);
        sensor.AddObservation(sr.color.g);
        sensor.AddObservation(sr.color.b);

        //  Tamaño
        sensor.AddObservation(transform.localScale.x);

        //  Entorno (Fondo)
        sensor.AddObservation(backgroundColor.r);
        sensor.AddObservation(backgroundColor.g);
        sensor.AddObservation(backgroundColor.b);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // El algoritmo intenta diferentes valores
        float changeR = actions.ContinuousActions[0];
        float changeG = actions.ContinuousActions[1];
        float changeB = actions.ContinuousActions[2];
        float changeSize = actions.ContinuousActions[3];

        //  cambios de Color con límites (Clamp01 limita entre 0 y 1)
        Color newColor = sr.color;
        newColor.r = Mathf.Clamp01(newColor.r + changeR * 0.004f);
        newColor.g = Mathf.Clamp01(newColor.g + changeG * 0.004f);
        newColor.b = Mathf.Clamp01(newColor.b + changeB * 0.004f);
        sr.color = newColor;

        //  cambios de Tamaño con límites Min/Max (Requerimiento Docente)
        float currentScale = transform.localScale.x;
        float nextScale = Mathf.Clamp(currentScale + changeSize * 0.004f, minScale, maxScale);
        transform.localScale = new Vector3(nextScale, nextScale, 1f);

        // --- Lógica Premiar supervivencia y adaptación) ---

        // Evaluación de Color
        float colorDiff = (Mathf.Abs(sr.color.r - backgroundColor.r) +
                          Mathf.Abs(sr.color.g - backgroundColor.g) +
                          Mathf.Abs(sr.color.b - backgroundColor.b)) / 3.0f;
        float colorScore = 1.0f - colorDiff;

        // Evaluación de Tamaño: 1 es tamaño mínimo (mejor), 0 es tamaño máximo
        float sizeScore = 1.0f - ((nextScale - minScale) / (maxScale - minScale));

        // Recompensa  Aprende de intentos exitosos 
        float totalReward = (colorScore + sizeScore) / 2.0f;
        AddReward(totalReward * 0.01f * Time.fixedDeltaTime);
    }

    public void OnDeath()
    {
        // Lógica para aprender de intentos fallidos
        SetReward(-1.0f);
        EndEpisode();
    }
}