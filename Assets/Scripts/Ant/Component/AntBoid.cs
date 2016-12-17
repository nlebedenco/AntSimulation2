using UnityEngine;

public class AntBoid : Ant
{
    public float chanceToTurn = 0.25f;
    public float maxTurningAngle = 90f;
    public float minDistanceToFriend = 4f;
    public float maxDistanceToFriend = 8f;

    protected AntAgentBoid agent;

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

        agent = new AntAgentBoid(character, chanceToTurn, maxTurningAngle, minDistanceToFriend, maxDistanceToFriend);
        character.agent = agent;
    }

    void Update()
    {
        agent.chanceToTurn = chanceToTurn;
        agent.maxTurningAngle = maxTurningAngle;
        agent.minDistanceToFriend = minDistanceToFriend;
        agent.maxDistanceToFriends = maxDistanceToFriend;
    }

    #endregion
}
