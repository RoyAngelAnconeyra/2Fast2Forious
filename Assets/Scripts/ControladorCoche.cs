using UnityEngine;
using System.Collections;
public class ControladorCoche : MonoBehaviour
{
    public GameObject cocheGO;
    public float anguloDeGiro;
    public float velocidad;
    void Start()
    {
        cocheGO = GameObject.FindAnyObjectByType<Coche>().gameObject;
    }

    void FixedUpdate()
    {
        float giroEnZ = 0;
        transform.Translate(Vector2.right * Input.GetAxis("Horizontal") * velocidad * Time.deltaTime);

        giroEnZ = Input.GetAxis("Horizontal") * -anguloDeGiro;

        cocheGO.transform.rotation = Quaternion.Euler(0,0,giroEnZ);
        
    }
}
