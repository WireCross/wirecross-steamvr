using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
 * Fufill procedural generation of puzzles
 */
public class SetupInput : MonoBehaviour
{
    public static GameObject puzzle;
    public static PuzzleInput input;

    public GameObject[] possible;
    public UnityEvent Trigger;

    private System.Random random = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        // check to see if we have any to begin with
        if (possible.Length != 0)
        {
            // try to instantiate our input prefab
            puzzle = Instantiate(possible[random.Next(0, possible.Length)], gameObject.transform.position, new Quaternion(0, 0, 0, 0), gameObject.transform);

            // find the input and set our static variable to it
            // (statics are terrible, but it's the easiest thing we can do with the existing codebase)
            input = FindPuzzleInput(puzzle);
            if (input != null)
                Debug.Log("Found PuzzleInput @ " + input);
            // setup its trigger
            input.OnCorrectSequence = Trigger;
        }
    }

    PuzzleInput FindPuzzleInput(GameObject obj)
    {
        // try to find a puzzle input script in the current obj
        PuzzleInput attempt = obj.GetComponent<PuzzleInput>();
        if (attempt != null)
            return attempt;

        // didn't find one yet, try all of its children until we find one
        for (int i=0; i < obj.transform.childCount; i++)
        {
            PuzzleInput input = FindPuzzleInput(obj.transform.GetChild(i).gameObject);
            if (input != null)
                return input;
        }
        return null;
    }

}
