using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTerrain : MonoBehaviour {

    private GenerateChunk chunkGenerator;
    private int CHUNK_SIZE = 100;
    public int terrainSize;
    private GameObject firstChunk;
    
    public int getChunkSize()
    {
        return CHUNK_SIZE;
    }

	// Use this for initialization
	void Start () {
        chunkGenerator = transform.root.gameObject.GetComponent<GenerateChunk>();
        firstChunk = this.gameObject.transform.GetChild(1).gameObject;
        for (int z = 0; z < terrainSize; z++) {
            for (int x = 0; x < terrainSize; x++)
            {
                if (z + x != 0)
                {
                    GameObject newChunk = Instantiate(firstChunk);
                    newChunk.name = "Chunk " + (z * terrainSize + x);
                    newChunk.transform.Translate(new Vector3(x * CHUNK_SIZE, 0, -z * CHUNK_SIZE));
                    newChunk.transform.parent = this.gameObject.transform;
                }
            }
        }
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
