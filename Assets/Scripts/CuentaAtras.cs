using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

public class CuentaAtras : MonoBehaviour
{
    public GameObject motorCarrteraGo;
    public MotorCarreteras motorCarreteraScript;
    public Sprite[] numeros;

    public GameObject contadorNumerosGO;
    public SpriteRenderer contadorNumerosComp;
    public GameObject controladorCocheGO;
    public GameObject cocheGo;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InicioComponentes();
    }

    void InicioComponentes()
    {
        motorCarrteraGo = GameObject.Find("MotorCarreteras");
        motorCarreteraScript = motorCarrteraGo.GetComponent<MotorCarreteras>();

        contadorNumerosGO = GameObject.Find("ContadorNumeros");
        contadorNumerosComp = contadorNumerosGO.GetComponent<SpriteRenderer>();

        cocheGo = GameObject.Find("Coche");
        controladorCocheGO = GameObject.Find("ControladorCoche");

        InicioCuentaAtras();
    }

    void InicioCuentaAtras()
    {
        StartCoroutine(Contando());
    }
    
    IEnumerator Contando()
    {
        controladorCocheGO.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(2);

        contadorNumerosComp.sprite = numeros[1];
        this.gameObject.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(1);

        contadorNumerosComp.sprite = numeros[2];
        this.gameObject.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(1);

        contadorNumerosComp.sprite = numeros[3];
        motorCarreteraScript.inicioJuego = true;
        contadorNumerosGO.GetComponent<AudioSource>().Play();
        cocheGo.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(2);

        contadorNumerosGO.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
