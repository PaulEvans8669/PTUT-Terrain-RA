using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTerrain : MonoBehaviour {

    private int CHUNK_SIZE = 32;
    private int TEXTURE_SIZE;
    public int TERRAIN_SIZE = 1;
    private GameObject modelChunk;
    private GameObject arrLeft, arrRight, arrUp, arrDown, cubeMenu;
    private Texture2D texture;

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

        arrLeft = transform.GetChild(1).gameObject;
        arrLeft.transform.position = new Vector3(transform.localScale.x * - 10f, 0f, transform.localScale.z  * - 48.1f);

        arrRight = transform.GetChild(2).gameObject;
        arrRight.transform.position = new Vector3(transform.localScale.x * 136f, 0f, transform.localScale.z * -16.1f);

        arrUp = transform.GetChild(3).gameObject;
        arrUp.transform.position = new Vector3(transform.localScale.x * 47.9f, 0f, transform.localScale.z * 40f);

        arrDown = transform.GetChild(4).gameObject;
        arrDown.transform.position = new Vector3(transform.localScale.x * 80.1f, 0f, transform.localScale.z * -105.5f);

        for (int z = 0; z < TERRAIN_SIZE; z++) {
            for (int x = 0; x < TERRAIN_SIZE; x++)
            {
                addNewChunk(z,  x);
            }
        }
        modelChunk.SetActive(false);
        
        GameObject waterPlane = GameObject.Find("Plane");
        waterPlane.transform.localPosition = new Vector3(0, -1, 0);
        waterPlane.transform.localScale = new Vector3(TERRAIN_SIZE, 1, TERRAIN_SIZE);

    }

    private void addNewChunk(int z, int x)
    {
        GameObject newChunk = Instantiate(modelChunk, new Vector3((x * CHUNK_SIZE) - (TERRAIN_SIZE * CHUNK_SIZE) / 2, 0, -z * CHUNK_SIZE + ((TERRAIN_SIZE-1) * CHUNK_SIZE) / 2 - CHUNK_SIZE/2), Quaternion.identity, this.gameObject.transform);
        newChunk.name = "Chunk " + (z * TERRAIN_SIZE + x);
        newChunk.AddComponent<GenerateChunk>();
        chunkList.Add(newChunk);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
