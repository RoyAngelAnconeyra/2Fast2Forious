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
    private ControladorCoche controladorCoche;
    private float tiempoUltimoIncremento = 0f;

    [Header("CocheObstaculos")]
    public GameObject[] prefacVehiculos;


    void Start()
    {
        controladorCoche = FindAnyObjectByType<ControladorCoche>();
        // Inicializa la velocidad GLOBAL a la velocidad mínima definida en el coche
        velocidad = controladorCoche.velocidad = controladorCoche.velocidadMinima;
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
        BuscoVehiculos();
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

    void BuscoVehiculos()
    {
        prefacVehiculos = Resources.LoadAll<GameObject>("Vehiculos");
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
        PosicionVehiculos();
    }

    void PosicionoCalles()
    {
        tamañoCalle += MidoCalle(calleAnterior);
        calleNueva.transform.position = new Vector3(calleAnterior.transform.position.x, calleAnterior.transform.position.y + tamañoCalle, 0);
        salioDePantalla = false;
    }

    void PosicionVehiculos()
    {
        if (contadorCalles <= 1 || !inicioJuego) return;

        // Máximo de vehículos por pieza segun tiempo transcurrido
        int segundos = Mathf.FloorToInt(Time.timeSinceLevelLoad);
        int maxVehiculos = 1 + (segundos / 30); //aumenta en 1 cada 30s

        // Obtén el SpriteRenderer de la nueva pieza (el área visible de la pista)
        SpriteRenderer sr = calleNueva.GetComponent<SpriteRenderer>();
        if(sr == null){Debug.Log("No hay SpriteRender en calleNueva.");}

        float ancho = sr.bounds.size.x;
        float alto = sr.bounds.size.y;

        // Región Y válida (evita bordes extremos)
        float yBase = calleNueva.transform.position.y - (alto / 2f);
        float yMin = yBase + alto * 0.10f;
        float yMax = yBase + alto * 0.90f;

        List<(float xMin, float xMax)> franjas = BuscarIntervalosSinColliders(sr, calleNueva, (yMin + yMax) / 2f);


        if (franjas.Count == 0) return;

        for (int vehiculos = 0; vehiculos < maxVehiculos && franjas.Count > 0; vehiculos++)
        {
            // Elige franja al azar
            var franja = franjas[Random.Range(0, franjas.Count)];
            float randX = Random.Range(franja.xMin, franja.xMax);

            // Elige Y en la zona segura (puedes variar o usar el centro)
            float randY = Random.Range(yMin, yMax);
            Vector2 pos = new Vector2(randX, randY);

            // Instancia el vehículo sólo si el punto está también libre en Y
            Collider2D[] sobreZonas = Physics2D.OverlapPointAll(pos);
            bool permitido = true;
            foreach (var col in sobreZonas)
            {
                if (col.GetComponent<TerrenoLento>() != null || col.GetComponent<CocheObstaculo>() != null)
                {
                    permitido = false;
                    break;
                }
            }
            if (permitido && prefacVehiculos.Length > 0)
            {
                GameObject prefab = prefacVehiculos[Random.Range(0, prefacVehiculos.Length)];
                GameObject vehiculo = Instantiate(prefab, pos, Quaternion.identity);
                vehiculo.transform.parent = GameObject.Find("MotorVehiculos").transform;

                // Sincronizar la velocidad del vehículo con la velocidad global de la pista
                var obs = vehiculo.GetComponent<CocheObstaculo>();
                if (obs != null) obs.velocidadBajada = velocidad;
            }
        }
    }

    List<(float xMin, float xMax)> BuscarIntervalosSinColliders(SpriteRenderer sr, GameObject pieza, float yMedio)
    {
        List<(float, float)> huecos = new List<(float, float)>();
        float ancho = sr.bounds.size.x;
        float xBase = sr.bounds.center.x;
        float paso = 0.2f; // más bajo = más preciso, más lento
        float rangoMin = xBase - ancho * 0.5f;
        float rangoMax = xBase + ancho * 0.5f;

        float xMinLibre = float.NaN;
        bool adentro = false;
        for (float x = rangoMin; x <= rangoMax; x += paso)
        {
            Vector2 pos = new Vector2(x, yMedio);
            Collider2D[] colls = Physics2D.OverlapPointAll(pos);
            bool hayColliderPropio = false;
            foreach (var c in colls)
            {
                if (c.transform.IsChildOf(pieza.transform))
                    hayColliderPropio = true;
            }
            if (!hayColliderPropio)
            {
                if (!adentro) { xMinLibre = x; adentro = true; }
            }
            else
            {
                if (adentro)
                {
                    huecos.Add((xMinLibre, x - paso));
                    adentro = false;
                }
            }
        }
        if (adentro) huecos.Add((xMinLibre, rangoMax));
        return huecos;
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
                    tamanho += calleAnterior.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().bounds.size.y;
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
            // Mantener sincronizdas las velocidades globales
            float tiempoActual = Time.timeSinceLevelLoad;
            float minVel = controladorCoche.velocidadMinima;
            float maxVel = controladorCoche.velocidadMaxima;

            // En cada frame, usa la global actual
            velocidad = controladorCoche.velocidad;

            // Cada 30s, sube la velocidad +5f si no llega al máximo
            if (tiempoActual > tiempoUltimoIncremento + 30f && velocidad < maxVel)
            {
                velocidad = Mathf.Min(velocidad + 5f, maxVel);
                controladorCoche.velocidad = velocidad; // También la del coche
                // También pueden sincronizar así si quieres que el incremento solo se aplique gradualmente:
                // controladorCoche.velocidadMaxima = velocidad;
                tiempoUltimoIncremento = tiempoActual;
            }

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
