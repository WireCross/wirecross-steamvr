using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrackEntry : MonoBehaviour
{
    private static int tracking = 0;
    private List<string> correct = new List<string> { "1", "2" };
    private List<string> lastObjects = new List<string>();
    private int maxSize = 30;
    
    // Start is called before the first frame update
    void Start(){}

    // Update is called once per frame
    void Update()
    {
        if (lastObjects.SequenceEqual(correct))
        {
            gameObject.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", new Color(0f, 1f, 0f));
        }
        else
        {
            gameObject.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", new Color(1f, 0f, 0f));
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        lastObjects.Add(other.name);
    }

    public void OnTriggerExit(Collider other)
    {
        lastObjects.Remove(other.name);
    }
}
