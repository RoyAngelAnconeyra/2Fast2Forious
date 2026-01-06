using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Maneja la entrada táctil con botones en pantalla para controles móviles.
/// Proporciona un valor HorizontalInput que puede usarse igual que Input.GetAxis("Horizontal").
/// </summary>
public class TouchInput : MonoBehaviour
{
    // Singleton para acceso fácil desde otros scripts
    public static TouchInput Instance { get; private set; }

    /// <summary>
    /// Valor de entrada horizontal: -1 (izquierda), 0 (neutro), +1 (derecha)
    /// </summary>
    public float HorizontalInput { get; private set; } = 0f;

    // Estados de los botones
    private bool leftPressed = false;
    private bool rightPressed = false;

    void Awake()
    {
        // Configurar singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Update()
    {
        // Calcular input basado en qué botones están presionados
        if (leftPressed && !rightPressed)
        {
            HorizontalInput = -1f;
        }
        else if (rightPressed && !leftPressed)
        {
            HorizontalInput = 1f;
        }
        else
        {
            HorizontalInput = 0f;
        }
    }

    // Métodos públicos para ser llamados por los Event Triggers de los botones UI

    /// <summary>
    /// Llamado cuando se presiona el botón izquierdo (PointerDown)
    /// </summary>
    public void OnLeftButtonDown()
    {
        leftPressed = true;
    }

    /// <summary>
    /// Llamado cuando se suelta el botón izquierdo (PointerUp)
    /// </summary>
    public void OnLeftButtonUp()
    {
        leftPressed = false;
    }

    /// <summary>
    /// Llamado cuando se presiona el botón derecho (PointerDown)
    /// </summary>
    public void OnRightButtonDown()
    {
        rightPressed = true;
    }

    /// <summary>
    /// Llamado cuando se suelta el botón derecho (PointerUp)
    /// </summary>
    public void OnRightButtonUp()
    {
        rightPressed = false;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
