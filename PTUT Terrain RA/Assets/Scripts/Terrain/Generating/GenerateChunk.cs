using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GenerateChunk : MonoBehaviour {

    private int CHUNK_SIZE;
    private int TEXTURE_SIZE;


    private void Awake()
    {
        CHUNK_SIZE = GameObject.Find("Terrain").gameObject.GetComponent<GenerateTerrain>().getChunkSize();
        TEXTURE_SIZE = GameObject.Find("Terrain").gameObject.GetComponent<GenerateTerrain>().getTextureSize();
        generateMesh();
        generateTexture();
        /*
        */
    }

    public int getChunkSize(){
        return CHUNK_SIZE;
    }

    private void generateMesh()
    {

        /*
         *  Vertices
         *  UVs
         *  Tangents
         */
        Mesh mesh;
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Chunk";

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
        
        /*
         * Triangles
         */ 

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

        /*
         * Collider
         */

        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        

    }

    public void generateTexture()
    {

        Texture2D texture = new Texture2D(TEXTURE_SIZE, TEXTURE_SIZE);
        texture.filterMode = FilterMode.Bilinear;
        this.GetComponent<Renderer>().material.mainTexture = texture;
        this.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2((float)0.0001, (float)0.0001));
        texture = this.GetComponent<Renderer>().material.mainTexture as Texture2D;
        for (int textureY = 0; textureY < TEXTURE_SIZE; textureY++)
        {
            for (int textureX = 0; textureX < TEXTURE_SIZE; textureX++)
            {
                Color color = new Color((float)113 / 255, (float)125 / 255, (float)45 / 255);
                if (textureX == 0 || textureX == TEXTURE_SIZE - 1 || textureY == 0 || textureY == TEXTURE_SIZE - 1)
                {
                    color = new Color(0, 0, 1);
                }
                texture.SetPixel(textureX, textureY, color);
            }
        }

        texture.Apply();
        this.GetComponent<Renderer>().material.enableInstancing = true;
    }
}