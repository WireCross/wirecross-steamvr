using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(AudioSource))]
public class Button : MonoBehaviour
{
    public int id;

    private AudioSource source;
    private ButtonSequence seq;
    private bool down;

    // Start is called before the first frame update
    void Start()
    {
        // try to find our ButtonSequence to notify when we're pressed
        seq = FindSequenceParent();
        if (seq == null)
            throw new System.Exception("No button sequence found");
        source = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonDown()
    {
        // prevent multiple presses of the button at once
        // (mainly for 2D, where ButtonDown gets called constantly)
        if (!down)
        {
            down = true;
            source.Play(0);
        }
    }

    public void ButtonUp()
    {
        down = false;
    }

    public void ButtonPressed()
    {
        // notify ButtonSequence that we were pressed
        seq.ButtonPressed(id);
    }

    private ButtonSequence FindSequenceParent()
    {
        GameObject obj = gameObject;
        ButtonSequence seq = null;
        int depth = 15;

        // keep going up to find the parent ButtonSequence
        while (depth > 0 && obj != null)
        {
            if ((seq = obj.GetComponent<ButtonSequence>()) != null)
            {
                return seq;
            }
            obj = obj.transform.parent.gameObject;
            depth--;
        }

        return seq;
    }
}
