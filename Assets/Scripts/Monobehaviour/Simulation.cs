using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Simulation : MonoBehaviour
{
    #region Static 

    public static Simulation Instance { get; private set; }

    #endregion

    [ReadOnly]
    public Nest nestPrefab;

    [ReadOnly]
    public Food foodPrefab;

    [ReadOnly]
    public Predator predatorPrefab;

    [ReadOnly]
    public uint pheromoneGridWidth = 2000;

    [ReadOnly]
    public uint pheromoneGridHeight = 2000;

    [ReadOnly]
    public float pheromoneGridScale = 1.0f;

    public bool antsCanCommunicateFear = true;

    private List<Predator> predators = new List<Predator>();
    private List<Nest> nests = new List<Nest>();

    private Pheromone[,] pheromoneGrid;     // initialized on Awake()

    private bool _enableFear;
    public bool FearEnabled
    {
        get { return _enableFear; }
        private set
        {
            if (value != _enableFear)
            {
                Ant[] agents = GameObject.FindObjectsOfType<Ant>();
                for (int i = 0; i < agents.Length; ++i)
                    agents[i].isFearContagious = value;

                _enableFear = value;
            }
        }
    }

    #region Unity Events

    void Awake()
    {
        if  (Simulation.Instance == null)
        {
            Simulation.Instance = this;

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
        var predator = Instantiate<Predator>(predatorPrefab);
        Camera.main.GetComponent<CameraFollow>().target = predator.transform;
        predators.Add(predator);


    }

    void Update()
    {
        // Update fear communication for all agents
        FearEnabled = antsCanCommunicateFear;
    }

    #endregion

}
