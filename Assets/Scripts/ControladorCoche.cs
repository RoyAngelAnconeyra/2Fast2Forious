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

    void FixedUpdate()
    {
        float giroEnZ = 0;
        transform.Translate(Vector2.right * Input.GetAxis("Horizontal") * velocidad * Time.deltaTime);

        // Limitar el movimiento entre -9 y 9 en X
        Vector3 posicion = transform.position;
        posicion.x = Mathf.Clamp(posicion.x, -8.5f, 8.5f);
        transform.position = posicion;

        giroEnZ = Input.GetAxis("Horizontal") * -anguloDeGiro;

        cocheGO.transform.rotation = Quaternion.Euler(0,0,giroEnZ);
        if (!estaEnTerrenoLento && velocidad < velocidadMaxima)
        {
            velocidad += velocidadAumento * Time.deltaTime;
            velocidad = Mathf.Min(velocidad, velocidadMaxima);
        }
    }

}
