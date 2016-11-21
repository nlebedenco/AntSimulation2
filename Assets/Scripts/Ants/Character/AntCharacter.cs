using UnityEngine;

using System;
using System.Collections;

public class AntCharacter : RigidbodyCharacter, IAntCharacter
{
    [Range(0, 10)]
    public float reactionTime = 0.1f;

    [ReadOnly]
    public string predatorLayerName = "Predator";

    protected int predatorLayer;

    public event Action Updated;
    public event Action<Predator> PredatorFound;
    public event Action<Predator> PredatorLost;

    protected void OnUpdated()
    {
        var handler = Updated;
        if (handler != null)
            handler();
    }

    protected void OnPredatorFound(Predator predator)
    {
        var handler = PredatorFound;
        if (handler != null)
            handler(predator);
    }

    protected void OnPredatorLost(Predator predator)
    {
        var handler = PredatorLost;
        if (handler != null)
            handler(predator);
    }

    #region Unity Events 

    protected override void Awake()
    {
        base.Awake();

        predatorLayer = LayerMask.NameToLayer(predatorLayerName);
    }

    protected override void FixedUpdate()
    {
        OnUpdated();

        base.FixedUpdate();
    }

    private IEnumerator DelayedPredatorFound(Predator predator)
    {
        yield return new WaitForSeconds(reactionTime);
        OnPredatorFound(predator);
    }

    private IEnumerator DelayedPredatorLost(Predator predator)
    {
        yield return new WaitForSeconds(reactionTime);
        OnPredatorLost(predator);
    }

    private void ReactToPredatorFound(Predator predator)
    {
        if (reactionTime > 0)
            StartCoroutine(DelayedPredatorFound(predator));
        else
            OnPredatorFound(predator);
    }

    private void ReactToPredatorLost(Predator predator)
    {
        if (reactionTime > 0)
            StartCoroutine(DelayedPredatorLost(predator));
        else
            OnPredatorLost(predator);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == predatorLayer)
        {
            ReactToPredatorFound(other.gameObject.GetComponent<Predator>());
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == predatorLayer)
        {
            ReactToPredatorLost(other.gameObject.GetComponent<Predator>());
        }
    }

    #endregion
}
