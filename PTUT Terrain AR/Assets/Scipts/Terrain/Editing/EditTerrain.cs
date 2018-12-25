using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditTerrain : MonoBehaviour {

    private Camera mainCamera;
    private GameObject spotLight;

    // Use this for initialization
    void Start () {
        mainCamera = Camera.main;
        spotLight = GameObject.Find("Spot Light");
    }
	
	// Update is called once per frame
	void Update () {
        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hitInfo))
        {
            //projector.enabled = false;
            // Move the cursor to the point where the raycast hit.
            //this.transform.position = hitInfo.point;

            Vector3 hitPoint = hitInfo.point;
            Collider collider = hitInfo.collider;

            Vector3 coordHitMesh = collider.transform.InverseTransformPoint(hitPoint);
            //Debug.Log("x: " + hitPoint.x + " y: " + hitPoint.y + " z: " + hitPoint.z);


        }
    }
}
