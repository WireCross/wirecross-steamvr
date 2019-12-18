using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class PuzzleInput : MonoBehaviour
{
    public UnityEvent OnCorrectSequence;

    public abstract void SetupAnswers(Color[] colors, Mesh[] meshes, int[] correctSequence);
}
