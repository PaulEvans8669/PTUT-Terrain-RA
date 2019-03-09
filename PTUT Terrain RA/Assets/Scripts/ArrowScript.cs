using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    private static GameObject terrainGameObject;
    private int Direction { get; set; }
    // Start is called before the first frame update
    void Start()
    { 
        if(terrainGameObject == null)
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
        if(clickOnArrow() && terrainGameObject.GetComponent<EditTerrain>().MainTerrain != null)
        {
            Assets.Scripts.lib.Terrain MainTerrain = terrainGameObject.GetComponent<EditTerrain>().MainTerrain;
            switch (Direction)
            {
                case 0:
                    MainTerrain.moveNorth();
                    break;
                case 1:
                    MainTerrain.moveSouth();
                    break;
                case 2:
                    MainTerrain.moveWest();
                    break;
                case 3:
                    MainTerrain.moveEast();
                    break;
            }
            
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
}
