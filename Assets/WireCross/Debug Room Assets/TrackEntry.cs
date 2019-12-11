using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class TrackEntry : MonoBehaviour
{
    public Texture2D sprite;
    public GameObject sourceObject;
    
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

    private List<GameObject> correct = new List<GameObject>();
    private List<GameObject> lastObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        // populate possible meshes
        if(meshes == null)
        {
            meshes = new List<Mesh>();
            meshes.Add(PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Capsule));
            meshes.Add(PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Cube));
            meshes.Add(PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Sphere));
            meshes.Add(GetTriangularThing());
        }


        List<Color> possibleColors = new List<Color>(colors);
        List<Mesh> possibleMeshes = new List<Mesh>(meshes);
        for (int i = 0; i < 3; i++)
        {
            GameObject prefab = Instantiate(sourceObject, new Vector3(transform.position.x + (-1 + i), transform.position.y, transform.position.z + 3), Quaternion.identity);
            prefab.name = "" + i;

            Color selected = ExtractRandomElement(possibleColors);
            GameObject cube = prefab.transform.GetChild(0).gameObject;

            Renderer cubeRenderer = cube.GetComponent<Renderer>();
            Material mat = cubeRenderer.sharedMaterial = new Material(Shader.Find("MK/Glow/Selective/Sprites/Default"));
            mat.SetColor("_Color", selected);
            mat.SetColor("_MKGlowColor", selected);
            mat.SetFloat("_MKGlowPower", 0.5f);
            mat.SetTexture("_MKGlowTex", sprite);

            Mesh newMesh = ExtractRandomElement(meshes);
            cube.GetComponent<MeshFilter>().sharedMesh = newMesh;

            correct.Add(prefab);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (lastObjects.Count == correct.Count)
        {
            if (lastObjects.SequenceEqual(correct))
            {
                gameObject.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", new Color(0f, 1f, 0f));
                return;
            }
            else
            {
                if (!hasEjectedRecently)
                {
                    foreach (GameObject obj in lastObjects)
                    {
                        float dx = (float)(random.NextDouble() * 2 - 1) * 5f;
                        float dz = (float)(random.NextDouble() * 2 - 1) * 5f;
                        obj.GetComponent<Rigidbody>().AddForce(new Vector3(dx, 10, dz), ForceMode.Impulse);
                    }
                    hasEjectedRecently = true;
                    StartCoroutine(ResetEjectedState());
                }
            }
        }

        gameObject.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", new Color(1f, 0f, 0f));
    }

    T ExtractRandomElement<T>(List<T> list) {
        int idx = random.Next(0, list.Count);
        T selected = list[idx];
        list.RemoveAt(idx);
        return selected;
    }

    public void OnTriggerEnter(Collider other)
    {
        lastObjects.Add(other.transform.root.gameObject);
    }

    public void OnTriggerExit(Collider other)
    {
        lastObjects.Remove(other.transform.root.gameObject);
    }

    IEnumerator ResetEjectedState()
    {
        yield return new WaitForSeconds(0.5f);
        hasEjectedRecently = false;
    }

    Mesh GetTriangularThing()
    {
        Mesh mesh = new Mesh();

        Vector3 p0 = new Vector3(0, 0, 0);
        Vector3 p1 = new Vector3(1, 0, 0);
        Vector3 p2 = new Vector3(0.5f, 0, Mathf.Sqrt(0.75f));
        Vector3 p3 = new Vector3(0.5f, Mathf.Sqrt(0.75f), Mathf.Sqrt(0.75f) / 3);

        mesh.vertices = new Vector3[]{
                p0,p1,p2,
                p0,p2,p3,
                p2,p1,p3,
                p0,p3,p1
        };
        mesh.triangles = new int[]{
                0,1,2,
                3,4,5,
                6,7,8,
                9,10,11
        };

        Vector2 uv0 = new Vector2(0, 0);
        Vector2 uv1 = new Vector2(1, 0);
        Vector2 uv2 = new Vector2(0.5f, 1);

        mesh.uv = new Vector2[]{
                uv0,uv1,uv2,
                uv0,uv1,uv2,
                uv0,uv1,uv2,
                uv0,uv1,uv2
        };
        
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();
        return mesh;
    }

}
