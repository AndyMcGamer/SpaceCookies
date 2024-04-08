using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highscoreText;

    private void Awake()
    {
        scoreText.text = $"Score: {PlayerPrefs.GetInt("Score")}";
        highscoreText.text = $"High Score: {PlayerPrefs.GetInt("HighScore")}";

    }

    public void RestartGame()
    {
        SceneController.instance.LoadScene("MainGame");
    }
}
