using UnityEngine;

public class coin : MonoBehaviour
{
    public static int coinCounter = 0;

    // destroys coins on collision and adds to total counter
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            coinCounter++;
            Destroy(gameObject);
        }

    }

}
