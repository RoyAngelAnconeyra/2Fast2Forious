using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

public class MotorCarreteras : MonoBehaviour
{
    public GameObject contenedorCallesGO;
    public GameObject[] contenedorCallesArray;
    public float velocidad;
    public bool inicioJuego;
    public bool juegoTerminado;
    int contadorCalles = 0;
    int numeroSelectorCalles;

    public GameObject calleAnterior;
    public GameObject calleNueva;

    public float tamañoCalle = 0;

    public Vector3 medidaLimitePantalla;
    public bool salioDePantalla;

    public GameObject mCamGo;
    public Camera mCamComp;

    public GameObject cocheGO;
    public GameObject audioFxGO;
    public AudioFX audioFXScript;
    public GameObject bgFinalGO;

    private float margenCreacion = 30f;
    private List<GameObject> piezasActivas = new List<GameObject>();


    void Start()
    {
        InicioJuego();
    }

    void InicioJuego()
    {
        contenedorCallesGO = GameObject.Find("ContenedorCalles");
        
        mCamGo = GameObject.Find("Main Camera");
        mCamComp = mCamGo.GetComponent<Camera>();

        bgFinalGO = GameObject.Find("PanelGameOver");
        bgFinalGO.SetActive(false);

        audioFxGO = GameObject.Find("AudioFX");
        audioFXScript = audioFxGO.GetComponent<AudioFX>();

        cocheGO = GameObject.FindAnyObjectByType<Coche>().gameObject;

        VelocidadMotorCarretera();
        MedirPantalla();
        BuscoCalles();
    }

    public void JuegoTerminadoEstados()
    {
        cocheGO.GetComponent<AudioSource>().Stop();
        audioFXScript.FXMusic();
        bgFinalGO.SetActive(true);
    }

    void VelocidadMotorCarretera()
    {
        velocidad = 18;
    }

    void BuscoCalles()
    {
        contenedorCallesArray = Resources.LoadAll<GameObject>("Calles");
        CrearPieza();
    }
    
    void CrearPieza()
    {
        contadorCalles++;
        calleAnterior = GameObject.Find("Calle" + (contadorCalles - 1));
        string tipoFinAnterior;

        //Elegir la más adecuada calle según inicio y fin
        if (calleAnterior.transform.childCount > 0)
        {
            Pieza ultimaPieza = ObtenerPiezaMasAlta();
            tipoFinAnterior = ultimaPieza.finTipo;
        }
        else
        {
            tipoFinAnterior = calleAnterior.GetComponent<Pieza>().finTipo;
        }
        List<GameObject> compatibles = new List<GameObject>();
        foreach (GameObject prefab in contenedorCallesArray)
        {
            Pieza script = prefab.GetComponent<Pieza>();
            if (script != null && script.inicioTipo == tipoFinAnterior)
            {
                compatibles.Add(prefab);
            }
        }

        if (compatibles.Count == 0)
        {
            Debug.LogWarning("No hay piezas compatibles para tipo: " + tipoFinAnterior);
            return;
        }

        // Elige una pieza compatible al azar
        GameObject seleccionada = compatibles[Random.Range(0, compatibles.Count)];
        GameObject nuevaPieza = Instantiate(seleccionada);
        nuevaPieza.name = "Calle" + contadorCalles;
        nuevaPieza.transform.parent = gameObject.transform;
        piezasActivas.Add(nuevaPieza);

        calleNueva = nuevaPieza;

        PosicionoCalles();
    }

    void PosicionoCalles()
    {
        //MidoCalle();
        tamañoCalle += MidoCalle(calleAnterior);
        calleNueva.transform.position = new Vector3(calleAnterior.transform.position.x, calleAnterior.transform.position.y + tamañoCalle, 0);
        salioDePantalla = false;
    }

    Pieza ObtenerPiezaMasAlta()
    {
        Pieza piezaMasAlta = null;
        float mayorY = float.MinValue;

        // Recorrer todos los hijos
        for (int i = 0; i < calleAnterior.transform.childCount; i++)
        {
            Transform hijo = calleAnterior.transform.GetChild(i);
            Pieza pieza = hijo.GetComponent<Pieza>();
            float posY = hijo.position.y;

            // Verificar si este hijo es el más alto
            if (posY > mayorY)
            {
                mayorY = posY;
                piezaMasAlta = pieza;
            }
        }

        return piezaMasAlta;
    }
    
    float MidoCalle(GameObject piezaContenedor)
    {
        float tamanho = 0;
        tamañoCalle = 0;
        if (piezaContenedor.transform.childCount > 0)
        {
            for (int i = 0; i < piezaContenedor.transform.childCount; i++)
            {
                if (calleAnterior.transform.GetChild(i).gameObject.GetComponent<Pieza>() != null)
                {
                    tamanho = calleAnterior.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().bounds.size.y;
                }
            }
        }
        else{
            tamanho = piezaContenedor.GetComponent<SpriteRenderer>().bounds.size.y;
        }
        return tamanho;

    }

    void MedirPantalla()
    {
        medidaLimitePantalla = new Vector3(0, mCamComp.ScreenToWorldPoint(new Vector3(0, 0, 0)).y - 0.5f, 0);
    }

    void Update()
    {
        if (inicioJuego == true && juegoTerminado == false)
        {
            transform.Translate(Vector3.down * velocidad * Time.deltaTime);
            float puntoAnticipacion = medidaLimitePantalla.y + margenCreacion;
            if (calleAnterior.transform.position.y + tamañoCalle < puntoAnticipacion && salioDePantalla == false)
            {
                CrearPieza();
            }
            // Destruye la pista anterior solo cuando ya salió totalmente
            for (int i = piezasActivas.Count - 1; i >= 0; i--)
            {
                float piezaY = piezasActivas[i].transform.position.y;
                float piezaTamaño = MidoCalle(piezasActivas[i]);
                if (piezaY + piezaTamaño < medidaLimitePantalla.y)
                {
                    Destroy(piezasActivas[i]);
                    piezasActivas.RemoveAt(i);
                }
            }
        }
            
    }

    void DestruyoCalles()
    {
        Destroy(calleAnterior);
        calleAnterior = null;
        calleAnterior = calleNueva;
        CrearPieza();
    }
}
