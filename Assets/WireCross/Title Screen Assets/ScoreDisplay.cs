using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // display our persistant high score
        gameObject.GetComponent<TextMesh>().text = PlayerPrefs.GetInt("HighScore", 0).ToString("0");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
