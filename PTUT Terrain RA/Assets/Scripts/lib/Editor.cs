using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Editor : MonoBehaviour
{
    private List<GameObject> chunkList;

    private Camera mainCamera;

    public int TERRAIN_SIZE = 1;
    private int CHUNK_SIZE = 32;
    private int TEXTURE_SIZE = 256;

    private GameObject spotLight;
    private Light spotLight_Light;
    public int taillePinceau = 10;

    public int FORCE = 10;
    public Texture2D texture;
    public GameObject model;

    private Assets.Scripts.lib.Terrain mainTerrain;
    private bool generated = false;

    void Start()
    {
        mainCamera = Camera.main;
        spotLight = GameObject.Find("Spot Light");
        spotLight_Light = spotLight.GetComponent<Light>();
    }

    void Update()
    {
        checkPublicParametersValues();
        manageTerrainUpdates();

        if (Input.GetKeyUp(KeyCode.G))
        {
            if (!generated)
            {
                Debug.Log("in");
                if (TERRAIN_SIZE < 4)
                {
                    TERRAIN_SIZE = 4;
                }
                mainTerrain = new Assets.Scripts.lib.Terrain(this.gameObject, "Savanne", TERRAIN_SIZE);
            }
        }

    }

    private void checkPublicParametersValues()
    {
        if (taillePinceau < 1)
        {
            taillePinceau = 1;
        }
        else if (taillePinceau > 100)
        {
            taillePinceau = 100;
        }

        int newSpotLight_LightSize = taillePinceau * CHUNK_SIZE / 100;
        if (newSpotLight_LightSize < 4)
        {
            newSpotLight_LightSize = 4;
        }
        spotLight_Light.cookieSize = newSpotLight_LightSize;
    }

    private void manageTerrainUpdates()
    {
        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hitInfo))
        {
            spotLight_Light.enabled = true;

            Vector3 hitPoint = hitInfo.point;
            Collider collider = hitInfo.collider;

            Vector3 coordHitMesh = collider.transform.InverseTransformPoint(hitPoint);
            Vector3 coordSpotLight = hitPoint;
            coordSpotLight.y += 100;
            spotLight.transform.position = coordSpotLight;
            GameObject targetChunk = collider.gameObject;
            if (targetChunk.name.Contains("Chunk"))
            {
                int targetChunkId = int.Parse(targetChunk.name.Split(' ')[1]);
                if (Input.GetMouseButton(0))
                {
                    
                    editColor(targetChunkId, coordHitMesh);
                    editHeights(targetChunkId, coordHitMesh);

                }
                else if (Input.GetMouseButton(1))
                {
                    generateNature(targetChunkId, coordHitMesh);
                }
            }
        }
    }

    Dictionary<GameObject, Texture2D> modifiedTextures = new Dictionary<GameObject, Texture2D>();
    private void editColor(int targetChunkId, Vector3 centerPoint)
    {
        Assets.Scripts.lib.Chunk chunk = mainTerrain.ChunkList[targetChunkId];

        int x = (int)Mathf.Round(centerPoint.x) * (TEXTURE_SIZE / CHUNK_SIZE);
        int y = (int)Mathf.Round(centerPoint.z) * (TEXTURE_SIZE / CHUNK_SIZE);

        int minY = (int)((y - (TEXTURE_SIZE * ((float)taillePinceau / 2 / 100))));
        int maxY = (int)((y + (TEXTURE_SIZE * ((float)taillePinceau / 2 / 100))));

        int minX = (int)((x - (TEXTURE_SIZE * ((float)taillePinceau / 2 / 100)))) - 1;
        int maxX = (int)((x + (TEXTURE_SIZE * ((float)taillePinceau / 2 / 100)))) + 1;
        

        for (int i = minY; i <= maxY; i++)
        {
            for (int j = minX; j <= maxX; j++)
            {
                int editY = i;
                int editX = j;


                Assets.Scripts.lib.Chunk correctChunk = getCorrectChunkForTexture(chunk, ref editY, ref editX);
                if (correctChunk != null)
                {

                    if (!modifiedTextures.ContainsKey(correctChunk.GameObject))
                    {
                        modifiedTextures.Add(correctChunk.GameObject, correctChunk.GameObject.GetComponent<Renderer>().material.mainTexture as Texture2D);
                    }
                    
                    Texture2D textureChunk = modifiedTextures[correctChunk.GameObject];
                    textureChunk.SetPixel(editX, editY, texture.GetPixel(editX, editY));
                }
            }
        }
        foreach (KeyValuePair<GameObject, Texture2D> modifiedTexture in modifiedTextures)
        {
            modifiedTexture.Value.Apply();
        }
        modifiedTextures.Clear();
    }

    private void generateNature(int targetChunkId, Vector3 centerPoint)
    {
        Assets.Scripts.lib.Chunk chunk = mainTerrain.ChunkList[targetChunkId];

        int x = (int)Mathf.Round(centerPoint.x);
        int y = (int)Mathf.Round(centerPoint.z);

        int minY = (int)(y - ((CHUNK_SIZE * (float)taillePinceau / 2 / 100))) + 1;
        int maxY = (int)(y + ((CHUNK_SIZE * (float)taillePinceau / 2 / 100)));

        int minX = (int)(x - (CHUNK_SIZE * ((float)taillePinceau / 2 / 100))) + 1;
        int maxX = (int)(x + (CHUNK_SIZE * ((float)taillePinceau / 2 / 100)));
        
        for (int i = minY; i < maxY; i++)
        {
            for (int j = minX; j < maxX; j++)
            {

                float rand = UnityEngine.Random.Range((float)0.1, (float)10.0);
                

                int editY = i;
                int editX = j;
                GameObject correctChunk = getCorrectChunkForMesh(chunk, ref editY, ref editX).GameObject;

                if (correctChunk != null)
                {
                    int index = editY * (CHUNK_SIZE + 1) + editX;

                    List<Vector3> chunkVertices = new List<Vector3>();

                    correctChunk.GetComponent<MeshFilter>().mesh.GetVertices(chunkVertices);

                    if (rand < 0.5 && editX >= 0 && editX <= 4 * CHUNK_SIZE && editY >= 0 && editY <= 4 * CHUNK_SIZE)
                    {
                        //Debug.Log(i+"        "+(-(i - CHUNK_SIZE)));
                        float height = chunkVertices[index].y;
                        //Debug.Log("Height: " + height);
                        GameObject clone = Instantiate(model, correctChunk.transform.position + new Vector3(editX, height, editY), Quaternion.identity) as GameObject;
                        clone.transform.parent = correctChunk.transform;
                        clone.name = "vegetation";
                    }
                }
            }
        }
    }

    Dictionary<GameObject, List<Vector3>> modifiedChunks = new Dictionary<GameObject, List<Vector3>>();

    private void editHeights(int targetChunkId, Vector3 centerPoint)
    {
        Assets.Scripts.lib.Chunk chunk = mainTerrain.ChunkList[targetChunkId];

        int x = (int)Mathf.Round(centerPoint.x);
        int y = (int)Mathf.Round(centerPoint.z);

        //Debug.Log(centerPoint.ToString());
        int minY = (int)(y - (CHUNK_SIZE * ((float)taillePinceau / 2 / 100)));
        int maxY = (int)(y + (CHUNK_SIZE * ((float)taillePinceau / 2 / 100)));

        int minX = (int)(x - (CHUNK_SIZE * ((float)taillePinceau / 2 / 100)));
        int maxX = (int)(x + (CHUNK_SIZE * ((float)taillePinceau / 2 / 100)));

        int maxDistZ = Mathf.Abs(y - Mathf.Abs(maxY)) + 1;
        int maxDistX = Mathf.Abs(x - Mathf.Abs(maxX)) + 1;
        float maxDist = Mathf.Sqrt(Mathf.Pow(maxDistZ, 2) + Mathf.Pow(maxDistX, 2));

        //Debug.Log("minY: " + minY + "maxY: " + maxY + "minX: " + minX + "maxX: " + maxX);
        for (int i = minY; i <= maxY; i++)
        {
            for (int j = minX; j <= maxX; j++)
            {

                int editZ = i;
                int editX = j;

                Assets.Scripts.lib.Chunk corChunk = getCorrectChunkForMesh(chunk, ref editZ, ref editX);
                if (corChunk != null)
                {
                    GameObject correctChunk = corChunk.GameObject;

                    int index = editZ * (CHUNK_SIZE + 1) + editX;

                    int distZ = Mathf.Abs(y - i);
                    int distX = Mathf.Abs(x - j);

                    if (editZ != i)
                    {
                        distZ--;
                    }
                    if (editX != j)
                    {
                        distX--;
                    }


                    float dist = Mathf.Sqrt(Mathf.Pow(distZ, 2) + Mathf.Pow(distX, 2));
                    //Debug.Log("Dist: " + dist);


                    if (!modifiedChunks.ContainsKey(correctChunk))
                    {
                        List<Vector3> chunkVertices = new List<Vector3>();
                        correctChunk.GetComponent<MeshFilter>().mesh.GetVertices(chunkVertices);
                        modifiedChunks.Add(correctChunk, chunkVertices);
                    }
                    List<Vector3> vertices = modifiedChunks[correctChunk];

                    float height = vertices[index].y;
                    vertices[index] = new Vector3(vertices[index].x, (height + (float)FORCE / 100 * ((maxDist - dist) / maxDist)), vertices[index].z);

                }
            }
        }
        recalculateColliders();
        recalculateNature();
        chunk.recalculateAdjacentHeights();
        modifiedChunks.Clear();

    }

    private void recalculateColliders()
    {
        foreach (KeyValuePair<GameObject, List<Vector3>> modifiedChunk in modifiedChunks)
        {
            modifiedChunk.Key.GetComponent<MeshFilter>().mesh.SetVertices(modifiedChunk.Value);
            modifiedChunk.Key.GetComponent<MeshCollider>().sharedMesh = modifiedChunk.Key.GetComponent<MeshFilter>().mesh;
        }
    }

    private void recalculateNature()
    {
        foreach (KeyValuePair<GameObject, List<Vector3>> modifiedChunk in modifiedChunks)
        {
            GameObject chunk = modifiedChunk.Key;
            List<Vector3> listHeights = modifiedChunk.Value;
            foreach (Transform child in chunk.transform)
            {
                float x = child.transform.localPosition.x;
                float z = child.transform.localPosition.z;
                int index = (int)z * (CHUNK_SIZE + 1) + (int)x;
                float height = listHeights[index].y;
                child.transform.localPosition = new Vector3(x, height, z);
            }
        }

    }

    private Assets.Scripts.lib.Chunk getCorrectChunkForMesh(Assets.Scripts.lib.Chunk chunk, ref int y, ref int x)
    {
        //Debug.Log("CHUNK_SIZE: " + CHUNK_SIZE);
        if (x >= CHUNK_SIZE + 1 && y >= CHUNK_SIZE + 1)
        {
            //Debug.Log("TR");
            x -= CHUNK_SIZE + 1;
            y -= CHUNK_SIZE + 1;
            return chunk.getTopRightChunk();
        }
        else if (x >= CHUNK_SIZE + 1 && y >= 0)
        {
            //Debug.Log("R");
            x -= CHUNK_SIZE + 1;
            return chunk.getRightChunk();
        }
        else if (x >= CHUNK_SIZE + 1)
        {
            //Debug.Log("BR");
            x -= CHUNK_SIZE + 1;
            y += CHUNK_SIZE + 1;
            return chunk.getBottomRightChunk();
        }
        else if (x >= 0 && y >= CHUNK_SIZE + 1)
        {
            //Debug.Log("T");
            y -= CHUNK_SIZE + 1;
            return chunk.getTopChunk();
        }
        else if (x >= 0 && y >= 0)
        {
            return chunk;
        }
        else if (x >= 0)
        {
            //Debug.Log("B");
            y += CHUNK_SIZE + 1;
            return chunk.getBottomChunk();
        }
        else if (y >= CHUNK_SIZE + 1)
        {
            //Debug.Log("TL");
            x += CHUNK_SIZE + 1;
            y -= CHUNK_SIZE + 1;
            return chunk.getTopLeftChunk();
        }
        else if (y >= 0)
        {
            //Debug.Log("L");
            x += CHUNK_SIZE + 1;
            return chunk.getLeftChunk();
        }
        else
        {
            //Debug.Log("BL");
            x += CHUNK_SIZE + 1;
            y += CHUNK_SIZE + 1;
            return chunk.getBottomLeftChunk();
        }
    }

    private Assets.Scripts.lib.Chunk getCorrectChunkForTexture(Assets.Scripts.lib.Chunk chunk, ref int y, ref int x)
    {
        if (x >= TEXTURE_SIZE && y >= TEXTURE_SIZE)
        {
            //Debug.Log("TR");
            x -= TEXTURE_SIZE;
            y -= TEXTURE_SIZE;
            return chunk.getTopRightChunk();
        }
        else if (x >= TEXTURE_SIZE && y >= 0)
        {
            //Debug.Log("R");
            x -= TEXTURE_SIZE;
            return chunk.getRightChunk();
        }
        else if (x >= TEXTURE_SIZE)
        {
            //Debug.Log("BR");
            x -= TEXTURE_SIZE;
            y += TEXTURE_SIZE;
            return chunk.getBottomRightChunk();
        }
        else if (x >= 0 && y >= TEXTURE_SIZE)
        {
            //Debug.Log("T");
            y -= TEXTURE_SIZE;
            return chunk.getTopChunk();
        }
        else if (x >= 0 && y >= 0)
        {
            return chunk;
        }
        else if (x >= 0)
        {
            //Debug.Log("B");
            y += TEXTURE_SIZE;
            return chunk.getBottomChunk();
        }
        else if (y >= TEXTURE_SIZE)
        {
            //Debug.Log("TL");
            x += TEXTURE_SIZE;
            y -= TEXTURE_SIZE;
            return chunk.getTopLeftChunk();
        }
        else if (y >= 0)
        {
            //Debug.Log("L");
            x += TEXTURE_SIZE;
            return chunk.getLeftChunk();
        }
        else
        {
            //Debug.Log("BL");
            x += TEXTURE_SIZE;
            y += TEXTURE_SIZE;
            return chunk.getBottomLeftChunk();
        }
    }
}
