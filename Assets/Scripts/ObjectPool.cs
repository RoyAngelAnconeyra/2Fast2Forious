using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Singleton object pool manager for vehicles and road pieces.
/// Reduces garbage collection by reusing GameObjects.
/// </summary>
public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    [Header("Vehicle Pooling")]
    [SerializeField] private int vehiculoPoolSize = 20;
    private List<GameObject> vehiculoPool = new List<GameObject>();
    private GameObject[] vehiculoPrefabs;
    private Transform vehiculosParent;

    [Header("Pieza Pooling")]
    [SerializeField] private int piezaPoolSizePerType = 3;
    // Dictionary: inicioTipo -> List of pooled pieces with that type
    private Dictionary<string, List<GameObject>> piezaPools = new Dictionary<string, List<GameObject>>();
    private GameObject[] piezaPrefabs;
    private Transform piezasParent;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Initialize vehicle pool with prefabs loaded from Resources/Vehiculos.
    /// </summary>
    public void InitializeVehiculos(GameObject[] prefabs, Transform parent)
    {
        vehiculoPrefabs = prefabs;
        vehiculosParent = parent;

        // Prewarm pool
        for (int i = 0; i < vehiculoPoolSize && vehiculoPrefabs.Length > 0; i++)
        {
            GameObject prefab = vehiculoPrefabs[Random.Range(0, vehiculoPrefabs.Length)];
            GameObject obj = Instantiate(prefab, vehiculosParent);
            obj.SetActive(false);
            vehiculoPool.Add(obj);
        }
    }

    /// <summary>
    /// Initialize pieza pools with prefabs loaded from Resources/Calles/{level}.
    /// Creates separate pools per inicioTipo.
    /// </summary>
    public void InitializePiezas(GameObject[] prefabs, Transform parent)
    {
        piezaPrefabs = prefabs;
        piezasParent = parent;

        // Group prefabs by inicioTipo
        Dictionary<string, List<GameObject>> prefabsByType = new Dictionary<string, List<GameObject>>();
        foreach (var prefab in prefabs)
        {
            Pieza pieza = prefab.GetComponent<Pieza>();
            if (pieza != null)
            {
                if (!prefabsByType.ContainsKey(pieza.inicioTipo))
                    prefabsByType[pieza.inicioTipo] = new List<GameObject>();
                prefabsByType[pieza.inicioTipo].Add(prefab);
            }
        }

        // Prewarm pools for each type
        foreach (var kvp in prefabsByType)
        {
            string tipo = kvp.Key;
            List<GameObject> typePrefabs = kvp.Value;
            piezaPools[tipo] = new List<GameObject>();

            for (int i = 0; i < piezaPoolSizePerType && typePrefabs.Count > 0; i++)
            {
                GameObject prefab = typePrefabs[Random.Range(0, typePrefabs.Count)];
                GameObject obj = Instantiate(prefab, piezasParent);
                obj.SetActive(false);
                piezaPools[tipo].Add(obj);
            }
        }
    }

    /// <summary>
    /// Get a vehicle from pool, or instantiate if pool is empty.
    /// </summary>
    public GameObject GetVehiculo()
    {
        // Find inactive pooled vehicle
        for (int i = 0; i < vehiculoPool.Count; i++)
        {
            if (vehiculoPool[i] != null && !vehiculoPool[i].activeInHierarchy)
            {
                vehiculoPool[i].SetActive(true);
                return vehiculoPool[i];
            }
        }

        // Pool exhausted, create new
        if (vehiculoPrefabs != null && vehiculoPrefabs.Length > 0)
        {
            GameObject prefab = vehiculoPrefabs[Random.Range(0, vehiculoPrefabs.Length)];
            GameObject obj = Instantiate(prefab, vehiculosParent);
            vehiculoPool.Add(obj);
            return obj;
        }

        return null;
    }

    /// <summary>
    /// Return a vehicle to the pool.
    /// </summary>
    public void ReturnVehiculo(GameObject vehiculo)
    {
        if (vehiculo != null)
        {
            vehiculo.SetActive(false);
            // Reparent in case it was moved
            if (vehiculosParent != null)
                vehiculo.transform.SetParent(vehiculosParent);
        }
    }

    /// <summary>
    /// Get a road piece matching the specified inicioTipo.
    /// </summary>
    public GameObject GetPieza(string inicioTipo)
    {
        // Check if we have a pool for this type
        if (piezaPools.ContainsKey(inicioTipo))
        {
            List<GameObject> pool = piezaPools[inicioTipo];
            for (int i = 0; i < pool.Count; i++)
            {
                if (pool[i] != null && !pool[i].activeInHierarchy)
                {
                    pool[i].SetActive(true);
                    return pool[i];
                }
            }
        }

        // Pool empty or no pool for this type - find compatible prefab and instantiate
        List<GameObject> compatibles = new List<GameObject>();
        foreach (var prefab in piezaPrefabs)
        {
            Pieza pieza = prefab.GetComponent<Pieza>();
            if (pieza != null && pieza.inicioTipo == inicioTipo)
                compatibles.Add(prefab);
        }

        if (compatibles.Count > 0)
        {
            GameObject prefab = compatibles[Random.Range(0, compatibles.Count)];
            GameObject obj = Instantiate(prefab, piezasParent);

            // Add to pool for future reuse
            if (!piezaPools.ContainsKey(inicioTipo))
                piezaPools[inicioTipo] = new List<GameObject>();
            piezaPools[inicioTipo].Add(obj);

            return obj;
        }

        Debug.LogWarning($"ObjectPool: No compatible pieza found for type '{inicioTipo}'");
        return null;
    }

    /// <summary>
    /// Return a road piece to the pool based on its inicioTipo.
    /// </summary>
    public void ReturnPieza(GameObject pieza)
    {
        if (pieza == null) return;

        Pieza piezaComp = pieza.GetComponent<Pieza>();
        if (piezaComp != null)
        {
            pieza.SetActive(false);
            if (piezasParent != null)
                pieza.transform.SetParent(piezasParent);
        }
    }
}
