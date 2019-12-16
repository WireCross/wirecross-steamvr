using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public float timeLeft = 10.0f;
    public Text timeText;
    public Text scoreField;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= Time.deltaTime;
        timeText.text = "Time Left: " + (timeLeft).ToString("0");
        scoreField.text = "Num of Levels Completed: " + (GameStats.RoundsCompleted == 0 ? "None" : (GameStats.RoundsCompleted).ToString("0"));

        if (timeLeft < 0)
        {
            timeText.text = "Game over";
        }
    }

    public void LevelComplete()
    {
        GameStats.RoundsCompleted++;
        ColorDifficulty.IncrementDiffAndReset();
        SceneManager.LoadScene("Game Room");
    }
}
