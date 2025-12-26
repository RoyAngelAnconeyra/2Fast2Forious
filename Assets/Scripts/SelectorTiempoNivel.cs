using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SelectorTiempoNivel : MonoBehaviour
{
    [Header("UI Tiempo")]
    public TextMeshProUGUI txtTiempoSeleccionado;
    public Button btnFlechaIzq;
    public Button btnFlechaDer;
    /*
    [Header("UI Nivel")]
    public TextMeshProUGUI txtNivelSeleccionado;
    public Button btnPasto;
    public Button btnNieve;
    public Button btnDesierto;
    */
    [Header("Botón Jugar")]
    public Button btnJugar;
    
    private int tiempoSeleccionado = 3; // Empieza en 3 minutos
    //private string nivelSeleccionado = "Pasto"; // Nivel por defecto
    
    void Start()
    {
        // Configurar botones
        btnFlechaIzq.onClick.AddListener(() => CambiarTiempo(-1));
        btnFlechaDer.onClick.AddListener(() => CambiarTiempo(1));
        btnJugar.onClick.AddListener(Jugar);
        
        ActualizarUI();
    }
    
    void CambiarTiempo(int delta)
    {
        tiempoSeleccionado = Mathf.Clamp(tiempoSeleccionado + delta, 1, 10);
        ActualizarUI();
    }
    
    void ActualizarUI()
    {
        txtTiempoSeleccionado.text = tiempoSeleccionado.ToString();
    }

    void Jugar()
    {
        // Guardar datos para el juego
        PlayerPrefs.SetInt("TiempoJuego", tiempoSeleccionado * 60); // Convertir a segundos
        PlayerPrefs.Save();
        

    }
}
