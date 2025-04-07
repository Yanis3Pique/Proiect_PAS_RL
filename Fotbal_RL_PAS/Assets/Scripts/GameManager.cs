using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject ball;
    [SerializeField]
    private Transform ballStartPosition;
    [SerializeField]
    private List<PlayerAgent> redTeamPlayers;
    [SerializeField]
    private List<PlayerAgent> blueTeamPlayers;

    private SimpleMultiAgentGroup redTeamGroup;
    private SimpleMultiAgentGroup blueTeamGroup;

    public static PlayerAgent lastTouchedAgent;

    private int redTeamScore = 0;
    private int blueTeamScore = 0;

    private float stuckTimer = 0f;
    private const float checkInterval = 1f;
    private const float stuckThreshold = 2f;
    private Vector3 lastBallPosition;

    private void Start()
    {
        redTeamGroup = new SimpleMultiAgentGroup();
        blueTeamGroup = new SimpleMultiAgentGroup();

        foreach (var player in redTeamPlayers)
        {
            redTeamGroup.RegisterAgent(player);
        }

        foreach (var player in blueTeamPlayers)
        {
            blueTeamGroup.RegisterAgent(player);
        }

        lastBallPosition = ball.transform.position;
        StartCoroutine(CheckBallStuck());
    }

    private IEnumerator CheckBallStuck()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            float distance = Vector3.Distance(ball.transform.position, lastBallPosition);

            //Debug.Log("DISTANTA: " + distance);

            if (distance < 3f)
            {
                stuckTimer += checkInterval;
            }
            else
            {
                stuckTimer = 0f;
            }

            lastBallPosition = ball.transform.position;

            if (stuckTimer >= stuckThreshold)
            {
                redTeamGroup.AddGroupReward(-0.75f);
                blueTeamGroup.AddGroupReward(-0.75f);
                //Debug.Log("⚠️ Penalizare: mingea a stat blocată prea mult timp");
                stuckTimer = 0f;
            }
        }
    }

    public void ResetGame()
    { 
        ball.transform.position = ballStartPosition.position;
        ball.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        GameManager.lastTouchedAgent = null;
    }

    public void OnGoalScored(string goalTag)
    {
        if (goalTag == "BlueGoalCollider")
        {
            redTeamGroup.AddGroupReward(1.0f);
            redTeamScore += 1;

            blueTeamGroup.AddGroupReward(-1.0f);

            if (GameManager.lastTouchedAgent != null && blueTeamPlayers.Contains(GameManager.lastTouchedAgent))
            {
                GameManager.lastTouchedAgent.AddReward(-1.0f);
            }

            if (blueTeamScore - redTeamScore >= 2)
            {
                redTeamGroup.AddGroupReward(-0.5f);
            }
        }
        else if (goalTag == "RedGoalCollider")
        {
            blueTeamGroup.AddGroupReward(1.0f);
            blueTeamScore += 1;
            redTeamGroup.AddGroupReward(-1.0f);

            if (GameManager.lastTouchedAgent != null && redTeamPlayers.Contains(GameManager.lastTouchedAgent))
            {
                GameManager.lastTouchedAgent.AddReward(-1.0f);
            }

            if (redTeamScore - blueTeamScore >= 2)
            {
                blueTeamGroup.AddGroupReward(-0.5f);
            }
        }

        redTeamGroup.EndGroupEpisode();
        blueTeamGroup.EndGroupEpisode();


        ResetGame();
    }

    public (int myScore, int opponentScore) GetTeamScore(int teamID)
    {
        if (teamID == 1) 
            return (redTeamScore, blueTeamScore);
        else 
            return (blueTeamScore, redTeamScore);
    }
}
