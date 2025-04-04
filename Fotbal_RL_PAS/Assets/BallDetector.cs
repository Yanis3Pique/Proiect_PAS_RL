using UnityEngine;

public class GoalArea : MonoBehaviour
{
    // Referință la GameManager pentru a notifica când se marchează un gol
    public GameManager gameManager;

    private void OnCollisionEnter(Collision other)
    {
        

        // Verificăm dacă obiectul care a intrat este mingea (asigură-te că mingea are tag-ul "Ball")
        if (other.gameObject.CompareTag("Ball"))
        {
            Debug.Log("SE ATINGE");
            Debug.Log(gameObject.tag);
            // Transmitem tag-ul obiectului curent (zona porții) către GameManager
            gameManager.OnGoalScored(gameObject.tag);
        }
    }
}
