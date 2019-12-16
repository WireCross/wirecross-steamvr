using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ButtonSequence : PuzzleInput
{
    public List<int> correct = new List<int>
    {
        0, 1, 2
    };

    public List<GameObject> buttons;

    private List<int> current = new List<int>();
    private GameObject indicator;
    private Color initialColor;
    private AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        indicator = gameObject.transform.Find("Indicator").gameObject;
        initialColor = indicator.GetComponent<Renderer>().sharedMaterial.GetColor("_Color");
        source = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (current.Count == correct.Count)
        {
            if (current.SequenceEqual(correct))
            {
                indicator.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", new Color(0f, 1f, 0f));
                source.Play(0);
                OnCorrectSequence.Invoke();
            }
            else
            {
                indicator.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", new Color(1f, 0f, 0f));
                StartCoroutine(ResetIndicator());
            }

            current.Clear();
        }
    }

    public void ButtonPressed(int id)
    {
        if (!current.Contains(id))
        {
            current.Add(id);
            Debug.Log(id + " pressed");
            Debug.Log(string.Join(", ", current));
        }
    }

    IEnumerator ResetIndicator()
    {
        yield return new WaitForSeconds(3);
        indicator.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", initialColor);
    }

    public override void SetupAnswers(Color[] colors, Mesh[] meshes, int[] correctSequence)
    {
        Debug.Log("Setup " + colors.Length);
        for(int i=0; i < colors.Length; i++)
        {
            GameObject obj = buttons[i];
            obj.GetComponent<MeshFilter>().mesh = meshes[i];

            Material mat = new Material(buttons[i].GetComponent<Renderer>().material);
            mat.SetColor("_MKGlowColor", colors[i]);
            mat.SetColor("_Tint", colors[i]);
            mat.SetColor("_Color", colors[i]);

            obj.GetComponent<Renderer>().material = mat;
        }

        correct = new List<int>(correctSequence);
        Debug.Log("Setup: " + string.Join(", ", correct));
    }

}
