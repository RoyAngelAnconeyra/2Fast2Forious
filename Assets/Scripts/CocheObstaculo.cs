using UnityEngine;

public class CocheObstaculo : MonoBehaviour
{
    public GameObject cronometroGO;
    public Cronometro cronometroScript;
    public GameObject audioFXGO;
    public AudioFX audioFXScript;

    void Start()
    {
        cronometroGO = GameObject.FindAnyObjectByType<Cronometro>().gameObject;
        cronometroScript = cronometroGO.GetComponent<Cronometro>();

        audioFXGO = GameObject.FindAnyObjectByType<AudioFX>().gameObject;
        audioFXScript = audioFXGO.GetComponent<AudioFX>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<Coche>()!= null)
        {
            audioFXScript.FXSonidoChoque();
            cronometroScript.tiempo = cronometroScript.tiempo -20;
            Destroy(this.gameObject);
        }
    }
}
