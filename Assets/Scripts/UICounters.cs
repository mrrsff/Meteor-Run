using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UICounters : MonoBehaviour
{
    public TextMeshProUGUI scoreText, timeText;
    float time;
    public void UpdateScore(float score)
    {
        scoreText.text = score.ToString("0");
    }
    private void Start() {
        time = 0;
        PlayerController.OnScoreChange += UpdateScore;
        StartCoroutine(TimeFlow());
        PlayerController.OnPlayerDeath += OnPlayerDeath;
    }
    void OnPlayerDeath()
    {
        PlayerController.OnPlayerDeath -= OnPlayerDeath;
        PlayerController.OnScoreChange -= UpdateScore;
        StopAllCoroutines();
    }
    public void UpdateTime(float time)
    {
        timeText.text = time.ToString("F2");
    }
    IEnumerator TimeFlow()
    {
        while (true)
        {
            var timeDelta = .01f;
            yield return new WaitForSeconds(timeDelta);
            time += timeDelta;
            UpdateTime(time);
        }
    }
}
