using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        if (possible.Length != 0)
        {
            puzzle = Instantiate(possible[random.Next(0, possible.Length)], gameObject.transform.position, new Quaternion(0, 0, 0, 0), gameObject.transform);

            input = FindPuzzleInput(puzzle);
            if (input != null)
                Debug.Log("Found PuzzleInput @ " + input);
            input.OnCorrectSequence = Trigger;
        }
    }

    PuzzleInput FindPuzzleInput(GameObject obj)
    {
        PuzzleInput attempt = obj.GetComponent<PuzzleInput>();
        if (attempt != null)
            return attempt;

        for(int i=0; i < obj.transform.childCount; i++)
        {
            PuzzleInput input = FindPuzzleInput(obj.transform.GetChild(i).gameObject);
            if (input != null)
                return input;
        }
        return null;
    }

}
