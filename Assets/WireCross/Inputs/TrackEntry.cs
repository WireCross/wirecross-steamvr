using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.Events;

public class TrackEntry : PuzzleInput
{
    public Texture2D sprite;

    private System.Random random = new System.Random();
    private bool hasEjectedRecently = false;

    private List<Mesh> meshes = null;

    private List<Color> colors = new List<Color> {
        new Color(0, 0, 1),
        new Color(0, 1, 0),
        new Color(0, 1, 1),
        new Color(1, 0, 0),
        new Color(1, 0, 1),
        new Color(1, 1, 0),
        new Color(1, 1, 1)
    };

    private List<int> correct = new List<int>
    {
        0, 1, 2
    };

    private GameObject choices;
    private List<GameObject> lastObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    void Setup()
    {
        // populate possible meshes
        if (meshes == null)
        {
            meshes = new List<Mesh>();
            meshes.Add(PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Cube));
            meshes.Add(PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Sphere));
            meshes.Add(PrimitiveHelper.GetTriangularThing());
        }

        // load up our cube that we can throw around and stuff
        GameObject sourceObject = Resources.Load("InteractableCube") as GameObject;
        List<Color> possibleColors = new List<Color>(colors);
        List<Mesh> possibleMeshes = new List<Mesh>(meshes);

        // generate a parent object for our cubes
        choices = new GameObject();
        choices.name = "Choices";

        for (int i = 0; i < 3; i++)
        {
            // instantiate our prefab objects in a row
            GameObject prefab = Instantiate(sourceObject,
                                            new Vector3(
                                                transform.position.x - 3,
                                                transform.position.y,
                                                transform.position.z + (-1 + i)
                                            ), Quaternion.identity, choices.transform);
            prefab.name = "" + i; // name it i so we can track its entry into our trigger zone

            // select a random color and use it as our material and glowing color
            Color selected = ExtractRandomElement(possibleColors);
            GameObject cube = prefab.transform.GetChild(0).gameObject;

            // change glow and tint color to our selected color
            Renderer cubeRenderer = cube.GetComponent<Renderer>();
            Material mat = cubeRenderer.sharedMaterial = new Material(Shader.Find("MK/Glow/Selective/Sprites/Default"));
            mat.SetColor("_Color", selected);
            mat.SetColor("_MKGlowColor", selected);
            mat.SetFloat("_MKGlowPower", 0.5f);
            mat.SetTexture("_MKGlowTex", sprite);

            // extract a random mesh and use it on our cube
            Mesh newMesh = ExtractRandomElement(meshes);
            newMesh.RecalculateBounds();
            cube.GetComponent<MeshFilter>().sharedMesh = newMesh;

            // now emit a light source
            Light light = prefab.AddComponent<Light>();
            light.color = selected;
            light.renderMode = LightRenderMode.ForcePixel;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (lastObjects.Count == correct.Count)
        {
            // if we gor our right answer, set our indicator's color to green and trigger corret sequence
            if (InputMatches())
            {
                gameObject.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", new Color(0f, 1f, 0f));
                OnCorrectSequence.Invoke();
                return;
            }
            else
            {
                // check if we've throw them out already (prevents cubes from flying everywhere)
                if (!hasEjectedRecently)
                {
                    // for each object that was in here, apply a random force to it
                    foreach (GameObject obj in lastObjects)
                    {
                        float dx = (float)(random.NextDouble() * 2 - 1) * 5f;
                        float dz = (float)(random.NextDouble() * 2 - 1) * 5f;
                        obj.GetComponent<Rigidbody>().AddForce(new Vector3(dx, 10, dz), ForceMode.Impulse);
                    }

                    // set ejected recently and start a countdown to do it again if it happens
                    hasEjectedRecently = true;
                    StartCoroutine(ResetEjectedState());
                }
            }
        }

        // otherwise, our indicator is red
        gameObject.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", new Color(1f, 0f, 0f));
    }

    bool InputMatches()
    {
        for(int i=0; i < correct.Count; i++)
        {
            // try to check the last objects put in; we can get away with parsing the
            // name of the object since the only movable object are the cubes
            if(correct[i] != int.Parse(lastObjects[i].name))
            {
                return false;
            }
        }
        return true;
    }

    T ExtractRandomElement<T>(List<T> list) {
        int idx = random.Next(0, list.Count);
        T selected = list[idx];
        list.RemoveAt(idx);
        return selected;
    }

    // trigger functions to track entry and exit

    public void OnTriggerEnter(Collider other)
    {
        lastObjects.Add(other.transform.parent.gameObject);
        Debug.Log("Enter: " + string.Join(",", lastObjects));
    }

    public void OnTriggerExit(Collider other)
    {
        lastObjects.Remove(other.transform.parent.gameObject);
        Debug.Log("Exit: " + string.Join(",", lastObjects));
    }

    IEnumerator ResetEjectedState()
    {
        yield return new WaitForSeconds(0.5f);
        hasEjectedRecently = false;
    }

    public override void SetupAnswers(Color[] colors, Mesh[] meshes, int[] correctSequence)
    {
        Debug.Log("Setup");
        for (int i=0; i < colors.Length; i++)
        {
            try
            {
                // try to find our real cube (which is named HackForShadow since glowing stuff don't produce shadows)
                GameObject child = choices.transform.Find((i).ToString("0")).Find("HackForShadow").gameObject;
                child.GetComponent<MeshFilter>().sharedMesh = meshes[i];

                // set up our tint and glow color
                Material mat = new Material(child.GetComponent<Renderer>().material);
                mat.SetColor("_Tint", colors[i]);
                mat.SetColor("_MKGlowColor", colors[i]);

                child.GetComponent<Renderer>().material = mat;
                child.GetComponent<Light>().color = colors[i];
            } catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        correct = new List<int>(correctSequence);
        Debug.Log("Setup: " + string.Join(", ", correct));
    }

}
