using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupPuzzle : MonoBehaviour
{
    public GameObject[] possible;

    private System.Random random = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        if (possible.Length != 0)
        {
            GameObject selected = possible[random.Next(0, possible.Length)];
            GameObject prefab = Instantiate(selected, gameObject.transform.position, new Quaternion(0,0,0,0), gameObject.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
