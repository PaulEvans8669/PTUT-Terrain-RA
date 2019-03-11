using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPalette : MonoBehaviour
{

    private bool dropped;

    private Camera camera;
    // Start is called before the first frame update
    void Start()
    {

        camera = Camera.main;
        dropped = false;

    }

    // Update is called once per frame
    void Update()
    {

        RaycastHit hitInfo = new RaycastHit();

        if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hitInfo))
        {

            Collider collider = hitInfo.collider;

            if (Input.GetMouseButtonDown(0) && (collider.gameObject).Equals(GameObject.Find("Palette").gameObject))
            {

                if (dropped)
                {
                    close();
                }
                else
                {
                    open();
                }

            }

        }

    }

    public bool getDropped()
    {

        return dropped;

    }

    public void open()
    {

        GameObject shovel = GameObject.Find("Shovel").gameObject;
        GameObject brush = GameObject.Find("Brush").gameObject;
        GameObject vegetation = GameObject.Find("Vegetation").gameObject;

        GameObject.Find("Menu").GetComponent<Dropdown>().closeFile();

        shovel.transform.localPosition = new Vector3(-1.1f, 0f, 0f);
        brush.transform.localPosition = new Vector3(-2.2f, 0f, 0f);
        vegetation.transform.localPosition = new Vector3(-3.3f, 0f, 0f);

        dropped = true;

    }

    public void close()
    {

        GameObject shovel = GameObject.Find("Shovel").gameObject;
        GameObject brush = GameObject.Find("Brush").gameObject;
        GameObject vegetation = GameObject.Find("Vegetation").gameObject;

        GameObject.Find("Menu").GetComponent<Dropdown>().openFile();

        shovel.transform.localPosition = new Vector3(0f, 0f, 5f);
        brush.transform.localPosition = new Vector3(0f, 0f, 10f);
        vegetation.transform.localPosition = new Vector3(0f, 0f, 20f);

        dropped = false;

    }

}
