using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject ball;
    public Transform ballStartPosition;
    public List<PlayerAgent> redTeamPlayers;
    public List<PlayerAgent> blueTeamPlayers;
    public Transform[] redTeamStartPositions;
    public Transform[] blueTeamStartPositions;

    public void ResetGame()
    { 
        ball.transform.position = ballStartPosition.position;
        ball.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    public void OnGoalScored(string goalTag)
    {
        if (goalTag == "BlueGoalCollider")
        {
            foreach (var player in redTeamPlayers)
            {
                player.AddReward(1.0f);
                player.EndEpisode();
            }
            foreach (var player in blueTeamPlayers)
            {
                player.AddReward(-1.0f);
                player.EndEpisode();
            }
        }
        else if (goalTag == "RedGoalCollider")
        {
            foreach (var player in blueTeamPlayers)
            {
                player.AddReward(1.0f);
                player.EndEpisode();
            }
            foreach (var player in redTeamPlayers)
            {
                player.AddReward(-1.0f);
                player.EndEpisode();
            }
        }

        ResetGame();
    }
}
