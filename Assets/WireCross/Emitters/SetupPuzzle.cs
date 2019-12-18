using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Fufill procedural generation of puzzles
 */
public class SetupPuzzle : MonoBehaviour
{
    public static GameObject puzzle;
    public GameObject[] possible;

    private System.Random random = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        if (possible.Length > 0)
        {
            Debug.Log("SetupPuzzle");
            // generate a random puzzle here
            puzzle = Instantiate(possible[random.Next(0, possible.Length)], gameObject.transform.position, new Quaternion(0,0,0,0), gameObject.transform);
            Debug.Log("Instantiate");
            // try to find our emitter to make sure we have one (just for debugging)
            Emitter emitter = FindEmitter(puzzle);
            if (emitter == null)
                Debug.Log("no emitter?");
        }
    }

    Emitter FindEmitter(GameObject obj)
    {
        // try to find an emitter; GetComponent doesn't work with inheritance, so
        // we need to check _each_ type of emitter
        Emitter attempt1 = obj.GetComponent<BoardGenerator>();
        Emitter attempt2 = obj.GetComponent<VignetteEffect>();
        if (attempt1 != null)
            return attempt1;
        if (attempt2 != null)
            return attempt2;

        for (int i = 0; i < obj.transform.childCount; i++)
        {
            Emitter input = FindEmitter(obj.transform.GetChild(i).gameObject);
            if (input != null)
                return input;
        }
        return null;
    }

}
