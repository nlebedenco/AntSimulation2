using UnityEngine;

using System;
using System.Collections;

public class AntCharacter : RigidbodyCharacter, IAntCharacter
{
    [Range(0, 10)]
    public float reactionTime = 0.1f;

    [ReadOnly]
    public string predatorLayerName = "Predator";

    [ReadOnly]
    public string antLayerName = "Ant";

    protected int antLayer;
    protected int predatorLayer;

    #region IAntCharacter 

    public Agent agent { get; set; }

    public event Action Updated;

    public event Action<IPredatorCharacter> PredatorFound;
    public event Action<IPredatorCharacter> PredatorLost;

    public event Action<IAntCharacter> AntFound;
    public event Action<IAntCharacter> AntLost;

    #endregion

    protected void OnUpdated()
    {
        var handler = Updated;
        if (handler != null)
            handler();
    }

    protected void OnPredatorFound(IPredatorCharacter predator)
    {
        var handler = PredatorFound;
        if (handler != null)
            handler(predator);
    }

    protected void OnPredatorLost(IPredatorCharacter predator)
    {
        var handler = PredatorLost;
        if (handler != null)
            handler(predator);
    }

    protected void OnAntFound(IAntCharacter ant)
    {
        var handler = AntFound;
        if (handler != null)
            handler(ant);
    }

    protected void OnAntLost(IAntCharacter ant)
    {
        var handler = AntLost;
        if (handler != null)
            handler(ant);
    }

    #region Unity Events 

    protected override void Awake()
    {
        base.Awake();

        predatorLayer = LayerMask.NameToLayer(predatorLayerName);
        antLayer = LayerMask.NameToLayer(antLayerName);
    }

    protected override void FixedUpdate()
    {
        OnUpdated();

        base.FixedUpdate();
    }

    private IEnumerator DelayedPredatorFound(IPredatorCharacter predator)
    {
        yield return new WaitForSeconds(reactionTime);
        OnPredatorFound(predator);
    }

    private IEnumerator DelayedPredatorLost(IPredatorCharacter predator)
    {
        yield return new WaitForSeconds(reactionTime);
        OnPredatorLost(predator);
    }

    private IEnumerator DelayedAntFound(IAntCharacter ant)
    {
        yield return new WaitForSeconds(reactionTime);
        OnAntFound(ant);
    }

    private IEnumerator DelayedAntLost(IAntCharacter ant)
    {
        yield return new WaitForSeconds(reactionTime);
        OnAntLost(ant);
    }

    private void ReactToPredatorFound(IPredatorCharacter predator)
    {
        if (reactionTime > 0)
            StartCoroutine(DelayedPredatorFound(predator));
        else
            OnPredatorFound(predator);
    }

    private void ReactToPredatorLost(IPredatorCharacter predator)
    {
        if (reactionTime > 0)
            StartCoroutine(DelayedPredatorLost(predator));
        else
            OnPredatorLost(predator);
    }

    private void ReactToAntFound(IAntCharacter ant)
    {
        if (reactionTime > 0)
            StartCoroutine(DelayedAntFound(ant));
        else
            OnAntFound(ant);
    }

    private void ReactToAntLost(IAntCharacter ant)
    {
        if (reactionTime > 0)
            StartCoroutine(DelayedAntLost(ant));
        else
            OnAntLost(ant);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == predatorLayer)
        {
            ReactToPredatorFound(other.gameObject.GetComponent<IPredatorCharacter>());
        }
        else if (other.gameObject.layer == antLayer)
        {
            ReactToAntFound(other.gameObject.GetComponent<IAntCharacter>());
        }

    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == predatorLayer)
        {
            ReactToPredatorLost(other.gameObject.GetComponent<IPredatorCharacter>());
        }
        else if (other.gameObject.layer == antLayer)
        {
            ReactToAntLost(other.gameObject.GetComponent<IAntCharacter>());
        }
    }

    #endregion
}
