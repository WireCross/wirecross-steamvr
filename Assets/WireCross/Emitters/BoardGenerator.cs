﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    public Texture2D glowSprite;
    public Material baseMaterial;

    private static readonly float TOP_RIGHT_X = -5.75f;
    private static readonly float TOP_RIGHT_Y = 3.75f;

    private static readonly float BOTTOM_LEFT_X = 5.5f;
    private static readonly float BOTTOM_LEFT_Y = -3.25f;

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

    private System.Random random = new System.Random();
    private GameObject board;
    private List<GameObject> objects = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        Font font = Resources.Load<Font>("Font/8BITWONDER");
        Material mat = Resources.Load<Material>("Font/FontMat");
        if (font == null)
            throw new Exception("bad font");
        if (mat == null)
            throw new Exception("bad mat");

        board = gameObject.transform.GetChild(0).gameObject;

        KeyValuePair<string, string> chosen = CONFUSING_PAIRS.ElementAt(random.Next(0, CONFUSING_PAIRS.Count));

        Array vals = Enum.GetValues(typeof(Shape));
        Shape shape = (Shape)vals.GetValue(random.Next(vals.Length));

        foreach (GameObject obj in GeneratePatternPosition(chosen.Value, shape, mat, font)) {
            Vector3 off = obj.transform.localPosition;

            obj.transform.parent = gameObject.transform;
            obj.transform.localPosition = off;
            obj.transform.rotation = new Quaternion(0, 0, 0, 0);
            objects.Add(obj);
        }

        for (int i=0; i < GENERATE_COUNT; i++)
        {
            GameObject text = Generate3DText(chosen.Key, mat, font);

            int attempts = 0;
            bool done = false;

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

            if (!done)
            {
                Destroy(text);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool IsSpotFree(GameObject obj)
    {
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

        Color glowColor = answer ? ColorDifficulty.GetTertiaryColor(1f) : ColorDifficulty.GetPrimaryColor(1f);
        Material glowMat = new Material(baseMaterial);
        glowMat.SetColor("_Color", ColorDifficulty.GetPrimaryColor(0.25f));
        glowMat.SetColor("_MKGlowColor", glowColor);
        glowMat.SetFloat("_MKGlowPower", 2.5f);
        glowMat.SetTexture("_MKGlowTex", glowSprite);

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
            float radius = 3;
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
}
