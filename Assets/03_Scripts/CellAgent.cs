using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CellAgent : Agent
{
    private SpriteRenderer sr;

    [Header("Límites de Adaptación")]
    public float minScale = 0.2f;
    public float maxScale = 2.0f;

    public override void Initialize()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // La IA solo se observa a sí misma (Ciega al fondo)
        sensor.AddObservation(sr.color.r);
        sensor.AddObservation(sr.color.g);
        sensor.AddObservation(sr.color.b);
        sensor.AddObservation(transform.localScale.x);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // 1. Aplicar cambios de Color
        Color newColor = sr.color;
        newColor.r = Mathf.Clamp01(newColor.r + actions.ContinuousActions[0] * 0.01f);
        newColor.g = Mathf.Clamp01(newColor.g + actions.ContinuousActions[1] * 0.01f);
        newColor.b = Mathf.Clamp01(newColor.b + actions.ContinuousActions[2] * 0.01f);
        sr.color = newColor;

        // 2. Aplicar cambios de Tamaño
        float currentScale = transform.localScale.x;
        float nextScale = Mathf.Clamp(currentScale + (actions.ContinuousActions[3] * 0.1f * Time.fixedDeltaTime), minScale, maxScale);
        transform.localScale = new Vector3(nextScale, nextScale, 1f);

        // 3. RECOMPENSA PASIVA (Supervivencia)
        // Le damos un premio pequeño por existir. Si la matas, deja de recibir esto.
        AddReward(0.001f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
        continuousActionsOut[3] = Input.GetKey(KeyCode.Space) ? -1.0f : 1.0f;
    }

    // Este método lo llamarás desde tu script de Clic/Muerte
    public void OnDeath()
    {
        SetReward(-1.0f); // Castigo por ser atrapada
        EndEpisode();
    }
}