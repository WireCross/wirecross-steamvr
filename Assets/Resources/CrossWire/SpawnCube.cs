using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class SpawnCube : MonoBehaviour
{
    private static Material glowMaterial;
    private static Texture glowSprite;
    private static int id = 1;

    private Random rng = new Random();

    // Start is called before the first frame update
    void Start()
    {
        if(glowMaterial == null)
        {
            glowMaterial = (Material)Resources.Load<Material>("CrossWire/GlowEffect");
        }

        if(glowSprite == null)
        {
            glowSprite = (Texture)Resources.Load<Texture>("CrossWire/GlowSprite");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.D))
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(rng.Next(-5, 5), 15, rng.Next(-5, 5));
            cube.AddComponent<Rigidbody>();
            cube.name = (rng.Next(2) + 1).ToString();

            Renderer renderer = cube.GetComponent<Renderer>();

            Material tmp = renderer.sharedMaterial = new Material(Shader.Find("MK/Glow/Selective/Sprites/Default"));
            tmp.SetColor("_Color", GetRandomColor());
            tmp.SetColor("_MKGlowColor", GetRandomColor());
            tmp.SetFloat("_MKGlowPower", GetRandomNumber(0, 0.2f));
            tmp.SetTexture("_MKGlowTex", glowSprite);

            Destroy(cube, 3);
        }
    }

    private Color GetRandomColor()
    {
        return new Color(GetRandomNumber(0, 0.3f), GetRandomNumber(0, 0.3f), GetRandomNumber(0, 0.3f));
    }

    private float GetRandomNumber(float low, float high)
    {
        return (float)(rng.NextDouble() * (high - low) + low);
    }
}
