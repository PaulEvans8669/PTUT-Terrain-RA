using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushSelection : MonoBehaviour
{

    private Camera camera;
    private int size = 0;
    private int cursor;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        cursor = -1;
    }

    // Update is called once per frame
    void Update()
    {

        RaycastHit hitInfo = new RaycastHit();

        if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hitInfo))
        {

            Collider collider = hitInfo.collider;

            if (Input.GetMouseButtonDown(0) && (collider.gameObject).Equals(GameObject.Find("BrushUp").gameObject))
            {

                if (cursor < size)
                {

                    transform.parent.GetComponent<DropBrush>().setTexture(
                        transform.parent.GetComponent<DropBrush>().getTexture(++cursor)
                    );

                    GameObject.Find("Terrain").GetComponent<EditTerrain>().setTexture(
                        TGALoader.LoadTGA(
                            transform.parent.GetComponent<DropBrush>().getTexture(cursor)
                        )
                    );

                }

            }

            if (Input.GetMouseButtonDown(0) && (collider.gameObject).Equals(GameObject.Find("BrushDown").gameObject))
            {

                if (cursor > 0)
                {

                    transform.parent.GetComponent<DropBrush>().setTexture(
                        transform.parent.GetComponent<DropBrush>().getTexture(--cursor)
                    );

                    GameObject.Find("Terrain").GetComponent<EditTerrain>().setTexture(
                        TGALoader.LoadTGA(
                            transform.parent.GetComponent<DropBrush>().getTexture(cursor)
                        )
                    );

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
