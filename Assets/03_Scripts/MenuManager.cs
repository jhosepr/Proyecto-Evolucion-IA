using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Necesario para que funcione la espera

public class MenuManager : MonoBehaviour
{
    [Header("Configuración de Animación")]
    public Animator fadeAnimator; // Arrastraremos el objeto FadeImage aquí
    public float tiempoEspera = 1.5f; // Cuánto tarda en ponerse negro (ajústalo a tu gusto)

    public void IniciarJuego()
    {
        // En lugar de cargar directo, llamamos a la función de espera
        StartCoroutine(TransicionAlJuego());
    }

    IEnumerator TransicionAlJuego()
    {
        // 1. Activa la animación del Fade
        if (fadeAnimator != null)
        {
            fadeAnimator.SetTrigger("EmpezarFade");
        }

        // 2. Espera el tiempo que definiste
        yield return new WaitForSeconds(tiempoEspera);

        // 3. Recién ahora cambia de escena
        SceneManager.LoadScene("SampleScene");
    }

    public void Salir()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }
}