using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

public class MotorCarreteras : MonoBehaviour
{
    public GameObject contenedorCallesGO;
    public GameObject[] contenedorCallesArray;
    public string nombreNivel;
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
    // Cached references and performance helpers
    private GameObject motorVehiculosGO;
    private Transform motorVehiculosTransform;
    private Dictionary<GameObject, float> piezaAlturas = new Dictionary<GameObject, float>();
    [SerializeField]
    private LayerMask spawnCollisionMask = ~0;
    private Collider2D[] overlapBuffer = new Collider2D[16];
    private ContactFilter2D spawnContactFilter;

    [Header("CocheObstaculos")]
    public GameObject[] prefacVehiculos;
    [Tooltip("Posiciones X de los carriles donde pueden aparecer vehículos")]
    public float[] carrilsSpawn = { -6f, -3f, 0f, 3f, 6f };

    [Header("Velocidades del Juego")]
    public float velocidadPista = 5f;        // Velocidad de las piezas/calles
    public float velocidadVehiculosBase = 7f; // Velocidad BASE de vehículos
    public float velocidadFondos = 3f;        // Velocidad de fondos parallax
    [Range(0.0f, 0.5f)]
    public float variacionVehiculos = 0.25f;   // Rango de variación: ±[0-50%] de velocidad base (0.3 = ±30%)

    void Start()
    {
        controladorCoche = FindAnyObjectByType<ControladorCoche>();
        // Inicializa la velocidad GLOBAL a la velocidad mínima definida en el coche
        velocidadPista = controladorCoche.velocidadMinima;
        // Velocidad base de vehículos 1.5x la pista para asegurar que incluso con variación mínima (0.7)
        // los vehículos se muevan más rápido que la pista: 1.5 * 0.7 = 1.05 > 1.0 (evita inmobilidad visual)
        velocidadVehiculosBase = controladorCoche.velocidadMinima * 1.5f;
        velocidadFondos = controladorCoche.velocidadMinima * 0.6f;
        controladorCoche.velocidad = velocidadPista;
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

        // Cache MotorVehiculos container to avoid repeated Find calls later
        motorVehiculosGO = GameObject.Find("MotorVehiculos");
        if (motorVehiculosGO != null) motorVehiculosTransform = motorVehiculosGO.transform;

        // Configure contact filter for non-alloc overlap checks
        spawnContactFilter = new ContactFilter2D();
        spawnContactFilter.SetLayerMask(spawnCollisionMask);
        spawnContactFilter.useTriggers = true;

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
        string rutaCalles = "Calles/" + nombreNivel;
        contenedorCallesArray = Resources.LoadAll<GameObject>(rutaCalles);

        if (contenedorCallesArray.Length == 0)
        {
            Debug.LogError("No se encontraron piezas en Resources/Calles/" + nombreNivel + ". Verifica la carpeta y el nombre del nivel.");
            return;
        }

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
        // Cache the new piece's height to avoid repeated bounds calculations
        float alturaNueva = MidoCalle(nuevaPieza);
        piezaAlturas[nuevaPieza] = alturaNueva;

        calleNueva = nuevaPieza;

        PosicionoCalles();
        PosicionVehiculos();
    }

    void PosicionoCalles()
    {
        float alturaAnterior = piezaAlturas.ContainsKey(calleAnterior) ? piezaAlturas[calleAnterior] : MidoCalle(calleAnterior);
        tamañoCalle = alturaAnterior;
        calleNueva.transform.position = new Vector3(calleAnterior.transform.position.x, calleAnterior.transform.position.y + alturaAnterior - 0.05f, 0);
        salioDePantalla = false;
    }

    /// <summary>
    /// Mezcla una lista in-place usando Fisher-Yates shuffle.
    /// </summary>
    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    void PosicionVehiculos()
    {
        if (contadorCalles <= 1 || !inicioJuego) return;
        if (carrilsSpawn == null || carrilsSpawn.Length == 0) return;

        // Máximo de vehículos por pieza según tiempo transcurrido
        int segundos = Mathf.FloorToInt(Time.timeSinceLevelLoad);
        int maxVehiculos = 1 + (segundos / 30); // aumenta en 1 cada 30s

        // Obtén el SpriteRenderer de la nueva pieza
        SpriteRenderer sr = calleNueva.GetComponent<SpriteRenderer>();
        if (sr == null) { Debug.Log("No hay SpriteRender en calleNueva."); return; }

        float alto = sr.bounds.size.y;

        // Región Y válida (evita bordes extremos)
        float yBase = calleNueva.transform.position.y - (alto / 2f);
        float yMin = yBase + alto * 0.15f;
        float yMax = yBase + alto * 0.85f;

        // Usar carriles fijos mezclados para variedad
        List<float> carriles = new List<float>(carrilsSpawn);
        Shuffle(carriles);

        int vehiculosCreados = 0;
        foreach (float carrilX in carriles)
        {
            if (vehiculosCreados >= maxVehiculos) break;

            float randY = Random.Range(yMin, yMax);
            Vector2 pos = new Vector2(carrilX, randY);

            // Verificar que el punto esté libre (sin TerrenoLento ni CocheObstaculo)
            int hitCount = Physics2D.OverlapPoint(pos, spawnContactFilter, overlapBuffer);
            bool permitido = true;
            for (int h = 0; h < hitCount; h++)
            {
                var col = overlapBuffer[h];
                if (col == null) continue;
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
                if (motorVehiculosTransform != null) vehiculo.transform.parent = motorVehiculosTransform;
                else vehiculo.transform.parent = gameObject.transform;

                // Sincronizar velocidad con la pista
                var obs = vehiculo.GetComponent<CocheObstaculo>();
                if (obs != null)
                {
                    // 1.1x a 1.6x de velocidad de pista (siempre se mueven hacia adelante)
                    float variacion = Random.Range(1.1f, 1.1f + variacionVehiculos * 2f);
                    obs.velocidadBajada = velocidadPista * variacion;
                }
                vehiculosCreados++;
            }
        }
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
        float tamanho = 0f;
        if (piezaContenedor == null) return 0f;
        if (piezaContenedor.transform.childCount > 0)
        {
            for (int i = 0; i < piezaContenedor.transform.childCount; i++)
            {
                var child = piezaContenedor.transform.GetChild(i).gameObject;
                if (child.GetComponent<Pieza>() != null)
                {
                    var sr = child.GetComponent<SpriteRenderer>();
                    if (sr != null) tamanho += sr.bounds.size.y;
                }
            }
        }
        else
        {
            var sr = piezaContenedor.GetComponent<SpriteRenderer>();
            if (sr != null) tamanho = sr.bounds.size.y;
        }
        return tamanho;
    }

    void MedirPantalla()
    {
        medidaLimitePantalla = new Vector3(0, mCamComp.ScreenToWorldPoint(new Vector3(0, 0, 0)).y - 0.5f, 0);
    }

    /// <summary>
    /// Sincroniza las velocidades de todos los vehículos activos con la nueva velocidad de pista.
    /// Evita que los vehículos se queden atrás cuando aumenta la velocidad global.
    /// </summary>
    void SincronizarVelocidadesVehiculos()
    {
        if (motorVehiculosTransform == null) return;

        for (int i = 0; i < motorVehiculosTransform.childCount; i++)
        {
            var child = motorVehiculosTransform.GetChild(i);
            var obs = child.GetComponent<CocheObstaculo>();
            if (obs != null)
            {
                // Si la velocidad del vehículo es menor que la pista, re-calcular
                // para mantener proporción y evitar que parezca ir hacia atrás
                if (obs.velocidadBajada < velocidadPista * 1.1f)
                {
                    float variacion = Random.Range(1.1f, 1.1f + variacionVehiculos * 2f);
                    obs.velocidadBajada = velocidadPista * variacion;
                }
            }
        }
    }

    void Update()
    {
        if (inicioJuego == true && juegoTerminado == false)
        {
            // Mantener sincronizdas las velocidades globales
            float tiempoActual = Time.timeSinceLevelLoad;
            float minVel = controladorCoche.velocidadMinima;
            float maxVel = controladorCoche.velocidadMaxima;

            // Cada 30s, sube la velocidad +5f si no llega al máximo
            if (tiempoActual > tiempoUltimoIncremento + 30f)
            {
                velocidadPista = Mathf.Min(velocidadPista + 5f, maxVel);
                velocidadVehiculosBase = Mathf.Min(velocidadVehiculosBase + 5f, maxVel);
                velocidadFondos = Mathf.Min(velocidadFondos + 3f, maxVel * 0.8f); // Fondos más lentos
                controladorCoche.velocidad = velocidadPista;
                tiempoUltimoIncremento = tiempoActual;

                // Sincronizar velocidades de vehículos existentes para evitar que se queden atrás
                SincronizarVelocidadesVehiculos();
            }

            transform.Translate(Vector3.down * velocidadPista * Time.deltaTime);
            float puntoAnticipacion = medidaLimitePantalla.y + margenCreacion;
            float alturaAnterior = piezaAlturas.ContainsKey(calleAnterior) ? piezaAlturas[calleAnterior] : MidoCalle(calleAnterior);
            if (calleAnterior != null && calleAnterior.transform.position.y + alturaAnterior < puntoAnticipacion && salioDePantalla == false)
            {
                CrearPieza();
            }
            // Destruye la pista anterior solo cuando ya salió totalmente
            for (int i = piezasActivas.Count - 1; i >= 0; i--)
            {
                float piezaY = piezasActivas[i].transform.position.y;
                float piezaTamaño = piezaAlturas.ContainsKey(piezasActivas[i]) ? piezaAlturas[piezasActivas[i]] : MidoCalle(piezasActivas[i]);
                if (piezaY + piezaTamaño < medidaLimitePantalla.y)
                {
                    var go = piezasActivas[i];
                    piezaAlturas.Remove(go);
                    Destroy(go);
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
