using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropFile : MonoBehaviour
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

            if (Input.GetMouseButtonDown(0) && (collider.gameObject).Equals(GameObject.Find("File").gameObject))
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

        GameObject save = GameObject.Find("Save").gameObject;
        GameObject neww = GameObject.Find("New").gameObject;
        GameObject load = GameObject.Find("Load").gameObject;
        GameObject palette = GameObject.Find("Palette").gameObject;

        palette.GetComponent<Dropdown>().closePalette();

        save.transform.localPosition = new Vector3(0f, 1.1f, 0f);
        neww.transform.localPosition = new Vector3(0f, -1.1f, 0f);
        load.transform.localPosition = new Vector3(0f, -2.2f, 0f);

        dropped = true;

    }

    public void close()
    {

        GameObject save = GameObject.Find("Save").gameObject;
        GameObject neww = GameObject.Find("New").gameObject;
        GameObject load = GameObject.Find("Load").gameObject;
        GameObject palette = GameObject.Find("Palette").gameObject;

        palette.GetComponent<Dropdown>().openPalette();

        save.transform.localPosition = new Vector3(0f, 0f, 5f);
        neww.transform.localPosition = new Vector3(0f, 0f, 10f);
        load.transform.localPosition = new Vector3(0f, 0f, 15f);

        dropped = false;

    }

}
