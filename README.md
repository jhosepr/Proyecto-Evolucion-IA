# Proyecto Evolución IA - Guía y Solución de ML-Agents

## Solución al problema de exportación ONNX
El problema inicial era la incapacidad de generar archivos `.onnx` a partir de los entrenamientos (`.pt`) de ML-Agents en Unity. Esto se debía a la falta de un entorno de Python configurado correctamente.

### Pasos realizados para solucionarlo:
1. **Instalación de Python:** Se instaló Python 3.8.10.
2. **Instalación de dependencias:** 
   - Se instaló la biblioteca `mlagents` (versión 0.29.0).
   - Se instaló `torch` (PyTorch) que es el motor de machine learning utilizado por detrás.
   - Se instaló `onnx` (versión compatible 1.12.0) y `protobuf` (3.20.0).
3. **Corrección de la configuración:**
   - En el archivo `configuration.yaml` correspondiente al entrenamiento, se eliminaron los parámetros que ya no son compatibles con la versión actual de ML-Agents:
     - `shared_critic`
     - `even_checkpoints`
     - `timeout_wait`
     - `no_graphics_monitor`
   - Se ajustó momentáneamente el parámetro `max_steps` para forzar la finalización y exportación del modelo al resumir el entrenamiento.

Una vez reanudado el entrenamiento por consola (con `--resume`), solo bastaba dar *Play* en Unity para que, al detectar que las condiciones de término se cumplían, se generara instantáneamente el archivo `.onnx`.

## Guía paso a paso: Implementando Evolución y Camuflaje (Neuroevolución + RL)

Para lograr que las células no solo aprendan a moverse inteligentemente (huyendo o escondiéndose), sino que también cambien físicamente (color, tamaño) a lo largo de las generaciones para sobrevivir, se debe combinar el **Aprendizaje por Refuerzo (Reinforcement Learning)** con **Algoritmos Genéticos**.

### Fase 1: Genética de la Célula
Debes crear un script separado, por ejemplo `CellGenetics.cs`, que contenga las variables físicas (fenotipo). Estas serán modificadas solo entre "rondas" o generaciones.

```csharp
public class CellGenetics : MonoBehaviour
{
    public Color genomeColor;
    public float genomeSize;
    
    // Función que se llamará al instanciar la siguiente generación
    public void Mutate(float mutationRate)
    {
        // Mutar el tamaño (puede crecer o encogerse ligeramente)
        genomeSize += Random.Range(-mutationRate, mutationRate);
        genomeSize = Mathf.Clamp(genomeSize, 0.5f, 2.0f); // Limitar tamaño min/max
        
        // Mutar el color (sumar o restar un valor pequeño al R, G, B)
        genomeColor = new Color(
            Mathf.Clamp01(genomeColor.r + Random.Range(-mutationRate, mutationRate)),
            Mathf.Clamp01(genomeColor.g + Random.Range(-mutationRate, mutationRate)),
            Mathf.Clamp01(genomeColor.b + Random.Range(-mutationRate, mutationRate)),
            1.0f
        );

        ApplyGenetics();
    }

    // Aplica los genes a la célula física
    public void ApplyGenetics()
    {
        transform.localScale = new Vector3(genomeSize, genomeSize, genomeSize);
        GetComponent<Renderer>().material.color = genomeColor;
    }
}
```

### Fase 2: Recolección de Observaciones en el Agente
En tu clase que hereda de `Agent` (ML-Agents), el método `CollectObservations` debe recopilar información sobre el entorno para la red neuronal.

```csharp
public override void CollectObservations(VectorSensor sensor)
{
    // 1. Distancia hacia el jugador (depredador)
    sensor.AddObservation(Vector3.Distance(transform.position, player.position));
    // 2. Dirección hacia el jugador
    sensor.AddObservation((player.position - transform.position).normalized);
    // 3. Su tamaño actual (mientras más pequeño, menos rango de visión tiene el jugador)
    sensor.AddObservation(GetComponent<CellGenetics>().genomeSize);
    
    // 4. (Opcional avanzado) Diferencia de color entre la célula y el suelo = nivel de camuflaje
    // Color floorColor = GetFloorColor();
    // Color myColor = GetComponent<CellGenetics>().genomeColor;
    // float camouflageFactor = Diff(floorColor, myColor);
    // sensor.AddObservation(camouflageFactor);
}
```

### Fase 3: Acciones y Recompensas (Reinforcement Learning)
El agente toma sus acciones basadas en sus observaciones, pero **sus metas se definen aquí con recompensas y castigos (Rewards)**.

```csharp
public override void OnActionReceived(ActionBuffers actions)
{
    // Acciones: moverse en X y Z
    float moveX = actions.ContinuousActions[0];
    float moveZ = actions.ContinuousActions[1];
    
    // Lógica para moverse aquí...
    transform.position += new Vector3(moveX, 0, moveZ) * speed * Time.deltaTime;

    // --- RECOMPENSAS DIARIAS (O por Frame) ---
    // Dar una micro-recompensa constantemente recompensa "seguir vivo"
    AddReward(0.001f);
}

// Evento: Al colisionar (o ser atrapado) por el jugador
private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        // CASTIGO MAYÚSCULO por morir.
        SetReward(-1.0f);
        
        // Guardar su tiempo de supervivencia en el gestor de evolución antes de morir
        EvolutionManager.RecordDeath(this);
        
        EndEpisode(); // Finaliza el intento y reinicia
    }
}
```

### Fase 4: El Sistema Evolutivo General (Generaciones)
El último eslabón. Mientras ML-Agents entrena cómo debe *moverse* la IA para sobrevivir en el motor basado en los premios del paso 3, tú requieres un `EvolutionManager.cs` en tu escena que mande el progreso de la forma física.

1. **Población Inicial:** Al inicio del juego instanciar 50 células con colores/tamaños aleatorios.
2. **Cosecha de los Mejores (Fitness):** Cada que una célula la atrapan, guardas su tiempo total viva. Si usas tiempo límite (ej. sobrevivir 60s), las que sobreviven los 60s son tus "campeones".
3. **Reproducción (Selección Natural):** Termina la "Generación 1". Tomas los genes (el Componente `CellGenetics`) de las 5 células que sobrevivieron más tiempo. Instancias a las 50 células de la "Generación 2", **copiándoles a todas los genes de los campeones, pero llamando a `Mutate()` en cada una de ellas**.

**Resultado esperado con el paso de múltiples generaciones:**
Por simple mecánica de selección, las células que hayan empezado al azar *más pequeñas* o con *un color más parecido al suelo* habrán sobrevivido más al jugador, dándoles así más "recompensas" de ML-agents. Este diseño, vuelta tras vuelta, provoca que la IA descubra por ella sola que la mejor "estrategia" no sólo es moverse hacia rincones asilados o esquinas de la pista (aprendido vía ONNX/PyTorch), sino propagar el camuflaje extremo a su especie.
