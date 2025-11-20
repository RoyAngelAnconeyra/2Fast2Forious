using UnityEngine;

public class DestruirFueraCamara : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        Vector3 punto = cam.WorldToViewportPoint(transform.position);
        if (punto.y < 0) // salió por abajo de la cámara
        {
            Destroy(gameObject);
        }
    }
}
