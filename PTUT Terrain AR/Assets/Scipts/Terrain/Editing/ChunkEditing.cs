using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public class ChunkEditing : MonoBehaviour {

    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    private Camera mainCamera;
    private Projector projector;
    private string localDirectory;
    private Material projectorMaterial;
    private Shader shader;


    // Use this for initialization
    void Start()
    {
        meshRenderer = this.gameObject.GetComponentInChildren<MeshRenderer>();
        meshCollider = this.gameObject.GetComponentInChildren<MeshCollider>();
        mainCamera = Camera.main;
        projector = new Projector();
        localDirectory = Directory.GetCurrentDirectory();
        projectorMaterial = new Material(shader);
        Texture2D tex = new Texture2D(1500,1500);
        projectorMaterial.SetTexture("_MainTex", new Texture(Directory.GetParent(localDirectory)+"/Images/brush1.mat"));
        projector.material = 

    }

    // Update is called once per frame
    void Update()
    {
        // Do a raycast into the world based on the user's
        // head position and orientation.
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        int chunkSize = 50;


        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hitInfo))
        {
            projector.enabled = false;
            // Move the cursor to the point where the raycast hit.
            //this.transform.position = hitInfo.point;

            Vector3 hitPoint = hitInfo.point;

            Vector3 coordHitMesh = meshCollider.transform.InverseTransformPoint(hitPoint);
            //Debug.Log("x: " + hitPoint.x + " y: " + hitPoint.y + " z: " + hitPoint.z);

            
        }
        else
        {
            projector.enabled = false;
        }
    }
}
