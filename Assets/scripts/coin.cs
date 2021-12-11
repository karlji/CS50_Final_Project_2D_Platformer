using UnityEngine;
using UnityEngine.UI;

public class coin : MonoBehaviour
{
    public static int coinCounter = 0;
    private GameObject coinUI;
    private Text coinUIText;
    private AudioSource audioData;

    // destroys coins on collision and adds to total counter
    private void Start()
    {
        coinUI = GameObject.Find("CoinUI");
        coinUIText = coinUI.GetComponent<Text>();
        audioData = GameObject.Find("coins").GetComponent<AudioSource>();
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            coinCounter++;
            coinUIText.text = coinCounter.ToString();
            audioData.Play();
            Destroy(gameObject);
        }

    }

}
