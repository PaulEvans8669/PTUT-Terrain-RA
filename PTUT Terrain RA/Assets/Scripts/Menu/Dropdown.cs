using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropdown : MonoBehaviour
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

            if (Input.GetMouseButtonDown(0) && (collider.gameObject).Equals(GameObject.Find("Menu").gameObject))
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

        GameObject shovel = GameObject.Find("Menu").transform.GetChild(0).gameObject;
        shovel.transform.localPosition = new Vector3(-1.1f, 0f, 0f);

        GameObject brush = GameObject.Find("Menu").transform.GetChild(1).gameObject;
        brush.transform.localPosition = new Vector3(-2.2f, 0f, 0f);

        GameObject vegetation = GameObject.Find("vegetation").gameObject;
        vegetation.transform.localPosition = new Vector3(-3.3f, 0f, 0f);

        GameObject save = GameObject.Find("Save").gameObject;
        save.transform.localPosition = new Vector3(-4.4f, 0f, 0f);

        GameObject newbutton = GameObject.Find("New").gameObject;
        newbutton.transform.localPosition = new Vector3(-4.4f, 0f, 0f);

        dropped = true;

    }

    public void close()
    {

        if (transform.GetChild(0).GetComponent<DropShovel>().getDropped())
        {
            transform.GetChild(0).GetComponent<DropShovel>().close();
            transform.GetChild(0).GetComponent<DropShovel>().setDropped(false);
        }

        if (transform.GetChild(1).GetComponent<DropBrush>().getDropped())
        {
            transform.GetChild(1).GetComponent<DropBrush>().close();
            transform.GetChild(1).GetComponent<DropBrush>().setDropped(false);
        }

        if (transform.GetChild(3).GetComponent<DropVeg>().getDropped())
        {
            transform.GetChild(3).GetComponent<DropVeg>().close();
            transform.GetChild(3).GetComponent<DropVeg>().setDropped(false);
        }

        GameObject shovel = GameObject.Find("Menu").transform.GetChild(0).gameObject;
        shovel.transform.localPosition = new Vector3(0f, 0f, 5f);

        GameObject brush = GameObject.Find("Menu").transform.GetChild(1).gameObject;
        brush.transform.localPosition = new Vector3(0f, 0f, 30f);

        GameObject vegetation = GameObject.Find("vegetation").gameObject;
        vegetation.transform.localPosition = new Vector3(0f, 0f, 40f);

        GameObject save = GameObject.Find("Save").gameObject;
        save.transform.localPosition = new Vector3(0f, 0f, 50f);

        GameObject newbutton = GameObject.Find("New").gameObject;
        newbutton.transform.localPosition = new Vector3(0f, 0f, 0f);

        dropped = false;

    }

}
