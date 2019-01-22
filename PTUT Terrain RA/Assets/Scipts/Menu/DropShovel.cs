using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropShovel : MonoBehaviour
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

            if (Input.GetMouseButtonDown(0) && (collider.gameObject).Equals(GameObject.Find("Menu").transform.GetChild(0).gameObject))
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

    public void setDropped(bool state)
    {

        dropped = state;

    }

    public void open()
    {

        Debug.Log("Dropped");

        GameObject plus = GameObject.Find("Menu").transform.GetChild(0).transform.GetChild(0).gameObject;
        plus.transform.localPosition = new Vector3(0f, -1.1f, 0f);

        GameObject minus = GameObject.Find("Menu").transform.GetChild(0).transform.GetChild(1).gameObject;
        minus.transform.localPosition = new Vector3(0f, -2.2f, 0f);

        GameObject fplus = GameObject.Find("Menu").transform.GetChild(0).transform.GetChild(2).gameObject;
        fplus.transform.localPosition = new Vector3(0f, 2.2f, 0f);

        GameObject fminus = GameObject.Find("Menu").transform.GetChild(0).transform.GetChild(3).gameObject;
        fminus.transform.localPosition = new Vector3(0f, 1.1f, 0f);

        dropped = true;

    }

    public void close()
    {

        Debug.Log("Closed");

        GameObject plus = GameObject.Find("Menu").transform.GetChild(0).transform.GetChild(0).gameObject;
        plus.transform.localPosition = new Vector3(0f, 0f, 5f);

        GameObject minus = GameObject.Find("Menu").transform.GetChild(0).transform.GetChild(1).gameObject;
        minus.transform.localPosition = new Vector3(0f, 0f, 10f);

        GameObject fplus = GameObject.Find("Menu").transform.GetChild(0).transform.GetChild(2).gameObject;
        fplus.transform.localPosition = new Vector3(0f, 0f, 15f);

        GameObject fminus = GameObject.Find("Menu").transform.GetChild(0).transform.GetChild(3).gameObject;
        fminus.transform.localPosition = new Vector3(0f, 0f, 20f);

        dropped = false;

    }

}