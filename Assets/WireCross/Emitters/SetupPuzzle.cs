using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            puzzle = Instantiate(possible[random.Next(0, possible.Length)], gameObject.transform.position, new Quaternion(0,0,0,0), gameObject.transform);
            Debug.Log("Instantiate");
            Emitter emitter = FindEmitter(puzzle);
            if (emitter == null)
                Debug.Log("no emitter?");
        }
    }

    Emitter FindEmitter(GameObject obj)
    {
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
