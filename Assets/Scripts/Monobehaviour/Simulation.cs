using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Simulation : MonoBehaviour
{
    #region Static 

    private static Simulation instance;

    #endregion

[ReadOnly]
    public Nest nestPrefab;

    [ReadOnly]
    public Food foodPrefab;

    [ReadOnly]
    public Predator predatorPrefab;

    [ReadOnly]
    public float startDelay = 1f;

    [ReadOnly]
    public bool nestSpawnEnabled = false;
    private bool canSpawnNests = false;

    [ReadOnly]
    public int maxNumberOfNests = 1;

    [ReadOnly]
    public float nestSpawnRate = 0.25f;

    [ReadOnly]
    public float minNestSpawnRadius = 50;

    [ReadOnly]
    public float maxNestSpawnRadius = 100;

#if UNITY_EDITOR
    [ReadOnly(RunMode.Any)]
    public int numberOfSpawnedNests;
#endif

    [ReadOnly]
    public uint pheromoneGridWidth = 100;

    [ReadOnly]
    public uint pheromoneGridHeight = 100;

    [ReadOnly]
    public float pheromoneGridSquareSize = 1.0f;

    private List<Predator> predators = new List<Predator>();
    private List<Nest> nests = new List<Nest>();

    private Pheromone[,] pheromoneGrid;     // initialized on Awake()
    private float nestSpawnDeltaTime;

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(startDelay);
        predators.Add(CreatePredator());
        nestSpawnEnabled = true;
    }

    private Predator CreatePredator()
    {
        Predator predator = Instantiate<Predator>(predatorPrefab);
        Camera.main.GetComponent<CameraFollow>().target = predator.transform;
        return predator;
    }

    private Nest CreateNest()
    {
        float spawnDistance = Random.Range(minNestSpawnRadius, maxNestSpawnRadius);
        Vector2 point = (Random.insideUnitCircle.normalized) * spawnDistance;
        Vector3 spawnPoint = transform.position + new Vector3(point.x, 0, point.y);

        Nest nest = Instantiate<Nest>(nestPrefab);
        nest.transform.position = spawnPoint;
        return nest;
    }

    #region Unity Events

    void Awake()
    {
        if  (Simulation.instance == null)
        {
            Simulation.instance = this;

            UnityEngine.Random.seed = System.Environment.TickCount;
            pheromoneGrid = new Pheromone[pheromoneGridWidth, pheromoneGridHeight];

            // TODO: Spawn Nests
            // TODO: Spawn Food Sources
            // TODO: Spawn Predators
        }
        else
        {
            Debug.LogErrorFormat("Another Simulation instance was found. This component will be removed.");
            Destroy(this);
        }
    }

    void Start()
    {
        StartCoroutine(DelayedStart());
    }


    void Update()
    {
        if (canSpawnNests != nestSpawnEnabled)
        {
            if (nestSpawnEnabled)
                nestSpawnDeltaTime = 0;

            canSpawnNests = nestSpawnEnabled;
        }

        if (canSpawnNests)
        {
            nestSpawnDeltaTime += Time.deltaTime;
            float spawnPeriod = 1f / nestSpawnRate;
            while (nests.Count < maxNumberOfNests && nestSpawnDeltaTime >= spawnPeriod)
            {
                Nest nest = CreateNest();
                nests.Add(nest);

                #if UNITY_EDITOR
                numberOfSpawnedNests = nests.Count;
                #endif

                nestSpawnDeltaTime -= spawnPeriod;
            }
        }
    }

    #endregion

}
