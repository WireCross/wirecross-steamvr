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
        // tick down our time remaining
        timeLeft -= Time.deltaTime;

        // display feedback to player
        timeText.text = "Time Left: " + (timeLeft).ToString("0");
        scoreField.text = "Num of Levels Completed: " + (GameStats.RoundsCompleted == 0 ? "None" : (GameStats.RoundsCompleted).ToString("0"));

        // intensify our bloom and chromatic abberation through simple power function, modeled to
        // scale our intensity severaly after the 40~ (of the 60) second mark
        bloom.intensity.value = Mathf.Pow(1.1f, (startTime - timeLeft) - 20);
        // abberation uses a different curve due to the fact that it's 0 to 1, unlike bloom
        chromatic.intensity.value = Mathf.Pow(1.5f, (startTime - timeLeft) - 40);

        if (timeLeft <= 0)
        {
            if (PlayerPrefs.GetInt("HighScore", 0) < GameStats.RoundsCompleted)
            {
                PlayerPrefs.SetInt("HighScore", GameStats.RoundsCompleted);
            }
            SceneManager.LoadScene("Title");
        }
    }

    public void LevelComplete()
    {
        // increment rounds completed for score purposes
        Debug.Log("Level completed");
        GameStats.RoundsCompleted++;
        // increase difficulty and get a new primary color
        ColorDifficulty.IncrementDiffAndReset();
        // reload the scene for a new puzzle
        SceneManager.LoadScene("Game Room");
    }
}
