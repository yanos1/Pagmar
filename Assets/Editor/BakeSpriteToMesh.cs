using UnityEngine;
using UnityEditor;
using System.IO;

public class SpriteToMeshBaker
{
    [MenuItem("Tools/Bake Sprite to Mesh")]
    public static void BakeSelectedSpriteToMesh()
    {
        Object obj = Selection.activeObject;
        if (obj == null || !(obj is Sprite))
        {
            Debug.LogError("Please select a Sprite in the Project view.");
            return;
        }

        Sprite sprite = (Sprite)obj;

        Mesh mesh = new Mesh();
        mesh.name = sprite.name + "_Mesh";

        Vector3[] vertices = new Vector3[sprite.vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2 v = sprite.vertices[i];
            vertices[i] = new Vector3(v.x, v.y, 0f);
        }

        mesh.vertices = vertices;

        int[] triangles = new int[sprite.triangles.Length];
        for (int i = 0; i < triangles.Length; i++)
        {
            triangles[i] = sprite.triangles[i];
        }

        mesh.triangles = triangles;

        Vector2[] uvs = new Vector2[sprite.uv.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = sprite.uv[i];
        }

        mesh.uv = uvs;

        // Save as asset
        string path = "Assets/" + sprite.name + "_Mesh.asset";
        AssetDatabase.CreateAsset(mesh, path);
        AssetDatabase.SaveAssets();

        Debug.Log("Mesh saved to: " + path);
    }
}