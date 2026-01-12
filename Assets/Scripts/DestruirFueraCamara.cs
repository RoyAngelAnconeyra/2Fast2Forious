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
            // Return to pool if this is a vehicle, otherwise destroy
            CocheObstaculo vehiculo = GetComponent<CocheObstaculo>();
            if (vehiculo != null && ObjectPool.Instance != null)
            {
                ObjectPool.Instance.ReturnVehiculo(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
