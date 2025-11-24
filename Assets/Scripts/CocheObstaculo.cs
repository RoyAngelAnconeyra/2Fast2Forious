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
        cronometroGO = GameObject.FindAnyObjectByType<Cronometro>().gameObject;
        cronometroScript = cronometroGO.GetComponent<Cronometro>();

        audioFXGO = GameObject.FindAnyObjectByType<AudioFX>().gameObject;
        audioFXScript = audioFXGO.GetComponent<AudioFX>();

        quitarTiempoGO = GameObject.Find("QuitarTiempo");
        quitarTiempo = quitarTiempoGO.GetComponent<TextMeshProUGUI>();
        quitarTiempo.text = "";
        
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

            Destroy(this.gameObject);
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
