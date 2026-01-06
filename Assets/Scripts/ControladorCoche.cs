using UnityEngine;
using System.Collections;
public class ControladorCoche : MonoBehaviour
{
    public GameObject cocheGO;
    public float anguloDeGiro;
    public float velocidad;
    public float velocidadMinima = 5f;
    public float velocidadMaxima = 20f;
    public float velocidadAumento = 0.5f;

    [HideInInspector]
    public bool estaEnTerrenoLento = false;

    void Start()
    {
        cocheGO = GameObject.FindAnyObjectByType<Coche>().gameObject;
    }

    /// <summary>
    /// Obtiene la entrada horizontal combinando teclado y controles táctiles.
    /// Prioriza touch si está disponible, sino usa teclado.
    /// </summary>
    float GetHorizontalInput()
    {
        // Primero intentar touch input (para móvil)
        if (TouchInput.Instance != null && TouchInput.Instance.HorizontalInput != 0f)
        {
            return TouchInput.Instance.HorizontalInput;
        }
        
        // Fallback a teclado/gamepad (para PC)
        return Input.GetAxis("Horizontal");
    }

    void FixedUpdate()
    {
        float horizontalInput = GetHorizontalInput();
        float giroEnZ = 0;
        transform.Translate(Vector2.right * horizontalInput * velocidad * Time.deltaTime);

        // Limitar el movimiento entre -9 y 9 en X
        Vector3 posicion = transform.position;
        posicion.x = Mathf.Clamp(posicion.x, -8.5f, 8.5f);
        transform.position = posicion;

        giroEnZ = horizontalInput * -anguloDeGiro;

        cocheGO.transform.rotation = Quaternion.Euler(0,0,giroEnZ);
        if (!estaEnTerrenoLento && velocidad < velocidadMaxima)
        {
            velocidad += velocidadAumento * Time.deltaTime;
            velocidad = Mathf.Min(velocidad, velocidadMaxima);
        }
    }

}
