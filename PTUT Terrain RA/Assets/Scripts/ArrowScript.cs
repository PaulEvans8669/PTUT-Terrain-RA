using Assets.Scripts.lib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{

    private int CHUNK_SIZE = 32;

    private static GameObject terrainGameObject;
    private int Direction { get; set; }

    private Assets.Scripts.lib.Terrain MainTerrain;
    // Start is called before the first frame update
    void Start()
    {
       


        if (terrainGameObject == null)
        {
            terrainGameObject = this.transform.parent.gameObject;
        }
        if (this.transform.name.Contains("Up"))
        {
            Direction = 0;
        }
        else if (this.transform.name.Contains("Down"))
        {
            Direction = 1;
        }
        else if (this.transform.name.Contains("Left"))
        {
            Direction = 2;
        }
        else if (this.transform.name.Contains("Right"))
        {
            Direction = 3;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (MainTerrain == null && terrainGameObject.GetComponent<EditTerrain>().MainTerrain != null)
        {
            MainTerrain = terrainGameObject.GetComponent<EditTerrain>().MainTerrain;
        }

        if (clickOnArrow())
        {
            switch (Direction)
            {
                case 0:
                    recalculateTerrainHaut();
                    //MainTerrain.moveNorth();
                    break;
                case 1:
                    recalculateTerrainBas();
                    //MainTerrain.moveSouth();
                    break;
                case 2:
                    recalculateTerrainGauche();
                    //MainTerrain.moveWest();
                    break;
                case 3:
                    recalculateTerrainDroit();
                    //MainTerrain.moveEast();
                    break;
            }
            MainTerrain.recalculateVisibility();
        }
    }

    private bool clickOnArrow()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
            {
                if (hitInfo.collider.transform.name == this.transform.name)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void recalculateTerrainDroit()
    {
        if (verifTerrain("d"))
        {
            for (int i = 0; i < MainTerrain.ChunkList.Count; i++)
            {

                GameObject chunk = MainTerrain.ChunkList[i].ChunkGameObject;
                Vector3 lp = chunk.transform.localPosition;
                chunk.transform.localPosition = new Vector3(lp.x - CHUNK_SIZE, 0, lp.z);

            }

            for (int i = 0; i < MainTerrain.VisibleChunksId.Count; i++)
            {
                
                MainTerrain.VisibleChunksId[i] += 1;

            }
        }
    }

    private void recalculateTerrainBas()
    {
        if (verifTerrain("b"))
        {
            for (int i = 0; i < MainTerrain.ChunkList.Count; i++)
            {

                GameObject chunk = MainTerrain.ChunkList[i].ChunkGameObject;
                Vector3 lp = chunk.transform.localPosition;
                chunk.transform.localPosition = new Vector3(lp.x, 0, lp.z + CHUNK_SIZE);

            }

            for (int i = 0; i < MainTerrain.VisibleChunksId.Count; i++)
            {
                
                MainTerrain.VisibleChunksId[i] += MainTerrain.Size;

            }
        }
    }

    private void recalculateTerrainGauche()
    {
        if (verifTerrain("g"))
        {
            for (int i = 0; i < MainTerrain.ChunkList.Count; i++)
            {

                GameObject chunk = MainTerrain.ChunkList[i].ChunkGameObject;
                Vector3 lp = chunk.transform.localPosition;
                chunk.transform.localPosition = new Vector3(lp.x + CHUNK_SIZE, 0, lp.z);

            }

            for (int i = 0; i < MainTerrain.VisibleChunksId.Count; i++)
            {
                
                MainTerrain.VisibleChunksId[i] -= 1;

            }
            
        }
    }

    private void recalculateTerrainHaut()
    {
        if (verifTerrain("h"))
        {
            for (int i = 0; i < MainTerrain.ChunkList.Count; i++)
            {

                GameObject chunk = MainTerrain.ChunkList[i].ChunkGameObject;
                Vector3 lp = chunk.transform.localPosition;
                chunk.transform.localPosition = new Vector3(lp.x, 0, lp.z - CHUNK_SIZE);

            }

            for (int i = 0; i < MainTerrain.VisibleChunksId.Count; i++)
            {
                
                MainTerrain.VisibleChunksId[i] -= MainTerrain.Size;

            }
        }
    }

    private bool verifTerrain(string s)
    {
        switch (s)
        {
            case "h":
                for (int i = 0; i < MainTerrain.VisibleChunksId.Count; i++)
                {
                    if ((MainTerrain.VisibleChunksId[i] - MainTerrain.Size) < 0)
                    {
                        return false;
                    }
                }
                return true;
            case "b":
                for (int i = 0; i < MainTerrain.VisibleChunksId.Count; i++)
                {
                    if ((MainTerrain.VisibleChunksId[i] + MainTerrain.Size) > MainTerrain.ChunkList.Count)
                    {
                        return false;
                    }
                }
                return true;
            case "d":
                if ((MainTerrain.VisibleChunksId[3]) % MainTerrain.Size == MainTerrain.Size - 1)
                {
                    return false;
                }
                return true;
            case "g":
                if ((MainTerrain.VisibleChunksId[0]) % MainTerrain.Size == 0)
                {
                    return false;
                }
                return true;
            default:
                return false;
        }
    }
}
