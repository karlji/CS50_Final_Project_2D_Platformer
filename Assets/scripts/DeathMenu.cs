using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class DeathMenu : MonoBehaviour
{
    public Text score;
    public Text highScore;
    private void Start()
    {
        score = GameObject.Find("Score").GetComponent<Text>();
        score.text = "Score " + coin.coinCounter.ToString();
        highScore = GameObject.Find("HighScore").GetComponent<Text>();

        if (PlayerPrefs.GetInt("HighScore", 0) <= coin.coinCounter)
        {
            PlayerPrefs.SetInt("HighScore", coin.coinCounter);
            highScore.text = "New High Score ! : " + PlayerPrefs.GetInt("HighScore", 0).ToString();
        }
        else
        {
            highScore.text = "High Score: " + PlayerPrefs.GetInt("HighScore", 0).ToString();
        }
        


    }
    public void Restart()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void Quit()
    {
        Application.Quit();
    }
}
