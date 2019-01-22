using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EditTerrain : MonoBehaviour {

    private int CHUNK_SIZE;
    private int TEXTURE_SIZE;
    private int TERRAIN_SIZE;
    private Texture2D myTexture;


    private Camera mainCamera;

    private GameObject spotLight;
    private Light spotLight_Light;
    public int taillePinceau = 10;
    public int FORCE  = 10;
    public Texture2D texture;
    public GameObject model;


    Dictionary<GameObject, List<Vector3>> modifiedChunks = new Dictionary<GameObject, List<Vector3>>();
    Dictionary<GameObject, Texture2D> modifiedTextures = new Dictionary<GameObject, Texture2D>();

    // Use this for initialization
    void Start () {
        CHUNK_SIZE = this.gameObject.GetComponent<GenerateTerrain>().getChunkSize();
        TEXTURE_SIZE = this.gameObject.GetComponent<GenerateTerrain>().getTextureSize();
        TERRAIN_SIZE = this.gameObject.GetComponent<GenerateTerrain>().getTerrainSize();

        mainCamera = Camera.main;
        spotLight = GameObject.Find("Spot Light");
        spotLight_Light = spotLight.GetComponent<Light>();
        //myTexture = (Texture2D)Resources.Load("/Assets/Ground_textures_pack/Ground_weeds/diffuse.tga");

    }

    // Update is called once per frame
    void Update() {
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
            if (Input.GetMouseButton(0))
            {

                
                editColor(targetChunk, coordHitMesh);
                editHeights(targetChunk, coordHitMesh);
                recalculateColliders();

            }
            else if (Input.GetMouseButton(1))
            {
                generateNature(targetChunk, hitPoint);
                //Debug.Log(targetChunk.name);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                supprNature();
            }
        }
    }

    private void supprNature()
    {
        GameObject[] tab = GameObject.FindObjectsOfType<GameObject>();
        for (int i = 0; i < tab.Length; i++)
        {
            if (tab[i].name == "vegetation")
            {

                Destroy(tab[i]);

            }
        }
    }

    private void generateNature(GameObject chunk, Vector3 centerPoint)
    {

        // z divisé
        // x modulo

        //Debug.Log(chunk.name);

        int x = (int)Mathf.Round(centerPoint.x);
        int y = -(int)Mathf.Round(centerPoint.z)+CHUNK_SIZE;
        //Debug.Log(x + " || " + y);
        //Debug.Log("");

        int minY = (int)(y - ((CHUNK_SIZE * (float)taillePinceau / 2 / 100))) + 1;
        int maxY = (int)(y + ((CHUNK_SIZE * (float)taillePinceau / 2 / 100)));

        int minX = (int)(x - (CHUNK_SIZE * ((float)taillePinceau / 2 / 100))) + 1;
        int maxX = (int)(x + (CHUNK_SIZE * ((float)taillePinceau / 2 / 100)));

        //Debug.Log("MinY = "+minY+" || maxY = "+maxY);
        //Debug.Log("");
        //Debug.Log("MinX = " + minX + " || maxX = " + maxX);

        supprModels(minX, minY, maxX, maxY);
        //Debug.Log("suppr");
        for(int i = minY; i < maxY; i++)
        {
            //Debug.Log(i);
            for(int j = minX; j < maxX; j++)
            {

                float rand = Random.Range((float)0.1, (float)10.0);

                int numChunk = int.Parse(chunk.name.Split(' ')[1]);

                int xChunk = numChunk % 4;
                int yChunk = numChunk / 4;
                int posX = j % CHUNK_SIZE;
                int posY = i % CHUNK_SIZE;

                int index = posY * (CHUNK_SIZE + 1) + posX;
                //Debug.Log("ok");
                GameObject correctChunk = getCorrectChunkForMesh(chunk, ref posY, ref posX);
                //KO
                List<Vector3> chunkVertices = new List<Vector3>();
                //KO
                correctChunk.GetComponent<MeshFilter>().mesh.GetVertices(chunkVertices);
                //modifiedChunks.Add(correctChunk, chunkVertices);
                //List<Vector3> vertices = modifiedChunks[correctChunk];

                //KO
                if (rand < 0.5 && j >= 0 && j <= 4*CHUNK_SIZE && i >= 0 && i <= 4 * CHUNK_SIZE)
                {
                    //Debug.Log(i+"        "+(-(i - CHUNK_SIZE)));
                    GameObject clone = Instantiate(model, new Vector3(j, chunkVertices[index].y, -(i-CHUNK_SIZE)), Quaternion.identity) as GameObject;
                    clone.name = "vegetation";
                    clone.transform.parent = correctChunk.transform;
                }
            }           
        }

    }

    private float calculHauteur()
    {
        return 0;
    }

    private void OnDestroy()
    {
        Debug.Log("je detruit une entité");
    }

    private void supprModels(int minX, int minY, int maxX, int maxY)
    {
        //Debug.Log("debut suppr");
        Debug.Log(minX % 32 + "<x<"+ maxX % 32 + " || "+ minY % 32 + "<y<"+ maxY % 32);
        GameObject[]tab = GameObject.FindObjectsOfType<GameObject>();
        for (int i = 0; i < tab.Length; i++)
        {
            float x = tab[i].transform.localPosition.x;
            float y = tab[i].transform.localPosition.z;

            if (tab[i].name == "vegetation") {

                Debug.Log("deboguage : " + tab[i].name + ", x:" + x + ", z:" + y);

                if (x >= minX % 32 && x <= maxX % 32 && y >= maxY % 32 && y <= minY % 32)
                {

                    Destroy(tab[i]);
                    Debug.Log("Suppression effectuée");

                }
            }  
        }
    }

    private void editHeights(GameObject chunk, Vector3 centerPoint)
    {

        int x = (int)Mathf.Round(centerPoint.x);
        int y = (int)Mathf.Round(centerPoint.z);

        //Debug.Log(centerPoint.ToString());
        int minY = (int)(y - (CHUNK_SIZE * ((float)taillePinceau / 2 / 100)))+1;
        int maxY = (int)(y + (CHUNK_SIZE * ((float)taillePinceau / 2 / 100)));

        int minX = (int)(x - (CHUNK_SIZE * ((float)taillePinceau / 2 / 100)))+1;
        int maxX = (int)(x + (CHUNK_SIZE * ((float)taillePinceau / 2 / 100)));

        int maxDistZ = Mathf.Abs(y - Mathf.Abs(maxY));
        int maxDistX = Mathf.Abs(x - Mathf.Abs(maxX));
        float maxDist = Mathf.Sqrt(Mathf.Pow(maxDistZ, 2) + Mathf.Pow(maxDistX, 2));

        //Debug.Log("minY: " + minY + "maxY: " + maxY + "minX: " + minX + "maxX: " + maxX);
        for (int i = minY; i <= maxY; i++)
        {
            for (int j = minX; j <= maxX; j++)
            {

                int editZ = i;
                int editX = j;

                GameObject correctChunk = getCorrectChunkForMesh(chunk, ref editZ, ref editX);
                if(correctChunk != null)
                {
                    
                    int index = editZ * (CHUNK_SIZE+1) + editX;

                    int distZ = Mathf.Abs(y - i);
                    int distX = Mathf.Abs(x - j);
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
                    vertices[index] = new Vector3(vertices[index].x, (height + (float)FORCE/100 * ((maxDist - dist) / maxDist)), vertices[index].z);


                    /*
                     * Si on est au bord d'un chunk, mais qu'on ne le dépasse pas (!) ,
                     * il faut aussi changer la hauteur du chunk a côté
                     * sinon artefact de vide
                     */

                }
            }
        }
    }

    private void recalculateColliders()
    {
        foreach (KeyValuePair<GameObject, List<Vector3>> modifiedChunk in modifiedChunks)
        {
            modifiedChunk.Key.GetComponent<MeshFilter>().mesh.SetVertices(modifiedChunk.Value);
            modifiedChunk.Key.GetComponent<MeshCollider>().sharedMesh = modifiedChunk.Key.GetComponent<MeshFilter>().mesh;
        }
        modifiedChunks.Clear();
    }

    private void editColor(GameObject chunk, Vector3 centerPoint)
    {

        int x = (int)Mathf.Round(centerPoint.x) * (TEXTURE_SIZE / CHUNK_SIZE);
        int y = (int)Mathf.Round(centerPoint.z) * (TEXTURE_SIZE / CHUNK_SIZE);

        int minY = (int)((y - (TEXTURE_SIZE * ((float)taillePinceau / 2 / 100))) + 1);
        int maxY = (int)((y + (TEXTURE_SIZE * ((float)taillePinceau / 2 / 100))));

        int minX = (int)((x - (TEXTURE_SIZE * ((float)taillePinceau / 2 / 100))) + 1);
        int maxX = (int)((x + (TEXTURE_SIZE * ((float)taillePinceau/2 / 100))));

        for (int i = minY; i <= maxY; i++)
        {
            for (int j = minX; j <= maxX; j++)
            {
                int editY = i;
                int editX = j;


                GameObject correctChunk = getCorrectChunkForTexture(chunk, ref editY, ref editX);
                if (correctChunk != null)
                {

                    int distY = Mathf.Abs(y - i);
                    int distX = Mathf.Abs(x - j);
                    float dist = Mathf.Sqrt(Mathf.Pow(distY, 2) + Mathf.Pow(distX, 2));


                    if (!modifiedTextures.ContainsKey(correctChunk))
                    {
                        modifiedTextures.Add(correctChunk, correctChunk.GetComponent<Renderer>().material.mainTexture as Texture2D);
                    }


                    //Color c = new Color(dist / maxDist, (maxDist - dist) / maxDist, 0);
                    Texture2D textureChunk = modifiedTextures[correctChunk];
                    //Debug.Log("pos X = " + editX + " ||  pos Y = " + editY);
                    //Debug.Log(texture.GetPixel(editX, editY));
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

    private Color getColorForHeight(float height)
    {
        Color c = new Color((float)238 / 255, (float)229 / 255, (float)218 / 255);
        if(9<= height && height < 12)
        {
            /*
            int rand = Random.Range(0, 3);
            if (rand > 1)
            {
                c = new Color((float)153 / 255, (float)116 / 255, (float)73 / 255);
            }
            */
        }
        else if (5<= height && height < 9)
        {
            c = new Color((float)153 / 255, (float)116 / 255, (float)73 / 255);
        }
        else if (height < 5)
        {
            c = new Color((float)113 / 255, (float)125 / 255, (float)45 / 255);
        }
        return c;
    }

    private GameObject getLeftChunk(GameObject chunk)
    {
        int id = int.Parse(chunk.name.Split(' ')[1]);
        int idChunkOnTheLeft = id - 1;
        if (idChunkOnTheLeft >= 0 && (int)(idChunkOnTheLeft/TERRAIN_SIZE) == (int)(id/TERRAIN_SIZE))
        {
            return GameObject.Find("Chunk " + idChunkOnTheLeft);
        }
        return null;
    }

    private GameObject getRightChunk(GameObject chunk)
    {
        int id = int.Parse(chunk.name.Split(' ')[1]);
        int idChunkOnTheRight = id + 1;
        if (idChunkOnTheRight < (1+id/TERRAIN_SIZE)*TERRAIN_SIZE)
        {
            return GameObject.Find("Chunk " + idChunkOnTheRight);
        }
        return null;
    }

    private GameObject getTopChunk(GameObject chunk)
    {
        int id = int.Parse(chunk.name.Split(' ')[1]);
        int idChunkAbove = id - TERRAIN_SIZE;
        if (idChunkAbove >= 0)
        {
            return GameObject.Find("Chunk " + idChunkAbove);
        }
        return null;
    }

    private GameObject getBottomChunk(GameObject chunk)
    {
        int id = int.Parse(chunk.name.Split(' ')[1]);
        int idChunkUnder = id + TERRAIN_SIZE;
        if (idChunkUnder < TERRAIN_SIZE*TERRAIN_SIZE)
        {
            return GameObject.Find("Chunk " + idChunkUnder);
        }
        return null;
    }

    private GameObject getTopLeftChunk(GameObject chunk)
    {
        GameObject topChunk = getTopChunk(chunk);
        if(topChunk != null)
        {
            return getLeftChunk(topChunk);
        }
        return null;
    }

    private GameObject getTopRightChunk(GameObject chunk)
    {
        GameObject topChunk = getTopChunk(chunk);
        if (topChunk != null)
        {
            return getRightChunk(topChunk);
        }
        return null;
    }

    private GameObject getBottomLeftChunk(GameObject chunk)
    {
        GameObject bottomChunk = getBottomChunk(chunk);
        if (bottomChunk != null)
        {
            return getLeftChunk(bottomChunk);
        }
        return null;
    }

    private GameObject getBottomRightChunk(GameObject chunk)
    {
        GameObject bottomChunk = getBottomChunk(chunk);
        if (bottomChunk != null)
        {
            return getRightChunk(bottomChunk);
        }
        return null;
    }

    private GameObject getCorrectChunkForMesh(GameObject chunk, ref int y, ref int x)
    {
        //Debug.Log("CHUNK_SIZE: " + CHUNK_SIZE);
        if (x >= CHUNK_SIZE+1 && y >= CHUNK_SIZE+1)
        {
            //Debug.Log("TR");
            x -= CHUNK_SIZE + 1;
            y -= CHUNK_SIZE + 1;
            return getTopRightChunk(chunk);
        }
        else if (x >= CHUNK_SIZE+1 && y >= 0)
        {
            //Debug.Log("R");
            x -= CHUNK_SIZE + 1;
            return getRightChunk(chunk);
        }
        else if (x >= CHUNK_SIZE+1)
        {
            //Debug.Log("BR");
            x -= CHUNK_SIZE + 1;
            y += CHUNK_SIZE + 1;
            return getBottomRightChunk(chunk);
        }
        else if (x >= 0 && y >= CHUNK_SIZE + 1)
        {
            //Debug.Log("T");
            y -= CHUNK_SIZE + 1;
            return getTopChunk(chunk);
        }
        else if (x >= 0 && y >= 0)
        {
            return chunk;
        }
        else if (x >= 0)
        {
            //Debug.Log("B");
            y += CHUNK_SIZE + 1;
            return getBottomChunk(chunk);
        }
        else if (y >= CHUNK_SIZE + 1)
        {
            //Debug.Log("TL");
            x += CHUNK_SIZE + 1;
            y -= CHUNK_SIZE + 1;
            return getTopLeftChunk(chunk);
        }
        else if (y >= 0)
        {
            //Debug.Log("L");
            x += CHUNK_SIZE + 1;
            return getLeftChunk(chunk);
        }
        else
        {
            //Debug.Log("BL");
            x += CHUNK_SIZE + 1;
            y += CHUNK_SIZE + 1;
            return getBottomLeftChunk(chunk);
        }

    }

    private GameObject getCorrectChunkForTexture(GameObject chunk, ref int y, ref int x)
    {
        if (x >= TEXTURE_SIZE && y >= TEXTURE_SIZE)
        {
            //Debug.Log("TR");
            x -= TEXTURE_SIZE;
            y -= TEXTURE_SIZE;
            return getTopRightChunk(chunk);
        }
        else if (x >= TEXTURE_SIZE && y >= 0)
        {
            //Debug.Log("R");
            x -= TEXTURE_SIZE;
            return getRightChunk(chunk);
        }
        else if (x >= TEXTURE_SIZE)
        {
            //Debug.Log("BR");
            x -= TEXTURE_SIZE;
            y += TEXTURE_SIZE;
            return getBottomRightChunk(chunk);
        }
        else if (x >= 0 && y >= TEXTURE_SIZE)
        {
            //Debug.Log("T");
            y -= TEXTURE_SIZE;
            return getTopChunk(chunk);
        }
        else if (x >= 0 && y >= 0)
        {
            return chunk;
        }
        else if (x >= 0)
        {
            //Debug.Log("B");
            y += TEXTURE_SIZE;
            return getBottomChunk(chunk);
        }
        else if (y >= TEXTURE_SIZE)
        {
            //Debug.Log("TL");
            x += TEXTURE_SIZE;
            y -= TEXTURE_SIZE;
            return getTopLeftChunk(chunk);
        }
        else if (y >= 0)
        {
            //Debug.Log("L");
            x += TEXTURE_SIZE;
            return getLeftChunk(chunk);
        }
        else
        {
            //Debug.Log("BL");
            x += TEXTURE_SIZE;
            y += TEXTURE_SIZE;
            return getBottomLeftChunk(chunk);
        }

    }
}
