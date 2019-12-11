using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(Interactable))]
public class PlayButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnButtonDown(Hand hand)
    {
        SceneManager.LoadScene("Debug Room");
    }
}
