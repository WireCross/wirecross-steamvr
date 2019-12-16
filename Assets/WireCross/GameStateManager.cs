using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class GameStateManager : MonoBehaviour
{
    public PostProcessVolume volume;

    public Text timeText;
    public Text scoreField;
    
    private float startTime = 60.0f;
    private float timeLeft = 60.0f;
    private Bloom bloom;
    private ChromaticAberration chromatic;

    // Start is called before the first frame update
    void Start()
    {
        timeLeft = 60.0f - (GameStats.RoundsCompleted * 5);

        bloom = volume.profile.GetSetting<Bloom>();
        chromatic = volume.profile.GetSetting<ChromaticAberration>();
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= Time.deltaTime;

        timeText.text = "Time Left: " + (timeLeft).ToString("0");
        scoreField.text = "Num of Levels Completed: " + (GameStats.RoundsCompleted == 0 ? "None" : (GameStats.RoundsCompleted).ToString("0"));

        bloom.intensity.value = Mathf.Pow(1.1f, (startTime - timeLeft) - 20);
        chromatic.intensity.value = Mathf.Pow(1.5f, (startTime - timeLeft) - 40);

        if (timeLeft <= 0)
        {
            PlayerPrefs.SetInt("HighScore", GameStats.RoundsCompleted);
            SceneManager.LoadScene("Title");
        }
    }

    public void LevelComplete()
    {
        Debug.Log("Level completed");
        GameStats.RoundsCompleted++;
        ColorDifficulty.IncrementDiffAndReset();
        SceneManager.LoadScene("Game Room");
    }
}
