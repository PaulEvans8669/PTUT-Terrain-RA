using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditTerrain : MonoBehaviour {

    private int CHUNK_SIZE;

    private Camera mainCamera;

    private GameObject spotLight;
    private Light light;
    private Texture2D cookieTexture;
    
    private List<Vector3> vertices;

    public float tailleCookie = 10;
    // Use this for initialization
    void Start () {
        CHUNK_SIZE = this.gameObject.GetComponent<GenerateTerrain>().getChunkSize();

        mainCamera = Camera.main;
        spotLight = GameObject.Find("Spot Light");
        light = spotLight.GetComponent<Light>();
        cookieTexture = light.cookie as Texture2D;


        vertices = new List<Vector3>();
    }
	
	// Update is called once per frame
	void Update () {
        if (tailleCookie < 1 )
        {
            tailleCookie = 1;
        }
        else if(tailleCookie > 100)
        {
            tailleCookie = 100;
        }
        light.cookieSize = tailleCookie*CHUNK_SIZE/100;


        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hitInfo))
        {
            light.enabled = true;
            // Move the cursor to the point where the raycast hit.
            //this.transform.position = hitInfo.point;

            Vector3 hitPoint = hitInfo.point;
            Collider collider = hitInfo.collider;

            Vector3 coordHitMesh = collider.transform.InverseTransformPoint(hitPoint);
            Vector3 coordSpotLight = coordHitMesh;
            coordSpotLight.y += 100;
            spotLight.transform.position = coordSpotLight;
            //Debug.Log("x: " + hitPoint.x + " y: " + hitPoint.y + " z: " + hitPoint.z);

            if (Input.GetMouseButton(0))
            {
                Mesh mesh = collider.gameObject.GetComponent<MeshFilter>().mesh;
                
                modifyHeight(ref mesh, coordHitMesh);
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                mesh.RecalculateTangents();
                collider.gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
            }

        }
        else
        {
            light.enabled = false;
        }


    }

    private void modifyHeight(ref Mesh mesh, Vector3 centerPoint)
    {
        mesh.GetVertices(vertices);

        int textureSize = cookieTexture.height;

        for(int z = 0; z <tailleCookie; z++)
        {
            for (int x = 0; x < tailleCookie; x++)
            {
                float height = cookieTexture.GetPixel(textureSize*x/(int)tailleCookie, textureSize*z / (int)tailleCookie).a;
                int meshX = (int)centerPoint.x + x - (int)tailleCookie/2;
                int meshZ = (int)centerPoint.z + z - (int)tailleCookie/2;
                //Debug.Log("z: " + meshZ + "\tx: " + meshX +" => "+height);
                if(meshX>=0 && meshZ >=0 && meshX<=CHUNK_SIZE && meshZ<=CHUNK_SIZE) { 
                    int index = meshZ * (CHUNK_SIZE + 1) + meshX;
                    Vector3 vect = vertices[index];
                    vertices[index] = new Vector3(vect.x, vect.y + height, vect.z);
                }

            }
        }
        mesh.SetVertices(vertices);

    }
}
