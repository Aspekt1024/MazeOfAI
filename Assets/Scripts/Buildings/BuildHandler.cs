using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ActionInput = InputHandler.ActionInput;

public class BuildHandler : MonoBehaviour {

    private bool buildMode;
    private bool placingBuilding;

    private Player player;
    private Camera playerCam;
    private Transform building;
    private GameObject buildingPrefab;
    
    private void Awake() {

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerCam = player.GetComponentInChildren<Camera>();
    }
	
	private void Update () {
        if (!buildMode) return;

        if (placingBuilding)
        {
            Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 1000, 1 << LayerMask.NameToLayer("Terrain"));

            if (hit.collider != null)
            {
                building.transform.position = GetSnappedPos(hit.point);
            }

            if (Input.GetMouseButtonDown(0))
            {
                buildMode = false;
                DeployBuilding(hit.point);
            }
        }
        else
        {
            CheckInputs();
        }
    }

    public void EnterBuildMode()
    {
        buildMode = true;
    }

    public void CancelBuild()
    {
        if (!placingBuilding) return;

        buildMode = false;
        placingBuilding = false;
        Destroy(building.gameObject);
    }

    private void CheckInputs()
    {
        if (placingBuilding) return;
        
        if (!GetBuildingPrefab()) return;
        
        GameObject buildingParent = GameObject.Find("Buildings");
        Camera camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        if (camera == null) return;

        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, 1000, 1 << LayerMask.NameToLayer("Terrain"));

        if (hit.collider == null) return;

        placingBuilding = true;
        building = Instantiate(buildingPrefab, hit.point, Quaternion.identity, buildingParent.transform).transform;
        ColourBuilding();
    }

    private bool GetBuildingPrefab()
    {
        bool foundPrefab = false;

        InputHandler input = player.input;
        string building = string.Empty;

        if (player.input.ActionPressed(ActionInput.BuildSpring))
            building = "CapsuleSpring";
        if (player.input.ActionPressed(ActionInput.BuildFacility))
            building = "Facility";

        buildingPrefab = Resources.Load<GameObject>("Buildings/" + building);
        if (buildingPrefab != null)
            foundPrefab = true;

        return foundPrefab;
    }

    private void ColourBuilding()
    {
        foreach (Transform tf in building.transform)
        {
            if (tf.name == "Body")
            {
                foreach (Transform t in tf)
                {
                    t.GetComponent<Renderer>().material = Resources.Load<Material>("Textures/Unbuilt");
                }
            }
        }
    }

    private void DeployBuilding(Vector3 point)
    {
        Destroy(building.gameObject);
        building = Instantiate(buildingPrefab, point, Quaternion.identity, transform).transform;
        
        buildMode = false;
        placingBuilding = false;
        GridEvents.PlaceBuilding();
    }

    private Vector3 GetSnappedPos(Vector3 pos)
    {
        // TODO snap
        return new Vector3(pos.x, 0f, pos.z);
    }
}
