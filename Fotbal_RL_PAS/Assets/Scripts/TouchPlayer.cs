using System;
using System.Collections.Generic;
using UnityEngine;

public class TouchPlayer : MonoBehaviour
{
    private Dictionary<PlayerAgent, float> lastTouchTime = new Dictionary<PlayerAgent, float>();
    private float cooldownDuration = 0.5f;

    private void OnCollisionEnter(Collision collision)
    {
        var player = collision.gameObject.GetComponent<PlayerAgent>();
        if (player != null)
        {
            float currentTime = Time.time;

            if (!lastTouchTime.ContainsKey(player) || currentTime - lastTouchTime[player] >= cooldownDuration)
            {
                Rigidbody ballRb = GetComponent<Rigidbody>();
                Vector3 ballVelocity = ballRb.linearVelocity;
                Vector3 toGoal = (player.adversaryGoal.position - transform.position).normalized;
                float progress = Vector3.Dot(ballVelocity, toGoal);

                player.AddReward(progress * 0.05f);
                player.AddReward(0.025f);

                Vector3 kickDirection = (transform.position - player.transform.position).normalized;
                float kickForce = 5f;
                ballRb.AddForce(kickDirection * kickForce, ForceMode.VelocityChange);

                GameManager.lastTouchedAgent = player;
                lastTouchTime[player] = currentTime;
            }
        }
    }
}
