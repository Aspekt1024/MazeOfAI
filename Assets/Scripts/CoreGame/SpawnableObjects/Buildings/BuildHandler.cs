using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ActionInput = InputHandler.ActionInput;

public class BuildHandler : MonoBehaviour {

    private bool buildMode;
    private bool placingBuilding;

    private Player player;
    private Camera playerCam;
    private Transform building;
    private Building buildingScript;
    private GameObject buildingPrefab;
    private PlacementGrid placementGrid;
    
    private void Awake() {

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerCam = player.GetComponentInChildren<Camera>();
        placementGrid = gameObject.AddComponent<PlacementGrid>();
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
                building.transform.position = Positioning.GetSnappedPosition(hit.point);
                if (buildingScript.GridSizeX == 2 && buildingScript.GridSizeY == 2)
                {
                    building.transform.position -= new Vector3(0.5f, 0f, 0.5f);
                }
            }

            if (Input.GetMouseButtonDown(0) && placementGrid.IsPlacementValid())
            {
                DeployBuilding();
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

        ExitBuildMode();
        Destroy(building.gameObject);
    }

    private void ExitBuildMode()
    {
        buildMode = false;
        placingBuilding = false;
        Cursor.visible = true;
        placementGrid.Disable();
    }

    private void CheckInputs()
    {
        if (placingBuilding) return;
        
        if (!GetBuildingPrefab()) return;
        
        GameObject buildingParent = GameObject.Find("Buildings");
        Camera camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();  // Could use Camera.main
        if (camera == null) return;

        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, 1000, 1 << LayerMask.NameToLayer("Terrain"));

        if (hit.collider == null) return;

        Cursor.visible = false;
        placingBuilding = true;
        building = Instantiate(buildingPrefab, hit.point, Quaternion.identity, buildingParent.transform).transform;
        buildingScript = building.GetComponent<Building>();
        placementGrid.SetNewBounds(building.gameObject);
        ColourBuilding();
    }

    private bool GetBuildingPrefab()
    {
        bool foundPrefab = false;
        string building = string.Empty;

        if (player.input.ActionPressed(ActionInput.BuildSpring))
            building = "CapsuleSpring";
        if (player.input.ActionPressed(ActionInput.BuildFacility))
            building = "Facility";
        if (player.input.ActionPressed(ActionInput.BuildTurret))
            building = "Turret";

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
                    if (t.GetComponent<Renderer>() != null)
                        t.GetComponent<Renderer>().material = Resources.Load<Material>("Textures/Unbuilt");
                }
                Transform[] bodyTfs = tf.GetComponentsInChildren<Transform>();
                foreach (Transform t in bodyTfs)
                {
                    if (t.GetComponent<Renderer>() != null)
                        t.GetComponent<Renderer>().material = Resources.Load<Material>("Textures/Unbuilt");
                }
            }
        }
    }

    private void DeployBuilding()
    {
        placementGrid.PlaceBuilding();
        Vector3 pos = building.position;

        // Recreate the building to restore colours
        Destroy(building.gameObject);
        building = Instantiate(buildingPrefab, pos, Quaternion.identity, transform).transform;

        building.GetComponent<Building>().Deploy();
        ExitBuildMode();
    }

}
