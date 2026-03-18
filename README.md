# 🧬 Proyecto Evolución IA: Guía Maestra de ML-Agents

Bienvenido al núcleo de control de la simulación. Este documento detalla cómo hemos transformado una IA estática en un ecosistema dinámico de **Neuroevolución** y **Aprendizaje por Refuerzo**. Aquí encontrarás todo lo necesario para entender, operar y mejorar este proyecto, explicado de forma que cualquiera pueda dominarlo.

---

## 🛠️ 1. Solución de Problemas Técnicos (Post-Mortem)

Si llegaste aquí porque los archivos `.onnx` no aparecían, esto es lo que arreglamos para que el sistema sea estable:

1.  **Entorno Python:** Se configuró un entorno con Python 3.8.10 para garantizar compatibilidad total con Unity ML-Agents 0.29.0.
2.  **Librerías Críticas:** Instalamos `mlagents`, `torch` (motor de cálculo) y `onnx` (formato de intercambio de modelos).
3.  **Limpieza de Configuración:** El archivo `configuration.yaml` tenía parámetros obsoletos (`shared_critic`, `even_checkpoints`, etc.). Los eliminamos para que el entrenamiento no se detenga por errores de sintaxis.
4.  **Exportación Forzada:** Ajustamos `max_steps` para que, al darle "Play" en Unity después de un entrenamiento, el sistema genere el archivo `.onnx` de inmediato.

---

## 🔬 2. El Concepto: Neuroevolución + RL

Este proyecto no usa solo "IA normal". Usa una combinación de dos mundos:
*   **Aprendizaje por Refuerzo (RL):** La IA aprende a **moverse** en tiempo real para esquivar tu ratón (el "cerebro").
*   **Algoritmos Genéticos:** Las células heredan **rasgos físicos** (color y tamaño) de sus ancestros más exitosos (el "ADN").

---

## 🚀 3. Guía Paso a Paso para Operar el Sistema

Sigue estos pasos para ver la evolución en acción:

### Paso A: Configuración en Unity
1.  Busca el objeto **Evolution** en tu jerarquía de escena.
2.  En el Inspector, verás el script `Evolution.cs`. Verifica que tenga asignado:
    *   **Cell Prefab:** El prefab de tu célula.
    *   **Time/Score/Generation UI:** Los objetos de texto (TextMeshPro) donde verás las estadísticas.
3.  **¡IMPORTANTE!** Asegúrate de que tu `Cell Prefab` tenga el script `CellAgent` y que en su `Behavior Parameters`, el campo **Model** tenga el archivo `.onnx` que generamos.

### Paso B: La Lógica de Supervivencia
Mientras juegas, notarás lo siguiente:
*   **Detección de Peligro:** Las células "sienten" tu ratón. Si te acercas, su sistema de recompensa las castiga, obligándolas a aprender rutas de escape.
*   **Camuflaje Activo:** Si el fondo es verde y la célula es roja, la IA recibirá una penalización. Su "instinto" la llevará a cambiar su color hacia el verde para ser "invisible".
*   **Spawn en Esquinas:** Hemos programado una probabilidad del 30% de que las células aparezcan en los bordes. Esto les enseña que los rincones son zonas seguras.

### Paso C: El Salto Generacional
Cada 30 segundos (configurable), la ronda termina:
1.  El sistema analiza quiénes sobrevivieron.
2.  Busca a la **Célula Campeona**: la que mejor se camufló y mejor tamaño alcanzó.
3.  **Mutación:** Las nuevas células de la siguiente generación nacen con el ADN de la campeona, pero con un ligero cambio aleatorio (mutación) para intentar ser aún mejores.

---

## 📈 4. Cómo Mejorar la IA (Para Usuarios Avanzados)

Si quieres llevar esto al siguiente nivel, ajusta estos valores en los scripts:

*   **Dificultad de Evolución:** En `Evolution.cs`, cambia `mutation = 0.15f`. Un valor más alto significa cambios físicos más drásticos por generación; un valor más bajo es una evolución más lenta y sutil.
*   **Inteligencia de Movimiento:** Si quieres que aprendan a huir más rápido, aumenta el `AddReward` en `CellAgent.cs` para la distancia al ratón.
*   **Duración de Ronda:** Si quieres ver 100 generaciones en poco tiempo, baja la `roundDuration` a 10 o 15 segundos.

---

## 📁 5. Estructura de Archivos Clave
*   `Assets/03_Scripts/CellAgent.cs`: El cerebro (ML-Agents). Maneja qué ve la célula y cómo reacciona.
*   `Assets/03_Scripts/Evolution.cs`: El motor de la vida. Maneja las generaciones, el spawn y la herencia genética.
*   `Assets/03_Scripts/Cell.cs`: El cuerpo físico. Maneja las interacciones y el ADN inicial.
*   `results/Entrenamiento_Final/configuration.yaml`: El mapa de entrenamiento para Python.

---
*Documentación generada por Antigravity AI para el Proyecto Evolución IA. "La supervivencia no es del más fuerte, sino del que mejor se adapta".*

