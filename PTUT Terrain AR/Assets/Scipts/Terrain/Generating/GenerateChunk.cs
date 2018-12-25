using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GenerateChunk : MonoBehaviour {

    private int chunkSize = 100;
    

    private void Awake()
    {
        GenerateMesh();


        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        for (int z = 0; z < chunkSize + 1; z++)
        {
            for (int x = 0; x < chunkSize + 1; x++)
            {
                int index = z * (chunkSize + 1) + x;
                vertices[index] = new Vector3(vertices[index].x, 0, vertices[index].z);
               /*
                if (x<(chunkSize+1)/2)
                    vertices[index] = new Vector3(vertices[index].x, 5, vertices[index].z);
                if (y == yc && x == xc)
                    vertices[index] = new Vector3(vertices[index].x, 10, vertices[index].z);
                */
            }
        }

        mesh.vertices = vertices;


        MeshCollider meshCollider = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        meshCollider.sharedMesh = mesh;

        GenerateTexture();

    }

    public int getChunkSize(){
        return chunkSize;
    }

    private void GenerateMesh()
    {
        Mesh mesh;
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<Vector4> tangents = new List<Vector4>();

        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);

        for (int i = 0, z = 0; z <= chunkSize; z++)
        {
            for (int x = 0; x <= chunkSize; x++, i++)
            {
                vertices.Add(new Vector3(x, 0, z));
                uv.Add(new Vector2((float)x / chunkSize, (float)z / chunkSize));
                tangents.Add(tangent);
            }
        }
        mesh.MarkDynamic();
        mesh.SetVertices(vertices);
        mesh.SetUVs(0,uv);
        mesh.SetTangents(tangents);
        


        int[] triangles = new int[chunkSize * chunkSize * 6];
        for (int ti = 0, vi = 0, z = 0; z < chunkSize; z++, vi++)
        {
            for (int x = 0; x < chunkSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + chunkSize + 1;
                triangles[ti + 5] = vi + chunkSize + 2;
            }
        }


        mesh.SetTriangles(triangles,0);
        mesh.RecalculateNormals();
    }

    private void GenerateTexture()
    {
        Texture2D texture = new Texture2D(chunkSize + 1, chunkSize + 1);
        GetComponent<Renderer>().material.mainTexture = texture;

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        List<Vector3> vertices = new List<Vector3>();
        mesh.GetVertices(vertices);
        Debug.Log("Vertices: "+vertices.ToString());

        for (int z = 0; z < chunkSize + 1; z++)
        {
            for (int x = 0; x < chunkSize + 1; x++)
            {
                int index = z * (chunkSize + 1) + x;
                float height = vertices[index].y;
                Debug.Log("Height: " + height);
                Color color = new Color(255, 255, 255);
                texture.SetPixel(x, z, color);
            }
        }
        texture.Apply();


    }
    void Update()
    {




    }
}