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

            if (Input.GetMouseButtonDown(0) && (collider.gameObject).Equals(GameObject.Find("Shovel").gameObject))
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

        GameObject rayPlus = GameObject.Find("ShovelRayPlus").gameObject;
        GameObject rayMinus = GameObject.Find("ShovelRayMinus").gameObject;
        GameObject extrPlus = GameObject.Find("ShovelExtrPlus").gameObject;
        GameObject extrMinus = GameObject.Find("ShovelExtrMinus").gameObject;

        extrMinus.transform.localPosition = new Vector3(0f, 1.1f, 0f);
        extrPlus.transform.localPosition = new Vector3(0f, 2.2f, 0f);
        rayMinus.transform.localPosition = new Vector3(0f, -2.2f, 0f);
        rayPlus.transform.localPosition = new Vector3(0f, -1.1f, 0f);

        dropped = true;

    }

    public void close()
    {

        GameObject plus = GameObject.Find("ShovelRayPlus").gameObject;
        GameObject minus = GameObject.Find("ShovelRayMinus").gameObject;
        GameObject fplus = GameObject.Find("ShovelExtrPlus").gameObject;
        GameObject fminus = GameObject.Find("ShovelExtrMinus").gameObject;
        
        plus.transform.localPosition = new Vector3(0f, 0f, 5f);
        minus.transform.localPosition = new Vector3(0f, 0f, 10f);
        fplus.transform.localPosition = new Vector3(0f, 0f, 15f);
        fminus.transform.localPosition = new Vector3(0f, 0f, 20f);

        dropped = false;

    }

}