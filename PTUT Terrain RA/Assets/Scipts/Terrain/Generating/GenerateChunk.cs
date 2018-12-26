using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GenerateChunk : MonoBehaviour {

    private int CHUNK_SIZE;
    


    private void Awake()
    {
        CHUNK_SIZE = transform.parent.gameObject.GetComponent<GenerateTerrain>().getChunkSize();
        //Debug.Log("CS: " + CHUNK_SIZE);
        GenerateMesh();


        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        for (int z = 0; z < CHUNK_SIZE + 1; z++)
        {
            for (int x = 0; x < CHUNK_SIZE + 1; x++)
            {
                int index = z * (CHUNK_SIZE + 1) + x;
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
        return CHUNK_SIZE;
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

        for (int i = 0, z = 0; z <= CHUNK_SIZE; z++)
        {
            for (int x = 0; x <= CHUNK_SIZE; x++, i++)
            {
                vertices.Add(new Vector3(x, 0, z));
                uv.Add(new Vector2((float)x / CHUNK_SIZE, (float)z / CHUNK_SIZE));
                tangents.Add(tangent);
            }
        }
        mesh.MarkDynamic();
        mesh.SetVertices(vertices);
        mesh.SetUVs(0,uv);
        mesh.SetTangents(tangents);
        


        int[] triangles = new int[CHUNK_SIZE * CHUNK_SIZE * 6];
        for (int ti = 0, vi = 0, z = 0; z < CHUNK_SIZE; z++, vi++)
        {
            for (int x = 0; x < CHUNK_SIZE; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + CHUNK_SIZE + 1;
                triangles[ti + 5] = vi + CHUNK_SIZE + 2;
            }
        }


        mesh.SetTriangles(triangles,0);
        mesh.RecalculateNormals();
    }

    private void GenerateTexture()
    {
        Texture2D texture = new Texture2D(CHUNK_SIZE + 1, CHUNK_SIZE + 1);
        GetComponent<Renderer>().material.mainTexture = texture;

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        List<Vector3> vertices = new List<Vector3>();
        mesh.GetVertices(vertices);

        for (int z = 0; z < CHUNK_SIZE + 1; z++)
        {
            for (int x = 0; x < CHUNK_SIZE + 1; x++)
            {
                int index = z * (CHUNK_SIZE + 1) + x;
                float height = vertices[index].y;
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