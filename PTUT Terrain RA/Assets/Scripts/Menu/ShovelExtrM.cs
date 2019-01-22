using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShovelExtrM : MonoBehaviour
{
    private Camera camera;
    // Start is called before the first frame update
    void Start()
    {

        camera = Camera.main;

    }

    // Update is called once per frame
    void Update()
    {

        RaycastHit hitInfo = new RaycastHit();

        if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hitInfo))
        {

            Collider collider = hitInfo.collider;

            if (Input.GetMouseButtonDown(0) && (collider.gameObject).Equals(GameObject.Find("Menu").transform.GetChild(0).transform.GetChild(3).gameObject))
            {

                minus();

            }

        }

    }

    public void minus()
    {

        if (GameObject.Find("Terrain").GetComponent<EditTerrain>().FORCE > -100)
        {
            GameObject.Find("Terrain").GetComponent<EditTerrain>().FORCE -= 10;
        }

    }
}
