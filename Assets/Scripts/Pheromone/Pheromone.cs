using UnityEngine;
using System.Collections;

public class Pheromone
{
    const float e = 2.7182818284590451f;

    public Pheromone(Vector3 position, float strength, float decay = 0.05f)
    {
        this.position = position;
        this.startTime = Time.time;
        this.strength = strength;
        this.decay = decay;
    }
    
    private Vector3 position;
    private float startTime;
    private float strength;
    private float decay;

    public Vector3 Position { get { return position; } }

    public float Strength
    {
        get
        {
            float deltaTime = Time.time - startTime;
            return strength / Mathf.Pow(e, decay * deltaTime);
        }
    }

    public float Decay { get { return decay; } }

    public void Reinforce(float strength)
    {
        if (strength > 0)
            this.strength += strength;
    }

    public float Attraction(float distance, float limit = 0)
    {
        return (limit > 0 && distance > limit) ? 0 
            : Strength / (distance * distance);
    }

    public float Attraction(Vector3 to, float limit = 0)
    {
        return Attraction(Vector3.Distance(to, Position), limit);
    }
}
