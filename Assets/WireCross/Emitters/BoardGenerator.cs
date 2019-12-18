using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardGenerator : Emitter
{
    // allow selection for base mat and glow sprite (MK)
    public Texture2D glowSprite;
    public Material baseMaterial;

    // define bondaries for generation
    private static readonly float TOP_RIGHT_X = -5.75f;
    private static readonly float TOP_RIGHT_Y = 3.75f;
    private static readonly float BOTTOM_LEFT_X = 5.5f;
    private static readonly float BOTTOM_LEFT_Y = -3.25f;

    // how many to generate (25 is the sweet spot)
    private static readonly int GENERATE_COUNT = 25;

    private static readonly Dictionary<string, string> CONFUSING_PAIRS = new Dictionary<string, string>
    {
        { "2", "5" },
        { "J", "L" },
        { "M", "N" },
        { "U", "V" },
        { "S", "Z" },

        // same as above but just reversed
        { "5", "2" },
        { "L", "J" },
        { "N", "M" },
        { "V", "U" },
        { "Z", "S" }
    };

    public enum Shape
    {
        SQUARE,
        TRIANGLE,
        CIRCLE
    }

    private Font font;
    private Material mat;
    private System.Random random = new System.Random();
    private List<GameObject> objects = new List<GameObject>();

    private List<Shape> selected = new List<Shape>();
    // current idx for what shape we're using
    private int idx = 0;

    void Start()
    {
        // select 3 random shapes
        Array vals = Enum.GetValues(typeof(Shape));
        for (int i = 0; i < 3; i++)
        {
            selected.Add((Shape)vals.GetValue(random.Next(vals.Length)));
        }

        // initialize based on the selected shapes
        Init();
    }

    // Start is called before the first frame update
    void Init()
    {
        // load up our font and mats, then check to see if they're actually loaded
        font = Resources.Load<Font>("Font/8BITWONDER");
        mat = Resources.Load<Material>("Font/FontMat");
        if (font == null)
            throw new Exception("bad font");
        if (mat == null)
            throw new Exception("bad mat");

        if (idx >= selected.Count)
            return;

        // get a random pair of potentially confusing characters
        KeyValuePair<string, string> chosen = CONFUSING_PAIRS.ElementAt(random.Next(0, CONFUSING_PAIRS.Count));

        // generate our answer first, and then add it to objects
        foreach (GameObject obj in GeneratePatternPosition(chosen.Value, selected[idx], mat, font)) {
            Vector3 off = obj.transform.localPosition;

            obj.transform.parent = gameObject.transform;
            obj.transform.localPosition = off;
            obj.transform.rotation = new Quaternion(0, 0, 0, 0);

            objects.Add(obj);
        }

        // now 'procedurally' generate our distractions through randomization and bounds checking
        for (int i=0; i < GENERATE_COUNT; i++)
        {
            GameObject text = Generate3DText(chosen.Key, mat, font);

            int attempts = 0;
            bool done = false;

            // try to generate only 25 times, otherwise we run into an infinite loop
            while (attempts++ < 25)
            {
                text.transform.parent = gameObject.transform;
                text.transform.localPosition = GetRandomValidPosition();
                text.transform.rotation = new Quaternion(0, 0, 0, 0);

                // this took WAAAAY too long to figure out...
                if (IsSpotFree(text))
                {
                    objects.Add(text);
                    done = true;
                    break;
                }
            }

            // if we're not done, then destroy the text since it's invalid
            if (!done)
            {
                Destroy(text);
            }
        }

        // notify that we're setup so that we can pass stuff over to PuzzleInput
        NotifySetup();
        Debug.Log("NotifySetup");
    }

    void CleanUp()
    {
        // delete everything except "Board"
        for(int cidx = 0; cidx < gameObject.transform.childCount; cidx++)
        {
            Transform obj = gameObject.transform.GetChild(cidx);
            if (obj.gameObject.name.Equals("Board"))
            {
                continue;
            }
            Destroy(obj.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool IsSpotFree(GameObject obj)
    {
        // check if Renderer bounds collides with another; used to make sure we don't overlap
        foreach(GameObject other in objects)
        {
            if (obj.GetComponent<Renderer>().bounds.Intersects(other.GetComponent<Renderer>().bounds))
            {
                return false;
            }
        }
        return true;
    }

    public GameObject Generate3DText(string text, Material mat, Font font, bool answer = false)
    {
        GameObject textObj = new GameObject();

        // setup our text mesh with some custom properties so that we have
        // middle-center anchor points and not-ugly font rendering
        TextMesh mesh = textObj.AddComponent<TextMesh>();
        mesh.text = text;
        mesh.anchor = TextAnchor.MiddleCenter;
        mesh.characterSize = 0.01f;
        mesh.fontSize = 500;
        mesh.alignment = TextAlignment.Center;
        mesh.font = font;

        Renderer renderer = textObj.GetComponent<Renderer>();
        if (renderer == null)
            renderer = textObj.AddComponent<MeshRenderer>();

        // if we're not an answer, use the primary color, otherwise use its opposite
        Color glowColor = answer ? ColorDifficulty.GetTertiaryColor(1f) : ColorDifficulty.GetPrimaryColor(1f);
        Material glowMat = new Material(baseMaterial);
        glowMat.SetColor("_Color", ColorDifficulty.GetPrimaryColor(0.25f));
        // set MKGlow to make the letters glow
        glowMat.SetColor("_MKGlowColor", glowColor);
        glowMat.SetFloat("_MKGlowPower", 2.5f);
        glowMat.SetTexture("_MKGlowTex", glowSprite);

        // pass our original mat and glowing material to the letter
        // (mat here is a custom material to prevent text from showing through everything)
        renderer.materials = new Material[] { glowMat, mat };

        BoxCollider collider = textObj.AddComponent<BoxCollider>();
        collider.size = new Vector3(0.5f, 0.5f, 0);
        collider.center = new Vector3(-0.5f, -0.5f, 0);

        textObj.name = answer ? "Answer" : "Distraction";
        return textObj;
    }

    public float GetRandomNumber(float minimum, float maximum)
    {
        return (float)(random.NextDouble() * (maximum - minimum) + minimum);
    }

    public Vector3 GetRandomValidPosition(float topOffset = 0, float bottomOffset = 0)
    {
        return new Vector3(GetRandomNumber(TOP_RIGHT_X + topOffset, BOTTOM_LEFT_X - bottomOffset), 
                           GetRandomNumber(TOP_RIGHT_Y - topOffset, BOTTOM_LEFT_Y + bottomOffset), 
                           0.25f);
    }

    public GameObject[] GeneratePatternPosition(string text, Shape shape, Material mat, Font font)
    {
        // 1 to 3
        List<GameObject> obj = new List<GameObject>();
        float scale = GetRandomNumber(1, 3);
        Vector3 center = GetRandomValidPosition(scale, scale);

        if (shape == Shape.TRIANGLE)
        {
            GameObject bottomLeft = Generate3DText(text, mat, font, true);
            bottomLeft.transform.localPosition = center + (new Vector3(scale, 0, 0));

            GameObject topMiddle = Generate3DText(text, mat, font, true);
            topMiddle.transform.localPosition = center + (new Vector3(0, scale, 0));

            GameObject bottomRight = Generate3DText(text, mat, font, true);
            bottomRight.transform.localPosition = center + (new Vector3(-scale, 0, 0));

            obj.Add(bottomLeft);
            obj.Add(topMiddle);
            obj.Add(bottomRight);
        }
        else if(shape == Shape.SQUARE)
        {
            GameObject bottomLeft = Generate3DText(text, mat, font, true);
            bottomLeft.transform.localPosition = center + (new Vector3(scale, 0, 0));

            GameObject topLeft = Generate3DText(text, mat, font, true);
            topLeft.transform.localPosition = center + (new Vector3(scale, scale, 0));

            GameObject topRight = Generate3DText(text, mat, font, true);
            topRight.transform.localPosition = center + (new Vector3(-scale, scale, 0));

            GameObject bottomRight = Generate3DText(text, mat, font, true);
            bottomRight.transform.localPosition = center + (new Vector3(-scale, 0, 0));

            obj.Add(bottomLeft);
            obj.Add(topLeft);
            obj.Add(topRight);
            obj.Add(bottomRight);
        }
        else if(shape == Shape.CIRCLE)
        {
            int items = 6;
            for(int i=0; i < items; i++)
            {
                float x = scale * Mathf.Cos(2 * Mathf.PI * i / items);
                float y = scale * Mathf.Sin(2 * Mathf.PI * i / items);

                GameObject point = Generate3DText(text, mat, font, true);
                point.transform.localPosition = center + (new Vector3(x, y, 0));
                obj.Add(point);
            }
        }

        return obj.ToArray();
    }


    public override void Next()
    {
        CleanUp();
        Init();
        idx++;

        NotifySetup();
    }

    public override void NotifySetup()
    {
        Debug.Log("Selected " + string.Join(", ", selected));
        List<Color> colors = new List<Color>
        {
            ColorDifficulty.GetPrimaryColor(1f),
            ColorDifficulty.GetSecondaryColor(1f),
            ColorDifficulty.GetTertiaryColor(1f)
        };

        // add our answer's mesh to a list
        List<Mesh> meshes = new List<Mesh>();
        Mesh mesh = GetMeshFor(selected[idx]);
        meshes.Add(mesh);

        // now add any of the selections that wasn't ours
        foreach(Shape shape in GetNot(selected[idx]))
        {
            meshes.Add(GetMeshFor(shape));
        }

        // send up our colors, meshes, and correct answer back to the input
        SetupInput.input.SetupAnswers(colors.ToArray(), meshes.ToArray(), new int[] { meshes.IndexOf(mesh) });
    }

    // some generic helper functions to get meshes, shapes, etc.

    public Mesh GetMeshFor(Shape shape)
    {
        switch (shape)
        {
            case Shape.SQUARE:
                return PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Cube);
            case Shape.CIRCLE:
                return PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Sphere);
            case Shape.TRIANGLE:
                return PrimitiveHelper.GetTriangularThing();
        }
        return null;
    }

    public Shape[] GetNot(Shape shape)
    {
        switch (shape)
        {
            case Shape.SQUARE:
                return new Shape[] { Shape.CIRCLE, Shape.TRIANGLE };
            case Shape.CIRCLE:
                return new Shape[] { Shape.SQUARE, Shape.TRIANGLE };
            case Shape.TRIANGLE:
                return new Shape[] { Shape.CIRCLE, Shape.SQUARE};
        }

        return null;
    }

    public static List<T> Randomize<T>(List<T> list)
    {
        List<T> randomizedList = new List<T>();
        System.Random rnd = new System.Random();
        while (list.Count > 0)
        {
            int index = rnd.Next(0, list.Count);
            randomizedList.Add(list[index]);
            list.RemoveAt(index);
        }
        return randomizedList;
    }

}
