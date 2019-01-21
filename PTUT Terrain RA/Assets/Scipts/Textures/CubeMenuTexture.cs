using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMenuTexture : MonoBehaviour
{

    private Texture2D texture;

    // Start is called before the first frame update
    void Start()
    {

        texture = (Texture2D) Resources.Load("/Assets/Images/crafting_table_front.png");
        GetComponent<Renderer>().material.mainTexture = texture;    
        texture.Apply();

    }

    // Update is called once per frame
    void Update()
    {



    }

    private void OnGUI()
    {
        
    }

}
