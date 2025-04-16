using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using TMPro;

public class GameManager : MonoBehaviour
{
	[Header("Game Objects")]
	[SerializeField] private GameObject ball;
	[SerializeField] private Transform ballStartPosition;
	[SerializeField] private List<PlayerAgent> redTeamPlayers;
	[SerializeField] private List<PlayerAgent> blueTeamPlayers;
	[SerializeField] private GameObject resetButton;

	[Header("UI Elements")]
	[SerializeField] private TextMeshProUGUI scoreText;
	[SerializeField] private TextMeshProUGUI timerText;

	[Header("Game Settings")]
	[SerializeField] private float gameDuration = 90f;

	private float gameTimer = 0f;
	private SimpleMultiAgentGroup redTeamGroup;
	private SimpleMultiAgentGroup blueTeamGroup;

	public static PlayerAgent lastTouchedAgent;

	private int redTeamScore = 0;
	private int blueTeamScore = 0;
	private bool gameOver = false;

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

		UpdateScoreUI();
		UpdateTimerUI();

		StartCoroutine(CheckBallStuck());
		StartCoroutine(GameTimerRoutine());
	}

	private IEnumerator CheckBallStuck()
	{
		while (true)
		{
			yield return new WaitForSeconds(checkInterval);

			float distance = Vector3.Distance(ball.transform.position, lastBallPosition);
			stuckTimer = (distance < 3f) ? stuckTimer + checkInterval : 0f;
			lastBallPosition = ball.transform.position;

			if (stuckTimer >= stuckThreshold)
			{
				redTeamGroup.AddGroupReward(-0.75f);
				blueTeamGroup.AddGroupReward(-0.75f);
				stuckTimer = 0f;
			}
		}
	}

	private IEnumerator GameTimerRoutine()
	{
		gameTimer = 0f;
		gameOver = false;

		if (resetButton != null)
			resetButton.SetActive(false);

		while (gameTimer < gameDuration)
		{
			UpdateTimerUI();
			yield return new WaitForSeconds(1f);
			gameTimer += 1f;
		}

		// Game over
		gameOver = true;

		if (resetButton != null)
			resetButton.SetActive(true);

		UpdateTimerUI(); // Will show FINAL
		redTeamGroup.EndGroupEpisode();
		blueTeamGroup.EndGroupEpisode();
		ResetGame();
	}

	public void OnGoalScored(string goalTag)
	{
		if (goalTag == "BlueGoalCollider")
		{
			redTeamGroup.AddGroupReward(1.0f);
			redTeamScore += 1;
			blueTeamGroup.AddGroupReward(-1.0f);

			if (lastTouchedAgent != null && blueTeamPlayers.Contains(lastTouchedAgent))
				lastTouchedAgent.AddReward(-1.0f);
		}
		else if (goalTag == "RedGoalCollider")
		{
			blueTeamGroup.AddGroupReward(1.0f);
			blueTeamScore += 1;
			redTeamGroup.AddGroupReward(-1.0f);

			if (lastTouchedAgent != null && redTeamPlayers.Contains(lastTouchedAgent))
				lastTouchedAgent.AddReward(-1.0f);
		}

		redTeamGroup.EndGroupEpisode();
		blueTeamGroup.EndGroupEpisode();

		UpdateScoreUI();
		ResetGame();
	}

	private void ResetGame()
	{
		ball.transform.position = ballStartPosition.position;
		Rigidbody rb = ball.GetComponent<Rigidbody>();
		rb.linearVelocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		lastTouchedAgent = null;
	}

	private void UpdateScoreUI()
	{
		if (scoreText != null)
			scoreText.text = $"{blueTeamScore}   -   {redTeamScore}";
	}

	private void UpdateTimerUI()
	{
		if (timerText != null)
		{
			if (gameTimer >= gameDuration)
			{
				timerText.text = "FINAL";
			}
			else
			{
				int secondsLeft = Mathf.RoundToInt(gameDuration - gameTimer);
				int minutes = secondsLeft / 60;
				int seconds = secondsLeft % 60;
				timerText.text = $"{minutes:00}:{seconds:00}";
			}
		}
	}

	public void ResetFullGame()
	{
		StopAllCoroutines();

		gameOver = false;
		gameTimer = 0f;
		redTeamScore = 0;
		blueTeamScore = 0;
		lastTouchedAgent = null;

		if (resetButton != null)
			resetButton.SetActive(false);

		UpdateScoreUI();
		UpdateTimerUI();
		ResetGame();

		StartCoroutine(CheckBallStuck());
		StartCoroutine(GameTimerRoutine());
	}

	public (int myScore, int opponentScore) GetTeamScore(int teamID)
	{
		return (teamID == 1)
			? (redTeamScore, blueTeamScore)
			: (blueTeamScore, redTeamScore);
	}

	public bool IsGameOver()
	{
		return gameOver;
	}
}
