using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegSelection : MonoBehaviour
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

            if (Input.GetMouseButtonDown(0) && (collider.gameObject).Equals(GameObject.Find("vegUp").gameObject))
            {

                if (cursor < size)
                {

                    transform.parent.GetComponent<DropVeg>().setModel(
                        Resources.Load(
                            transform.parent.GetComponent<DropVeg>().getModel(++cursor),
                            typeof(GameObject)
                        ) as GameObject
                    );

                    Debug.Log("passer par la");
                    GameObject.Find("Terrain").GetComponent<EditTerrain>().setModel(
                        Resources.Load(
                            transform.parent.GetComponent<DropVeg>().getModel(cursor)
                        ) as GameObject
                    );

                }

            }

            if (Input.GetMouseButtonDown(0) && (collider.gameObject).Equals(GameObject.Find("vegDown").gameObject))
            {

                if (cursor > 0)
                {

                    transform.parent.GetComponent<DropVeg>().setModel(
                        Instantiate(
                            Resources.Load(
                                transform.parent.GetComponent<DropVeg>().getModel(--cursor)
                            )
                        ) as GameObject
                    );

                    GameObject.Find("Terrain").GetComponent<EditTerrain>().setModel(
                        Instantiate(Resources.Load(
                            transform.parent.GetComponent<DropVeg>().getModel(cursor)
                        )) as GameObject
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
