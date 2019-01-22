using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTerrain : MonoBehaviour {

    private int CHUNK_SIZE = 32;
    private int TEXTURE_SIZE;
    public int TERRAIN_SIZE = 1;
    private GameObject modelChunk;

    private List<GameObject> chunkList; //Sorted by id
    
    public int getChunkSize()
    {
        return CHUNK_SIZE;
    }

    public int getTerrainSize()
    {
        return TERRAIN_SIZE;
    }

    public int getTextureSize()
    {
        return TEXTURE_SIZE;
    }

    public List<GameObject> getChunkList()
    {
        return chunkList;
    }

	// Use this for initialization
	void Start () {
        TEXTURE_SIZE = 256;
        modelChunk = GameObject.Find("ModelChunk");
        chunkList = new List<GameObject>();

        for (int z = 0; z < TERRAIN_SIZE; z++) {
            for (int x = 0; x < TERRAIN_SIZE; x++)
            {
                addNewChunk(z,  x);
            }
        }
        modelChunk.SetActive(false);
		
	}

    private void addNewChunk(int z, int x)
    {
        GameObject newChunk = Instantiate(modelChunk, new Vector3(x * CHUNK_SIZE, 0, -z * CHUNK_SIZE), Quaternion.identity, this.gameObject.transform);
        newChunk.name = "Chunk " + (z * TERRAIN_SIZE + x);
        newChunk.AddComponent<GenerateChunk>();
        chunkList.Add(newChunk);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
