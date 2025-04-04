using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject ball;
    public Transform ballStartPosition;

    // Liste pentru jucătorii din echipele RedTeam și BlueTeam
    public List<PlayerAgent> redTeamPlayers;
    public List<PlayerAgent> blueTeamPlayers;

    // Pozițiile inițiale ale jucătorilor pentru fiecare echipă
    public Transform[] redTeamStartPositions;
    public Transform[] blueTeamStartPositions;

    void Start()
    {
        //ResetGame();
    }

    // Resetează poziția mingii și a jucătorilor
    public void ResetGame()
    {
        Debug.Log("SE RESETEAZA!!!");

        // Repozitionează mingea în centru
        ball.transform.position = ballStartPosition.position;

        // Repozitionează jucătorii din RedTeam
        for (int i = 0; i < redTeamPlayers.Count; i++)
        {
            redTeamPlayers[i].transform.position = redTeamStartPositions[i].position;
            // Opțional: resetează și alte stări interne, cum ar fi rotația
        }

        // Repozitionează jucătorii din BlueTeam
        for (int i = 0; i < blueTeamPlayers.Count; i++)
        {
            blueTeamPlayers[i].transform.position = blueTeamStartPositions[i].position;
        }
    }

    // Metoda apelată la detectarea unui gol (folosind trigger-ul din collider-ul porții)
    public void OnGoalScored(string goalTag)
    {
        // Exemplu: dacă mingea a intrat în zona cu tag-ul BlueGoalCollider, atunci echipa RedTeam a marcat
        if (goalTag == "BlueGoalCollider")
        {
            // Acordă reward pozitiv echipei care a marcat și penalizează cealaltă echipă
            foreach (var player in redTeamPlayers)
            {
                player.AddReward(1.0f); // reward pozitiv pentru RedTeam
            }
            foreach (var player in blueTeamPlayers)
            {
                player.AddReward(-1.0f); // penalizare pentru BlueTeam
            }
        }
        // Similar pentru cazul când mingea intră în zona cu tag-ul RedGoalCollider (goal pentru BlueTeam)
        else if (goalTag == "RedGoalCollider")
        {
            foreach (var player in blueTeamPlayers)
            {
                player.AddReward(1.0f);
            }
            foreach (var player in redTeamPlayers)
            {
                player.AddReward(-1.0f);
            }
        }

        // După acordarea reward-urilor, resetează starea jocului
        ResetGame();
    }
}
