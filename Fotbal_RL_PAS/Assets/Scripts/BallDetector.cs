using UnityEngine;

public class GoalArea : MonoBehaviour
{
    public GameManager gameManager;

    private void OnCollisionEnter(Collision other)
    {
        
        if (other.gameObject.CompareTag("Ball"))
        {
            gameManager.OnGoalScored(gameObject.tag);
        }
    }
}
