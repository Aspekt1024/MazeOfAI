using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour {

    private Player player;
    private Camera playerCam;

    private bool inBuild;
    private Transform building;

    private void Awake()
    {
        player = GetComponent<Player>();
        playerCam = GetComponentInChildren<Camera>();
    }

	private void Update ()
    {

        if (inBuild)
        {
            Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 1000, 1 << LayerMask.NameToLayer("Terrain"));

            if (hit.collider != null)
                building.transform.position = hit.point;
                // TODO snap to grid
                
            if (Input.GetMouseButtonDown(0))
            {
                inBuild = false;
                DeployBuilding(hit.point);
            }
        }

        InputHandler input = player.input;
        if (input.ActionPressed(InputHandler.ActionInput.Cancel))
        {
            if (inBuild)
            {
                inBuild = false;
                Destroy(building.gameObject);
            }
        }

        if (input.ActionPressed(InputHandler.ActionInput.BuildSpring))
        {
            if (inBuild) return;    // Note: must return here to clear the input
            GameObject buildingParent = GameObject.Find("Buildings");
            Camera camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            if (camera == null) return;
            
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 1000, 1 << LayerMask.NameToLayer("Terrain"));

            if (hit.collider == null) return;

            inBuild = true;
            building = Instantiate(Resources.Load<GameObject>("Buildings/Facility"), hit.point, Quaternion.identity, buildingParent.transform).transform;
            ColourBuilding();
        }
    }

    private void ColourBuilding()
    {
        foreach(Transform tf in building.transform)
        {
            if (tf.name == "Body")
            {
                foreach(Transform t in tf)
                {
                    t.GetComponent<Renderer>().material = Resources.Load<Material>("Textures/Unbuilt");
                }
            }
        }
    }

    private void DeployBuilding(Vector3 point)
    {
        // TODO deploy held object using building script
        Destroy(building.gameObject);
        GameObject buildingParent = GameObject.Find("Buildings");
        building = Instantiate(Resources.Load<GameObject>("Buildings/Facility"), point, Quaternion.identity, buildingParent.transform).transform;
    }
}
