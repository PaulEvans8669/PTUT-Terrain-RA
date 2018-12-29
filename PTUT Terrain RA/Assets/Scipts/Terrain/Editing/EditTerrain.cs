using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditTerrain : MonoBehaviour {

    private int CHUNK_SIZE;
    private int TEXTURE_SIZE;
    private int TERRAIN_SIZE;
    

    private Camera mainCamera;

    private GameObject spotLight;
    private Light spotLigh_tLight;
    public int taillePinceau = 10;
    
    
    

    // Use this for initialization
    void Start () {
        CHUNK_SIZE = this.gameObject.GetComponent<GenerateTerrain>().getChunkSize();
        TEXTURE_SIZE = this.gameObject.GetComponent<GenerateTerrain>().getTextureSize();
        TERRAIN_SIZE = this.gameObject.GetComponent<GenerateTerrain>().getTerrainSize();

        mainCamera = Camera.main;
        spotLight = GameObject.Find("Spot Light");
        spotLigh_tLight = spotLight.GetComponent<Light>();
        
        
    }
	
	// Update is called once per frame
	void Update () {
        if (taillePinceau < 1 )
        {
            taillePinceau = 1;
        }
        else if(taillePinceau > 100)
        {
            taillePinceau = 100;
        }
        spotLigh_tLight.cookieSize = TERRAIN_SIZE*taillePinceau*CHUNK_SIZE/100;


        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hitInfo))
        {
            spotLigh_tLight.enabled = true;
            // Move the cursor to the point where the raycast hit.
            //this.transform.position = hitInfo.point;

            Vector3 hitPoint = hitInfo.point;
            Collider collider = hitInfo.collider;

            Vector3 coordHitMesh = collider.transform.InverseTransformPoint(hitPoint);
            Vector3 coordSpotLight = hitPoint;
            coordSpotLight.y += 100;
            spotLight.transform.position = coordSpotLight;
            //Debug.Log("x: " + hitPoint.x + " y: " + hitPoint.y + " z: " + hitPoint.z);

            if (Input.GetMouseButton(0))
            {

                GameObject targetChunk = collider.gameObject;
                editHeights(targetChunk, coordHitMesh);
                editColor(targetChunk, coordHitMesh);
                
            }

        }
        else
        {
            spotLigh_tLight.enabled = false;
        }


    }

    private void editHeights(GameObject chunk, Vector3 centerPoint)
    {

        int x = (int)Mathf.Round(centerPoint.x);
        int y = (int)Mathf.Round(centerPoint.z);

        //Debug.Log(centerPoint.ToString());
        int minY = (int)(y - (TEXTURE_SIZE * ((float)spotLigh_tLight.cookieSize / 100))) + 1;
        int maxY = (int)(y + (TEXTURE_SIZE * ((float)spotLigh_tLight.cookieSize / 100)));

        int minX = (int)(x - (TEXTURE_SIZE * ((float)spotLigh_tLight.cookieSize / 100))) + 1;
        int maxX = (int)(x + (TEXTURE_SIZE * ((float)spotLigh_tLight.cookieSize / 100)));

        for (int i = minY; i < maxY; i++)
        {
            for (int j = minX; j < maxX; j++)
            {

                int editY = i;
                int editX = j;

                GameObject correctChunk = getCorrectChunk(chunk, ref editY, ref editX);
                if (correctChunk != null)
                {

                    int index = editY * TEXTURE_SIZE + editX;


                    int distZ = Mathf.Abs(y - Mathf.Abs(i));
                    int distX = Mathf.Abs(x - Mathf.Abs(j));
                    float dist = Mathf.Sqrt(Mathf.Pow(distZ, 2) + Mathf.Pow(distX, 2));
                    int maxDistZ = Mathf.Abs(y - Mathf.Abs(maxY));
                    int maxDistX = Mathf.Abs(x - Mathf.Abs(maxX));
                    float maxDist = Mathf.Sqrt(Mathf.Pow(maxDistZ, 2) + Mathf.Pow(maxDistX, 2));
                    //Debug.Log("Dist: " + dist);


                    Mesh mesh = correctChunk.GetComponent<MeshFilter>().mesh;
                    List<Vector3> vertices = new List<Vector3>();
                    mesh.GetVertices(vertices);
                    try {
                        float height = vertices[index].y;
                        vertices[index] = new Vector3(vertices[index].x, height + ((maxDist - dist) / maxDist), vertices[index].z);

                    }catch(System.ArgumentOutOfRangeException ex)
                    {
                        Debug.Log(ex.Message+" "+editY +" "+editX);
                    }
                    mesh.SetVertices(vertices);
                }
            }
        }
        //chunk.GetComponent<MeshCollider>().sharedMesh = mesh;


    }

    private void editColor(GameObject chunk, Vector3 centerPoint)
    {
        /*
        Debug.Log("TextureSize: " + textureSize);
        Debug.Log(centerPoint.ToString());
        Debug.Log(Vector3.Scale(centerPoint, new Vector3(textureSize,textureSize,textureSize)).ToString());
        Debug.Log("Magic Number:" + (textureSize * (tailleCookie / 100)));
        Debug.Log("min: " + (int)(centerPoint.x - (textureSize * ((float)light.cookieSize / 100))) + "\tmax: " + (int)(centerPoint.x + (textureSize * ((float)light.cookieSize / 100))));
        */

        List<Texture2D> modifiedTextures = new List<Texture2D>();

        int x = (int)Mathf.Round(centerPoint.x);
        int y = (int)Mathf.Round(centerPoint.z);

        //Debug.Log(centerPoint.ToString());
        int minY = (int)(y - (TEXTURE_SIZE * ((float)spotLigh_tLight.cookieSize / 100)))+1;
        int maxY = (int)(y + (TEXTURE_SIZE * ((float)spotLigh_tLight.cookieSize / 100)));

        int minX = (int)(x - (TEXTURE_SIZE * ((float)spotLigh_tLight.cookieSize / 100)))+1;
        int maxX = (int)(x + (TEXTURE_SIZE * ((float)spotLigh_tLight.cookieSize / 100)));

        for (int i = minY; i <= maxY; i++)
        {
            for (int j = minX; j <= maxX; j++)
            {
                int editY = i;
                int editX = j;


                GameObject correctChunk = getCorrectChunk(chunk, ref editY, ref editX);
                if (correctChunk != null)
                {

                    int distY = Mathf.Abs(y - Mathf.Abs(i));
                    int distX = Mathf.Abs(x - Mathf.Abs(j));
                    float dist = Mathf.Sqrt(Mathf.Pow(distY, 2) + Mathf.Pow(distX, 2));
                    int maxDistZ = Mathf.Abs(y - Mathf.Abs(maxY));
                    int maxDistX = Mathf.Abs(x - Mathf.Abs(maxX));
                    float maxDist = Mathf.Sqrt(Mathf.Pow(maxDistZ, 2) + Mathf.Pow(maxDistX, 2));
                    //Debug.Log("y: " + i + "\tx: " + j + " =>" + dist);


                    //Debug.Log("R: " + 255 * dist / light.cookieSize);
                    Color c = new Color(dist / maxDist, (maxDist - dist) / maxDist, 0);

                    Texture2D texture = correctChunk.GetComponent<Renderer>().material.mainTexture as Texture2D;

                    texture.SetPixel(editX, editY, c);

                    if (!modifiedTextures.Contains(texture))
                    {
                        modifiedTextures.Add(texture);
                    }
                }
            }
        }

        foreach(Texture2D texture in modifiedTextures)
        {
            texture.Apply();
        }
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
            else
            {
                c = new Color((float)238 / 255, (float)229 / 255, (float)218 / 255);
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

    private GameObject getCorrectChunk(GameObject chunk, ref int y, ref int x)
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
