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

    public void openPalette()
    {
        GameObject palette = GameObject.Find("Palette").gameObject;
        palette.transform.localPosition = new Vector3(-1.1f, 0f, 0f);
    }

    public void closePalette()
    {
        GameObject palette = GameObject.Find("Palette").gameObject;

        if (GameObject.Find("Palette").GetComponent<DropPalette>().getDropped())
        {
            GameObject.Find("Palette").GetComponent<DropPalette>().close();
        }

        palette.transform.localPosition = new Vector3(0f, 0f, 5f);
    }

    public void openFile()
    {
        GameObject file = GameObject.Find("File").gameObject;
        file.transform.localPosition = new Vector3(1.1f, 0f, 0f);
    }

    public void closeFile()
    {
        GameObject file = GameObject.Find("File").gameObject;

        if (GameObject.Find("File").GetComponent<DropFile>().getDropped())
        {
            GameObject.Find("File").GetComponent<DropFile>().close();
        }

        file.transform.localPosition = new Vector3(0f, 0f, 50f);
    }

    public void open()
    {

        openFile();
        openPalette();
        
        dropped = true;

    }

    public void close()
    {

        /*if (palette.GetComponent<>().getDropped)
        {

        }*/

        closeFile();
        closePalette();

        dropped = false;

    }

}
