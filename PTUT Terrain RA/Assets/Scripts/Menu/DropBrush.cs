using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class DropBrush : MonoBehaviour
{
    private bool dropped;
    private List<string> textures = new List<string>();
    private string texturePath;

    private Camera camera;
    // Start is called before the first frame update
    void Start()
    {

        camera = Camera.main;
        dropped = false;
        texturePath = "Assets/Resources/Textures/";
        loadTextures();
        transform.Find("BrushUp").GetComponent<BrushSelection>().setSize(textures.Count);
        transform.Find("BrushDown").GetComponent<BrushSelection>().setSize(textures.Count);

    }

    // Update is called once per frame
    void Update()
    {

        RaycastHit hitInfo = new RaycastHit();

        if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hitInfo))
        {

            Collider collider = hitInfo.collider;

            if (Input.GetMouseButtonDown(0) && (collider.gameObject).Equals(GameObject.Find("Brush").gameObject))
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

    public void loadTextures()
    {

        try
        {

            DirectoryInfo Dir = new DirectoryInfo(texturePath);
            FileInfo[] FileList = Dir.GetFiles("*.tga", SearchOption.AllDirectories);

            foreach (FileInfo FI in FileList)
            {
                textures.Add(FI.FullName);
            }

        } catch(System.Exception ex)
        {
            Debug.Log(ex);
        }

    }

    public string getTexture(int i)
    {

        return textures[i];

    }

    public int getTexturesSize()
    {
        return textures.Count;
    }

    public void setTexture(string texture)
    {

        Texture2D tmpTexture = TGALoader.LoadTGA(texture);
        GetComponent<Renderer>().material.mainTexture = tmpTexture;

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

        GameObject brushUp = GameObject.Find("BrushUp").gameObject;
        brushUp.transform.localPosition = new Vector3(0f, 1.1f, 0f);

        GameObject brushDown = GameObject.Find("BrushDown").gameObject;
        brushDown.transform.localPosition = new Vector3(0f, -1.1f, 0f);

        dropped = true;

    }

    public void close()
    {

        GameObject brushUp = GameObject.Find("BrushUp").gameObject;
        brushUp.transform.localPosition = new Vector3(0f, 0f, 5f);

        GameObject brushDown = GameObject.Find("BrushDown").gameObject;
        brushDown.transform.localPosition = new Vector3(0f, 0f, 10f);

        dropped = false;

    }


}
