using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropLoad : MonoBehaviour
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

    }

    public bool getDropped()
    {

        return dropped;

    }

    public void open()
    {

        GameObject loadLeft = GameObject.Find("LoadLeft").gameObject;
        GameObject loadRight = GameObject.Find("LoadRight").gameObject;

        loadLeft.transform.localPosition = new Vector3(-1.1f, 0f, 0f);
        loadRight.transform.localPosition = new Vector3(1.1f, 0f, 0f);

        dropped = true;

    }

    public void close()
    {

        GameObject loadLeft = GameObject.Find("LoadLeft").gameObject;
        GameObject loadRight = GameObject.Find("LoadRight").gameObject;

        loadLeft.transform.localPosition = new Vector3(0f, 0f, 5f);
        loadRight.transform.localPosition = new Vector3(0f, 0f, 10f);

        dropped = false;

    }
}
