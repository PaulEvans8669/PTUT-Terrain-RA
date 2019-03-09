using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushSelection : MonoBehaviour
{
    
    private int size = 0;
    private int cursor;
    private EditTerrain editor;
    // Start is called before the first frame update
    void Start()
    {
        cursor = -1;
        editor = GameObject.Find("Terrain").GetComponent<EditTerrain>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hitInfo = new RaycastHit();

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
            {

                Collider collider = hitInfo.collider;

                if ((collider.gameObject).Equals(GameObject.Find("BrushUp").gameObject))
                {
                    editor.textureUp();
                }

                if ((collider.gameObject).Equals(GameObject.Find("BrushDown").gameObject))
                {
                    editor.textureDown();
                }

            }
        }

    }

    public void setSize(int max)
    {
        size = max;
    }

    public int getSize()
    {
        return size;
    }

    public void setCursor(int curs)
    {
        cursor = curs;
    }

    public int getCursor()
    {
        return cursor;
    }

}
