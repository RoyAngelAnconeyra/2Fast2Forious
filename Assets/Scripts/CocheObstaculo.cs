using UnityEngine;
using TMPro;
using System.Collections;

public class CocheObstaculo : MonoBehaviour
{
    public GameObject cronometroGO;
    public Cronometro cronometroScript;
    public GameObject audioFXGO;
    public AudioFX audioFXScript;
    public float velocidadBajada = 5;

    public GameObject quitarTiempoGO;
    public TextMeshProUGUI quitarTiempo;

    private MotorCarreteras motorCarreterasScript;

    void Start()
    {
        InitializeReferences();
    }

    void OnEnable()
    {
        // Reset references when reused from pool
        InitializeReferences();
    }

    void InitializeReferences()
    {
        if (cronometroScript == null)
        {
            cronometroGO = GameObject.FindAnyObjectByType<Cronometro>()?.gameObject;
            if (cronometroGO != null) cronometroScript = cronometroGO.GetComponent<Cronometro>();
        }

        if (audioFXScript == null)
        {
            audioFXGO = GameObject.FindAnyObjectByType<AudioFX>()?.gameObject;
            if (audioFXGO != null) audioFXScript = audioFXGO.GetComponent<AudioFX>();
        }

        if (quitarTiempo == null)
        {
            quitarTiempoGO = GameObject.Find("QuitarTiempo");
            if (quitarTiempoGO != null) quitarTiempo = quitarTiempoGO.GetComponent<TextMeshProUGUI>();
        }
        
        if (motorCarreterasScript == null)
            motorCarreterasScript = GameObject.FindAnyObjectByType<MotorCarreteras>();
    }

    void Update()
    {
        // ¡Sólo moverse si el juego NO terminó!
        if (motorCarreterasScript != null && motorCarreterasScript.juegoTerminado) return;

        transform.Translate(Vector3.down * velocidadBajada * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<Coche>()!= null)
        {
            audioFXScript.FXSonidoChoque();
            cronometroScript.tiempo = cronometroScript.tiempo -20;

            StartCoroutine(MostrarPenalizacion());

            // Return to pool instead of destroying
            if (ObjectPool.Instance != null)
                ObjectPool.Instance.ReturnVehiculo(gameObject);
            else
                Destroy(gameObject);
        }
    }

    IEnumerator MostrarPenalizacion()
    {
        quitarTiempo.text = "-00:20";
        //quitarTiempo.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.35f);

        quitarTiempo.text = "";

        //quitarTiempo.gameObject.SetActive(false);
    }

    
}
