using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMode : MonoBehaviour
{
    #region Static 

    private static GameMode instance;

    #endregion

    [ReadOnly]
    public Nest nestPrefab;

    [ReadOnly]
    public Food foodPrefab;

    [ReadOnly]
    public Predator predatorPrefab;

    [ReadOnly]
    public float startDelay = 1f;

    [Header("Nests")]

    public bool nestSpawnEnabled = false;

    public int maxNumberOfNests = 1;

    public float nestSpawnRate = 0.25f;

    public float minNestSpawnRadius = 50;

    public float maxNestSpawnRadius = 100;

#if UNITY_EDITOR
    [ReadOnly(RunMode.Any)]
    public int currentNumberOfNests;
#endif

    [Header("Food Sources")]

    public bool foodSpawnEnabled = false;

    public int maxNumberOfFoodSources = 1;

    public float foodSpawnRate = 0.25f;

    public float minFoodSpawnRadius = 10;

    public float maxFoodSpawnRadius = 800;

#if UNITY_EDITOR
    [ReadOnly(RunMode.Any)]
    public int currentNumberOfFoodSources;
#endif

    [Header("Pheromone Grid")]

    [ReadOnly]
    public uint pheromoneGridWidth = 100;

    [ReadOnly]
    public uint pheromoneGridHeight = 100;

    [ReadOnly]
    public float pheromoneGridSquareSize = 1.0f;

    private List<Predator> predators = new List<Predator>();
    private List<Nest> nests = new List<Nest>();
    private List<Food> foodSources = new List<Food>();

    private Pheromone[,] pheromoneGrid;     // initialized on Awake()

    private float nestSpawnDeltaTime;
    private float foodSpawnDeltaTime;

    private bool started = false;

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(startDelay);
        predators.Add(CreatePredator());
        started = true;
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

    private Food CreateFood()
    {
        float spawnDistance = Random.Range(minFoodSpawnRadius, maxFoodSpawnRadius);
        Vector2 point = (Random.insideUnitCircle.normalized) * spawnDistance;
        Vector3 spawnPoint = transform.position + new Vector3(point.x, 0, point.y);

        Food food = Instantiate<Food>(foodPrefab);
        food.transform.position = spawnPoint;
        return food;
    }

    #region Unity Events

    void Awake()
    {
        if  (GameMode.instance == null)
        {
            GameMode.instance = this;

            UnityEngine.Random.seed = System.Environment.TickCount;
            pheromoneGrid = new Pheromone[pheromoneGridWidth, pheromoneGridHeight];
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
        if (started)
        {
            SpawnNest();
            SpawnFood();
            
        }
    }

    #endregion

    private void SpawnNest()
    {
        if (nestSpawnEnabled)
        {
            nestSpawnDeltaTime += Time.deltaTime;
            float spawnPeriod = 1f / nestSpawnRate;
            while (nests.Count < maxNumberOfNests && nestSpawnDeltaTime >= spawnPeriod)
            {
                Nest nest = CreateNest();
                nests.Add(nest);

                #if UNITY_EDITOR
                currentNumberOfNests = nests.Count;
                #endif

                nestSpawnDeltaTime -= spawnPeriod;
            }
        }
        else
        {
            nestSpawnDeltaTime = 0;
        }
    }

    private void SpawnFood()
    {
        if (foodSpawnEnabled)
        {
            foodSpawnDeltaTime += Time.deltaTime;
            float spawnPeriod = 1f / foodSpawnRate;
            while (foodSources.Count < maxNumberOfFoodSources && foodSpawnDeltaTime >= spawnPeriod)
            {
                Food food = CreateFood();
                foodSources.Add(food);

                #if UNITY_EDITOR
                currentNumberOfFoodSources = foodSources.Count;
                #endif

                foodSpawnDeltaTime -= spawnPeriod;
            }
        }
        else
        {
            foodSpawnDeltaTime = 0;
        }
    }

}
