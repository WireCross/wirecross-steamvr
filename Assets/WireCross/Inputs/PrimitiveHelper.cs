using System.Collections.Generic;
using UnityEngine;

/**
 * Helper class to generate/steal Primitive meshes 
 */
public static class PrimitiveHelper
{
    private static Dictionary<PrimitiveType, Mesh> primitiveMeshes = new Dictionary<PrimitiveType, Mesh>();

    public static GameObject CreatePrimitive(PrimitiveType type, bool withCollider)
    {
        if (withCollider) { return GameObject.CreatePrimitive(type); }

        GameObject gameObject = new GameObject(type.ToString());
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = GetPrimitiveMesh(type);
        gameObject.AddComponent<MeshRenderer>();

        return gameObject;
    }

    public static Mesh GetPrimitiveMesh(PrimitiveType type)
    {
        if (!primitiveMeshes.ContainsKey(type))
        {
            CreatePrimitiveMesh(type);
        }

        return primitiveMeshes[type];
    }

    private static Mesh CreatePrimitiveMesh(PrimitiveType type)
    {
        GameObject gameObject = GameObject.CreatePrimitive(type);
        Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        Object.Destroy(gameObject);

        primitiveMeshes[type] = mesh;
        return mesh;
    }
    
    public static Mesh GetTriangularThing()
    {
        Mesh mesh = new Mesh();

        mesh.vertices = new Vector3[]{
                new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(0, -0.5f, -0.5f), new Vector3(0, 0.5f, 0),
        };

        mesh.triangles = new int[]{
            1, 2, 3,
            0, 1, 3,
            0, 2, 3,
        };

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

}