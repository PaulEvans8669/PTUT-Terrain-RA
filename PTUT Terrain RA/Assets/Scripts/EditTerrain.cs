using Assets.Scripts.lib;
using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

class EditTerrain : MonoBehaviour
{
    IDbConnection dbConnection;

    private List<GameObject> chunkList;

    private Camera mainCamera;

    public int TERRAIN_SIZE = 1;
    private int CHUNK_SIZE = 32;
    private int TEXTURE_SIZE = 256;

    private GameObject spotLight;
    private Light spotLight_Light;
    public int taillePinceau = 10;

    public int FORCE = 10;
    public Texture2D ModelTexture;
    public GameObject model;

    private Assets.Scripts.lib.Terrain mainTerrain;
    private bool generated = false;



    void Start()
    {
        mainCamera = Camera.main;
        spotLight = GameObject.Find("Spot Light");
        spotLight_Light = spotLight.GetComponent<Light>();

        string dbFilePath = "Assets/Resources/mapsData.db";
        dbConnection = new SqliteConnection("URI=file:" + dbFilePath);

        openConnection();

    }

    void OnApplicationQuit()
    {
        closeConnection();
    }

    void Update()
    {
        checkPublicParametersValues();
        manageTerrainUpdates();
        manageDatabaseActions();

        if (Input.GetKeyUp(KeyCode.G))
        {
            if (!generated)
            {
                if (TERRAIN_SIZE < 4)
                {
                    TERRAIN_SIZE = 4;
                }
                mainTerrain = new Assets.Scripts.lib.Terrain(this.gameObject, "Volcan0", TERRAIN_SIZE);
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
                    if (ModelTexture != null)
                    {
                        editColor(targetChunkId, coordHitMesh);
                    }
                    editHeights(targetChunkId, coordHitMesh);

                }
                else if (Input.GetMouseButton(1))
                {
                    if (model != null)
                    {
                        generateNature(targetChunkId, coordHitMesh);
                    }
                }
            }
        }
    }

    private void manageDatabaseActions()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            mainTerrain.save(dbConnection);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            closeConnection();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            openConnection();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            mainTerrain = new Assets.Scripts.lib.Terrain(dbConnection, this.gameObject, 17);
        }
    }

    /* Editors
     *  Color: modifie la texture
     *  Nature: ajoute de la végétation
     *  Heights: modifie la hauteur du chunk
     */
    #region Editors
    Dictionary<GameObject, Texture2D> modifiedTextures = new Dictionary<GameObject, Texture2D>();
    private void editColor(int targetChunkId, Vector3 centerPoint)
    {


        Color[] modelTextureColors = new Color[TEXTURE_SIZE * TEXTURE_SIZE];
        modelTextureColors = ModelTexture.GetPixels(0, 0, TEXTURE_SIZE, TEXTURE_SIZE);
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


                Chunk correctChunk = getCorrectChunkForTexture(chunk, ref editY, ref editX);
                if (correctChunk != null)
                {
                    correctChunk.NeedsTextureUpdate = true;
                    //if (!modifiedTextures.ContainsKey(correctChunk.ChunkGameObject))
                    //{
                    //    modifiedTextures.Add(correctChunk.ChunkGameObject, correctChunk.ChunkGameObject.GetComponent<Renderer>().material.mainTexture as Texture2D);
                    //}

                    //Texture2D textureChunk = modifiedTextures[correctChunk.ChunkGameObject];
                    //textureChunk.SetPixel(editX, editY, texture.GetPixel(editX, editY));

                    Color c = modelTextureColors[editY * TEXTURE_SIZE + editX];
                    correctChunk.Texture.SetPixel(editX, editY, c);//possibilité d'optimisation importante!! passer un tableau de couleurs dans SetPixel
                }
            }
        }
        foreach(Chunk modifiedChunk in mainTerrain.ChunkList)
        {
            if (modifiedChunk.NeedsTextureUpdate)
            {
                modifiedChunk.Texture.Apply();
                modifiedChunk.NeedsTextureUpdate = false;
            }
        }
        //foreach (KeyValuePair<GameObject, Texture2D> modifiedTexture in modifiedTextures)
        //{
        //    modifiedTexture.Value.Apply();
        //}
        //modifiedTextures.Clear();
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
                Chunk correctChunk = getCorrectChunkForMesh(chunk, ref editY, ref editX);

                if (correctChunk != null)
                {
                    if (rand < 0.15)
                    {


                        correctChunk.addVegetation(model, editX, editY);

                    }
                }
            }
        }
    }

    Dictionary<Chunk, List<Vector3>> modifiedChunks = new Dictionary<Chunk, List<Vector3>>();
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
                    GameObject correctChunk = corChunk.ChunkGameObject;

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


                    if (!modifiedChunks.ContainsKey(corChunk))
                    {
                        List<Vector3> chunkVertices = new List<Vector3>();
                        correctChunk.GetComponent<MeshFilter>().mesh.GetVertices(chunkVertices);
                        modifiedChunks.Add(corChunk, chunkVertices);
                    }
                    List<Vector3> vertices = modifiedChunks[corChunk];

                    float height = vertices[index].y;
                    vertices[index] = new Vector3(vertices[index].x, (height + (float)FORCE / 100 * ((maxDist - dist) / maxDist)), vertices[index].z);

                }
            }
        }
        recalculateColliders();
        recalculateNature();
        mainTerrain.recalculateAdjacentHeights(chunk);
        modifiedChunks.Clear();

    }
    #endregion

    /* Recalculators:
     *  Colliders: actulise les colliders des chunks modifiés
     *  Nature: actualise la position de la végéation pour les chunks modifiés
     */
    #region Recalculators
    private void recalculateColliders()
    {
        foreach (KeyValuePair<Chunk, List<Vector3>> modifiedChunk in modifiedChunks)
        {
            modifiedChunk.Key.ChunkGameObject.GetComponent<MeshFilter>().mesh.SetVertices(modifiedChunk.Value);
            modifiedChunk.Key.ChunkGameObject.GetComponent<MeshCollider>().sharedMesh = modifiedChunk.Key.ChunkGameObject.GetComponent<MeshFilter>().mesh;
        }
    }

    private void recalculateNature()
    {
        foreach (KeyValuePair<Chunk, List<Vector3>> modifiedChunk in modifiedChunks)
        {
            GameObject chunk = modifiedChunk.Key.ChunkGameObject;
            List<Vector3> listHeights = modifiedChunk.Value;
            foreach (Transform child in chunk.transform)
            {
                float x = child.transform.localPosition.x;
                float z = child.transform.localPosition.z;
                int index = (int)z * (CHUNK_SIZE + 1) + (int)x;
                float height = listHeights[index].y;
                if (height > -1)
                {
                    child.transform.localPosition = new Vector3(x, height, z);
                }
                else
                {
                    modifiedChunk.Key.deleteVegetation(child.gameObject);
                }
            }
        }

    }
    #endregion

    /* Chunk Utils:
     * get correct Chunk: For Mesh / For Texture
     */
    #region Util
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
    #endregion

    /* Database 
     * 
     */
    #region Datbase

    public void openConnection()
    {
        if (dbConnection.State.Equals(ConnectionState.Closed))
        {
            dbConnection.Open();
            Debug.Log("Connection opened.");
        }
        else
        {
            Debug.Log("Connection not closed.");
        }
    }


    public void closeConnection()
    {
        if (dbConnection != null && !dbConnection.State.Equals(ConnectionState.Closed))
        {
            dbConnection.Close();
            Debug.Log("Connection closed.");
        }
        else
        {
            Debug.Log("Connection already closed.");
        }
    }
    #endregion
}
