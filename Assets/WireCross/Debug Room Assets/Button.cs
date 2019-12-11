using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Button : MonoBehaviour
{
    public int id;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ButtonPressed(Hand hand)
    {
        Debug.Log(id + " was pressed");
    }
}
