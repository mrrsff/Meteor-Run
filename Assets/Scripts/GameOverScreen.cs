using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    GameObject gameOverScreen;
    TextMeshProUGUI scoreText;
    float score;
    void Start()
    {
        PlayerController.OnPlayerDeath += Open;
        PlayerController.OnScoreChange += UpdateScore;
        gameOverScreen = transform.GetChild(transform.childCount - 1).gameObject;
        gameOverScreen.SetActive(false);
        scoreText = gameOverScreen.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
    }
    void UpdateScore(float score)
    {
        this.score = score;
    }
    public void Restart()
    {
        PlayerController.OnPlayerDeath -= Open;
        PlayerController.OnScoreChange -= UpdateScore;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    void Open()
    {
        gameOverScreen.SetActive(true);
        
        gameOverScreen = transform.GetChild(transform.childCount - 1).gameObject;
        foreach (var tmp in gameOverScreen.transform.GetComponentsInChildren<TextMeshProUGUI>())
        {
            if(tmp.name == "Score") scoreText = tmp;
        }
        Debug.Log(scoreText == null);
        gameOverScreen.transform.localScale = Vector3.zero;

        scoreText.text = "Score: " + score.ToString("0");
        gameOverScreen.transform.DOScale(1, 1);
    }
}
