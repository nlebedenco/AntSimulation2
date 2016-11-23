using UnityEngine;

using System;
using System.Collections;

public class AntCharacter : RigidbodyCharacter, IAntCharacter
{
    [Header("Predator Detection")]

    [Range(0, 10)]
    public float reactionTime = 0.1f;

    [ReadOnly]
    public string predatorLayerName = "PredatorPresence";

    [Header("Ant Detection")]

    [ReadOnly]
    public string antLayerName = "Ant";

    [Header("Obstacle Detection")]

    [ReadOnly]
    public string obstacleLayerName = "Obstacle";

    protected int antLayer;
    protected int predatorLayer;
    protected int obstacleLayer;

    #region IAntCharacter 

    public Agent agent { get; set; }

    public event Action Updated;

    public event Action<IPredatorCharacter> PredatorFound;
    public event Action<IPredatorCharacter> PredatorLost;

    public event Action<IAntCharacter> AntFound;
    public event Action<IAntCharacter> AntLost;

    public event Action<Collider> Blocked;

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

    protected void OnBlocked(Collider obstacle)
    {
        var handler = Blocked;
        if (handler != null)
            handler(obstacle);
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

    #region Unity Events 

    protected override void Awake()
    {
        base.Awake();

        predatorLayer = LayerMask.NameToLayer(predatorLayerName);
        antLayer = LayerMask.NameToLayer(antLayerName);
        obstacleLayer = LayerMask.NameToLayer(obstacleLayerName);
    }

    protected override void FixedUpdate()
    {
        if (obstacleHitCount > 4)
        {
            obstacleHitCount = 0;
            OnBlocked(obstacle);
        }

        OnUpdated();

        base.FixedUpdate();
    }

    private Collider obstacle;
    private int obstacleHitCount;
    private float obstacleLastSeen;

    protected virtual void OnCollisionEnter(Collision collision)
    {
        var contact = collision.contacts[0];
        // When you using normalized vectors, Dot product will return 1 if the collision point is exactly in the front, 
        // 0 if it's in a 90° angle (left, right, up, down) and -1 if it's behind. 
        float dot = Vector3.Dot(transform.forward, (contact.point - transform.position).normalized);
        // Debug.LogFormat("{0} with {1}: {2}", contact.thisCollider.name, contact.otherCollider.name, dot);
        if (dot > 0.75f)
        {
            if (contact.otherCollider != obstacle)
            {
                obstacle = contact.otherCollider;
                obstacleHitCount = 0;
                obstacleLastSeen = Time.time;
            }
            else
            {
                if (Time.time - obstacleLastSeen < 2)
                    obstacleHitCount++;
                else
                    obstacleHitCount = 0;
                obstacleLastSeen = Time.time;
            }
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == predatorLayer)
        {
            ReactToPredatorFound(other.attachedRigidbody.GetComponent<IPredatorCharacter>());
        }
        else if (other.gameObject.layer == antLayer)
        {
            ReactToAntFound(other.attachedRigidbody.GetComponent<IAntCharacter>());
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == predatorLayer)
        {
            ReactToPredatorLost(other.attachedRigidbody.GetComponent<IPredatorCharacter>());
        }
        else if (other.gameObject.layer == antLayer)
        {
            ReactToAntLost(other.attachedRigidbody.GetComponent<IAntCharacter>());
        }
    }

    #endregion
}
