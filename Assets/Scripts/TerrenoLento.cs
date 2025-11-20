using UnityEngine;

public class TerrenoLento : MonoBehaviour
{
    public float factorReduccion = 0.5f;   // Qué tan rápido baja la velocidad
    private ControladorCoche controlador; 
    private bool cocheDentro = false;

    void Start()
    {
        controlador = FindAnyObjectByType<ControladorCoche>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cocheDentro = true;
            controlador.estaEnTerrenoLento = true;
            StopAllCoroutines();
            StartCoroutine(ReducirVelocidad());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cocheDentro = false;
            controlador.estaEnTerrenoLento = false;
            StopAllCoroutines();
        }
    }

    System.Collections.IEnumerator ReducirVelocidad()
    {
        while (controlador.velocidad > controlador.velocidadMinima && cocheDentro)
        {
            controlador.velocidad -= factorReduccion * Time.deltaTime;
            controlador.velocidad = Mathf.Max(controlador.velocidad, controlador.velocidadMinima);
            yield return null;
        }
    }

}
