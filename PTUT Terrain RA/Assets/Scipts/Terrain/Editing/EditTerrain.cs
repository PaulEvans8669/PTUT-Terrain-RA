using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditTerrain : MonoBehaviour {

    private int CHUNK_SIZE;
    private int TEXTURE_SIZE;
    private int terrainSize;

    public Color coloooor;

    private Camera mainCamera;

    private GameObject spotLight;
    private Light light;
    private Texture2D cookieTexture;
    public int tailleCookie = 10;

    private List<Vector3> vertices;
    
    

    // Use this for initialization
    void Start () {
        CHUNK_SIZE = this.gameObject.GetComponent<GenerateTerrain>().getChunkSize();
        TEXTURE_SIZE = this.gameObject.GetComponent<GenerateTerrain>().getTextureSize();
        terrainSize = this.gameObject.GetComponent<GenerateTerrain>().getTerrainSize();

        mainCamera = Camera.main;
        spotLight = GameObject.Find("Spot Light");
        light = spotLight.GetComponent<Light>();
        cookieTexture = light.cookie as Texture2D;
        
        vertices = new List<Vector3>();
        
    }
	
	// Update is called once per frame
	void Update () {
        if (tailleCookie < 1 )
        {
            tailleCookie = 1;
        }
        else if(tailleCookie > 100)
        {
            tailleCookie = 100;
        }
        light.cookieSize = terrainSize*tailleCookie*CHUNK_SIZE/100;


        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hitInfo))
        {
            light.enabled = true;
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
                //editHeights(targetChunk, coordHitMesh);
                editColor(targetChunk, coordHitMesh);
                
            }

        }
        else
        {
            light.enabled = false;
        }


    }

    private void editHeights(GameObject chunk, Vector3 centerPoint)
    {
        MeshFilter MF = chunk.GetComponent<MeshFilter>();
        Mesh mesh = MF.mesh;
        mesh.GetVertices(vertices);

        int x = (int)Mathf.Round(centerPoint.x);
        int y = (int)Mathf.Round(centerPoint.z);

        //Debug.Log(centerPoint.ToString());
        int minY = (int)(y - (TEXTURE_SIZE * ((float)light.cookieSize / 100)) / 2) + 1;
        int maxY = (int)(y + (TEXTURE_SIZE * ((float)light.cookieSize / 100)) / 2);

        int minX = (int)(x - (TEXTURE_SIZE * ((float)light.cookieSize / 100)) / 2) + 1;
        int maxX = (int)(x + (TEXTURE_SIZE * ((float)light.cookieSize / 100)) / 2);

        for (int i = minY; i < maxY; i++)
        {
            for (int j = minX; j < maxX; j++)
            {

                int editY = i;
                int editX = j;

                GameObject correctChunk = getCorrectChunk(chunk, ref editY, ref editX);
                if (correctChunk != null)
                {

                    int index = editY * TEXTURE_SIZE + j;


                    int distZ = Mathf.Abs(y - Mathf.Abs(i));
                    int distX = Mathf.Abs(x - Mathf.Abs(j));
                    float dist = Mathf.Sqrt(Mathf.Pow(distZ, 2) + Mathf.Pow(distX, 2));
                    int maxDistZ = Mathf.Abs(y - Mathf.Abs(maxY));
                    int maxDistX = Mathf.Abs(x - Mathf.Abs(maxX));
                    float maxDist = Mathf.Sqrt(Mathf.Pow(maxDistZ, 2) + Mathf.Pow(maxDistX, 2));
                    //Debug.Log("Dist: " + dist);
                    float height = vertices[index].y;
                    vertices[index] = new Vector3(vertices[index].x, height + ((maxDist - dist) / maxDist), vertices[index].z);
                }

            }
        }
        mesh.SetVertices(vertices);
        //chunk.GetComponent<MeshCollider>().sharedMesh = mesh;


    }

    private void modifyHeight(ref Mesh mesh, Vector3 centerPoint)
    {


        

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
        int minY = (int)(y - (TEXTURE_SIZE * ((float)light.cookieSize / 100))/2)+1;
        int maxY = (int)(y + (TEXTURE_SIZE * ((float)light.cookieSize / 100))/2);

        int minX = (int)(x - (TEXTURE_SIZE * ((float)light.cookieSize / 100))/2)+1;
        int maxX = (int)(x + (TEXTURE_SIZE * ((float)light.cookieSize / 100))/2);

        for (int i = minY; i <= maxY; i++)
        {
            for (int j = minX; j <= maxX; j++)
            {
                int editY = i;
                int editX = j;

                int distY = Mathf.Abs(y - Mathf.Abs(i));
                int distX = Mathf.Abs(x - Mathf.Abs(j));
                float dist = Mathf.Sqrt(Mathf.Pow(distY, 2) + Mathf.Pow(distX, 2));
                int maxDistZ = Mathf.Abs(y - Mathf.Abs(maxY));
                int maxDistX = Mathf.Abs(x - Mathf.Abs(maxX));
                float maxDist = Mathf.Sqrt(Mathf.Pow(maxDistZ, 2) + Mathf.Pow(maxDistX, 2));
                //Debug.Log("y: " + i + "\tx: " + j + " =>" + dist);


                GameObject correctChunk = getCorrectChunk(chunk, ref editY, ref editX);
                if(correctChunk != null)
                {
                    //Debug.Log("R: " + 255 * dist / light.cookieSize);
                    Color c = new Color(dist/maxDist, (maxDist - dist) / maxDist, 0);

                    Texture2D texture = correctChunk.GetComponent<Renderer>().material.mainTexture as Texture2D;
                    if (editX >= 0 && editX <= TEXTURE_SIZE && editY >= 0 && editY <= TEXTURE_SIZE)
                    {
                        texture.SetPixel(editX, editY, c);
                    }

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
        if (idChunkOnTheLeft >= 0 && (int)(idChunkOnTheLeft/terrainSize) == (int)(id/terrainSize))
        {
            return GameObject.Find("Chunk " + idChunkOnTheLeft);
        }
        return null;
    }

    private GameObject getRightChunk(GameObject chunk)
    {
        int id = int.Parse(chunk.name.Split(' ')[1]);
        int idChunkOnTheRight = id + 1;
        if (idChunkOnTheRight < (1+id/terrainSize)*terrainSize)
        {
            return GameObject.Find("Chunk " + idChunkOnTheRight);
        }
        return null;
    }

    private GameObject getTopChunk(GameObject chunk)
    {
        int id = int.Parse(chunk.name.Split(' ')[1]);
        int idChunkAbove = id - terrainSize;
        if (idChunkAbove >= 0)
        {
            return GameObject.Find("Chunk " + idChunkAbove);
        }
        return null;
    }

    private GameObject getBottomChunk(GameObject chunk)
    {
        int id = int.Parse(chunk.name.Split(' ')[1]);
        int idChunkUnder = id + terrainSize;
        if (idChunkUnder < terrainSize*terrainSize)
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
