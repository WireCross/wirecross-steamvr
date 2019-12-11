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
        GameObject selected = possible[random.Next(0, possible.Length)];

        GameObject prefab = Instantiate(selected, gameObject.transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
