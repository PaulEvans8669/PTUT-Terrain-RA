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
    public List<int> afficheChunk;
    public List<GameObject> chunkList2;

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

    public List<int> getAfficheChunk()
    {
        return afficheChunk;
    }

    // Use this for initialization
    void Start () {
        TEXTURE_SIZE = 256;
        modelChunk = GameObject.Find("ModelChunk");
        chunkList = new List<GameObject>();
        afficheChunk = new List<int>();

        arrLeft = GameObject.Find("Terrain").transform.GetChild(1).gameObject;
        arrLeft.transform.position = new Vector3(-10f, 0f, -48.1f);

        arrRight = GameObject.Find("Terrain").transform.GetChild(2).gameObject;
        arrRight.transform.position = new Vector3(136f, 0f, -16.1f);

        arrUp = GameObject.Find("Terrain").transform.GetChild(3).gameObject;
        arrUp.transform.position = new Vector3(47.9f, 0f, 40f);

        arrDown = GameObject.Find("Terrain").transform.GetChild(4).gameObject;
        arrDown.transform.position = new Vector3(80.1f, 0f, -105.5f);

        for (int z = 0; z < TERRAIN_SIZE; z++) {
            for (int x = 0; x < TERRAIN_SIZE; x++)
            {
                addNewChunk(z,  x);
            }
        }
        modelChunk.SetActive(false);

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                afficheChunk.Add(TERRAIN_SIZE * i + j);
                chunkList[TERRAIN_SIZE * i + j].SetActive(true);
            }
        }

        chunkList2 = chunkList;
        /*
        for (int i = 0; i < afficheChunk.Count; i++)
        {
            Debug.Log(afficheChunk[i]);
        }
        */

    }

    private void addNewChunk(int z, int x)
    {
        GameObject newChunk = Instantiate(modelChunk, new Vector3(x * CHUNK_SIZE, 0, -z * CHUNK_SIZE), Quaternion.identity, this.gameObject.transform);
        newChunk.name = "Chunk " + (z * TERRAIN_SIZE + x);
        newChunk.AddComponent<GenerateChunk>();
        chunkList.Add(newChunk);
        newChunk.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
