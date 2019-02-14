using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chargerTerrainDroit : MonoBehaviour
{

    private Camera camera;
    private List<GameObject> chunkList;
    private List<int> listeChunk;
    private int CHUNK_SIZE;
    private int TERRAIN_SIZE;
    private bool recalculEnCours;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        chunkList = GameObject.Find("Terrain").gameObject.GetComponent<GenerateTerrain>().getChunkList();
        listeChunk = GameObject.Find("Terrain").gameObject.GetComponent<GenerateTerrain>().getAfficheChunk();
        CHUNK_SIZE = GameObject.Find("Terrain").gameObject.GetComponent<GenerateTerrain>().getChunkSize();
        TERRAIN_SIZE =GameObject.Find("Terrain").gameObject.GetComponent<GenerateTerrain>().getTerrainSize();

        Debug.Log(GameObject.Find("Terrain").gameObject.GetComponent<GenerateTerrain>().getChunkList());
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitInfo = new RaycastHit();

        if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hitInfo))
        {

            Collider collider = hitInfo.collider;

            if (Input.GetMouseButtonDown(0) && collider.gameObject.Equals(GameObject.Find("ArrRight").gameObject) && recalculEnCours==false)
            {
                recalculEnCours = true;
                Debug.Log("Fleche Droite");

                if (chunkList == null || listeChunk == null)
                {
                    listeChunk = GameObject.Find("Terrain").gameObject.GetComponent<GenerateTerrain>().getAfficheChunk();
                    chunkList = GameObject.Find("Terrain").gameObject.GetComponent<GenerateTerrain>().getChunkList();
                }
                recalculateTerrainDroit();
            }

            if (Input.GetMouseButtonDown(0) && collider.gameObject.Equals(GameObject.Find("ArrLeft").gameObject) && recalculEnCours == false)
            {
                recalculEnCours = true;
                Debug.Log("Fleche Gauche");

                if (chunkList == null || listeChunk == null)
                {
                    listeChunk = GameObject.Find("Terrain").gameObject.GetComponent<GenerateTerrain>().getAfficheChunk();
                    chunkList = GameObject.Find("Terrain").gameObject.GetComponent<GenerateTerrain>().getChunkList();
                }
                recalculateTerrainGauche();
            }

            if (Input.GetMouseButtonDown(0) && collider.gameObject.Equals(GameObject.Find("ArrUp").gameObject) && recalculEnCours == false)
            {
                recalculEnCours = true;
                Debug.Log("Fleche Haut");

                if (chunkList == null || listeChunk == null)
                {
                    listeChunk = GameObject.Find("Terrain").gameObject.GetComponent<GenerateTerrain>().getAfficheChunk();
                    chunkList = GameObject.Find("Terrain").gameObject.GetComponent<GenerateTerrain>().getChunkList();
                }
                recalculateTerrainHaut();
            }

            if (Input.GetMouseButtonDown(0) && collider.gameObject.Equals(GameObject.Find("ArrDown").gameObject) && recalculEnCours == false)
            {
                recalculEnCours = true;
                Debug.Log("Fleche Bas");

                if (chunkList == null || listeChunk == null)
                {
                    listeChunk = GameObject.Find("Terrain").gameObject.GetComponent<GenerateTerrain>().getAfficheChunk();
                    chunkList = GameObject.Find("Terrain").gameObject.GetComponent<GenerateTerrain>().getChunkList();
                }
                recalculateTerrainBas();
            }


        }
        recalculEnCours = false;
    }

    private void recalculateTerrainDroit()
    {
        if (verifTerrain("d"))
        {
            for (int i = 0; i < chunkList.Count; i++)
            {

                GameObject chunk = chunkList[i];
                Vector3 position = chunk.transform.position;
                position.x -= CHUNK_SIZE;
                chunk.transform.position = position;

            }

            for (int i = 0; i < listeChunk.Count; i++)
            {

                chunkList[listeChunk[i]].SetActive(false);
                listeChunk[i] += 1;

            }

            for (int i = 0; i < listeChunk.Count; i++)
            {

                chunkList[listeChunk[i]].SetActive(true);

            }
        }
    }

    private void recalculateTerrainBas()
    {
        if (verifTerrain("b"))
        {
            for (int i = 0; i < chunkList.Count; i++)
            {

                GameObject chunk = chunkList[i];
                Vector3 position = chunk.transform.position;
                position.z += CHUNK_SIZE;
                chunk.transform.position = position;

            }

            for (int i = 0; i < listeChunk.Count; i++)
            {

                chunkList[listeChunk[i]].SetActive(false);
                listeChunk[i] += TERRAIN_SIZE;

            }

            for (int i = 0; i < listeChunk.Count; i++)
            {

                chunkList[listeChunk[i]].SetActive(true);

            }
        }
    }

    private void recalculateTerrainGauche()
    {
        if (verifTerrain("g"))
        {
            for (int i = 0; i < chunkList.Count; i++)
            {

                GameObject chunk = chunkList[i];
                Vector3 position = chunk.transform.position;
                position.x += CHUNK_SIZE;
                chunk.transform.position = position;

            }

            for (int i = 0; i < listeChunk.Count; i++)
            {

                chunkList[listeChunk[i]].SetActive(false);
                listeChunk[i] -= 1;

            }

            for (int i = 0; i < listeChunk.Count; i++)
            {

                chunkList[listeChunk[i]].SetActive(true);

            }
        }
    }

    private void recalculateTerrainHaut()
    {
        if (verifTerrain("h"))
        {
            for (int i = 0; i < chunkList.Count; i++)
            {

                GameObject chunk = chunkList[i];
                Vector3 position = chunk.transform.position;
                position.z -= CHUNK_SIZE;
                chunk.transform.position = position;

            }

            for (int i = 0; i < listeChunk.Count; i++)
            {

                chunkList[listeChunk[i]].SetActive(false);
                listeChunk[i] -= TERRAIN_SIZE;

            }

            for (int i = 0; i < listeChunk.Count; i++)
            {

                chunkList[listeChunk[i]].SetActive(true);

            }
        }
    }

    private bool verifTerrain(string s)
    {
        switch (s)
        {
            case "h":
                for (int i = 0; i < listeChunk.Count; i++)
                {
                    if ((listeChunk[i] - TERRAIN_SIZE)<0)
                    {
                        return false;
                    }
                }
                return true;
            case "b":
                for (int i = 0; i < listeChunk.Count; i++)
                {
                    if ((listeChunk[i] + +TERRAIN_SIZE) > chunkList.Count)
                    {
                        return false;
                    }
                }
                return true;
            case "d":
                if ((listeChunk[3]) % TERRAIN_SIZE == TERRAIN_SIZE-1)
                {
                    return false;
                }
                return true;
            case "g":
                if ((listeChunk[0])%TERRAIN_SIZE == 0)
                {
                    return false;
                }
                return true;
            default:
                return false;
        }
    }
}
