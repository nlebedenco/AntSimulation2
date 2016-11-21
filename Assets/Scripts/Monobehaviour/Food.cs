using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SphereCollider))]
public class Food : MonoBehaviour
{
    [ReadOnly]
    public Transform pillar;

    [ReadOnly]
    public float pilarSpeed = 40f;

    [ReadOnly]
    public float maxQuantity = 100f;

    public float currentQuantity = 100f;

    public float chunk = 0.01f;

    public float quality = 100f;

    #region Unity Events

    Vector3 pillarScale;

    void Awake()
    {
        pillarScale = pillar.localScale;
    }

    void Start()
    {
        StartCoroutine(ShowPilar());
    }

    void Update()
    {
        currentQuantity = Mathf.Clamp(currentQuantity, 0, maxQuantity);
        if (chunk < 0)
            chunk = 0;

        pillar.transform.localScale = Vector3.Lerp(pillar.transform.localScale, new Vector3(pillarScale.x, pillarScale.y * currentQuantity / maxQuantity, pillarScale.z), Time.deltaTime * 0.5f * pilarSpeed);
    }

    #endregion

    private IEnumerator ShowPilar()
    {
        Vector3 position = pillar.position;
        while (position.y < 0)
        {
            yield return new WaitForEndOfFrame();
            position += new Vector3(0, pilarSpeed * Time.deltaTime, 0);
            if (position.y > 0)
                position.y = 0;
            pillar.position = position;
        }
    }
}
