using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public class ChunkEditing : MonoBehaviour {

    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    private Camera mainCamera;


    // Use this for initialization
    void Start()
    {
        meshRenderer = this.gameObject.GetComponentInChildren<MeshRenderer>();
        meshCollider = this.gameObject.GetComponentInChildren<MeshCollider>();
        mainCamera = Camera.main;


    }

    // Update is called once per frame
    void Update()
    {
        // Do a raycast into the world based on the user's
        // head position and orientation.
        var headPosition = mainCamera.transform.position;
        var gazeDirection = mainCamera.transform.forward;
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        int chunkSize = 50;


        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hitInfo))
        {
            //projector.enabled = false;
            // Move the cursor to the point where the raycast hit.
            //this.transform.position = hitInfo.point;

            Vector3 hitPoint = hitInfo.point;
            Collider collider = hitInfo.collider;

            Vector3 coordHitMesh = meshCollider.transform.InverseTransformPoint(hitPoint);
            //Debug.Log("x: " + hitPoint.x + " y: " + hitPoint.y + " z: " + hitPoint.z);

            
        }
        else
        {
            //projector.enabled = false;
        }
    }
}
