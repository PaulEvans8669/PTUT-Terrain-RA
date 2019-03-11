using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateArrows : MonoBehaviour
{
    // Start is called before the first frame update
    private int type;
    private EditTerrain editor;
    // Start is called before the first frame update
    void Start()
    {
        if (this.gameObject.transform.name.Contains("Left"))
        {
            type = -1;
        }
        else
        {
            type = 1;
        }
        editor = GameObject.Find("Terrain").GetComponent<EditTerrain>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
            {
                Collider collider = hitInfo.collider;

                if ((collider.gameObject).Equals(this.gameObject))
                {
                    editor.TERRAIN_SIZE += type;
                }
            }
        }
    }
}
