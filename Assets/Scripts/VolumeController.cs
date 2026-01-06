using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla el volumen global del juego usando un Slider de UI.
/// Guarda la preferencia del usuario en PlayerPrefs.
/// </summary>
public class VolumeController : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Slider para controlar el volumen (0-1)")]
    public Slider volumeSlider;

    // Clave para guardar en PlayerPrefs
    private const string VOLUME_KEY = "GameVolume";

    void Start()
    {
        // Cargar volumen guardado (1.0 por defecto)
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 1f);
        
        // Aplicar al AudioListener (controla todo el audio del juego)
        AudioListener.volume = savedVolume;
        
        // Sincronizar el slider si existe
        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
    }

    /// <summary>
    /// Llamado cuando el slider cambia de valor.
    /// </summary>
    public void OnVolumeChanged(float value)
    {
        // Aplicar volumen global
        AudioListener.volume = value;
        
        // Guardar preferencia
        PlayerPrefs.SetFloat(VOLUME_KEY, value);
        PlayerPrefs.Save();
    }

    void OnDestroy()
    {
        // Limpiar listener para evitar memory leaks
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
        }
    }
}
