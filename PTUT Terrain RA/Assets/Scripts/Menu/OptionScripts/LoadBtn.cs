using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadBtn : MonoBehaviour
{
    private EditTerrain editor;
    // Start is called before the first frame update
    void Start()
    {
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
                    editor.loadTerrain();
                }
            }
        }
    }
}
