using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTerrain : MonoBehaviour {

    private GenerateChunk chunkGenerator;
    private int CHUNK_SIZE;
    private int TEXTURE_SIZE;
    public int terrainSize;
    private GameObject modelChunk;
    
    public int getChunkSize()
    {
        return CHUNK_SIZE;
    }

    public int getTerrainSize()
    {
        return terrainSize;
    }

    public int getTextureSize()
    {
        return TEXTURE_SIZE;
    }
	// Use this for initialization
	void Start () {
        CHUNK_SIZE = 25;
        TEXTURE_SIZE = CHUNK_SIZE+1;
        chunkGenerator = transform.root.gameObject.GetComponent<GenerateChunk>();
        modelChunk = GameObject.Find("ModelChunk");

        for (int z = 0; z < terrainSize; z++) {
            for (int x = 0; x < terrainSize; x++)
            {
                addNewChunk(z,  x);
            }
        }
        modelChunk.SetActive(false);
		
	}

    private void addNewChunk(int z, int x)
    {
        

        GameObject newChunk = Instantiate(modelChunk, new Vector3(x * CHUNK_SIZE, 0, -z * CHUNK_SIZE), Quaternion.identity, this.gameObject.transform);
        newChunk.name = "Chunk " + (z * terrainSize + x);
        newChunk.AddComponent<GenerateChunk>();
        Texture2D texture = new Texture2D(TEXTURE_SIZE, TEXTURE_SIZE);
        texture.filterMode = FilterMode.Point;
        newChunk.GetComponent<Renderer>().material.mainTexture = texture;
        newChunk.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2((float)0.0001, (float)0.0001));
        texture = newChunk.GetComponent<Renderer>().material.mainTexture as Texture2D;
        for (int textureY = 0; textureY <= TEXTURE_SIZE; textureY++)
        {
            for (int textureX = 0; textureX <= TEXTURE_SIZE; textureX++)
            {
                texture.SetPixel(textureX, textureY, new Color((float)113 / 255, (float)125 / 255, (float)45 / 255));
            }
        }
        texture.Apply();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
