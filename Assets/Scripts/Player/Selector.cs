using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour {

    private GameObject SelectedUnit;
    private GameObject SelectedBuilding;
    private Player player;
    private Transform selectionBox;

    private float indicatorFoV;

    private void Awake()
    {
        player = GetComponent<Player>();
        GameObject unitIndicatorPrefab = Resources.Load<GameObject>("Selection/UnitIndicator");
        selectionBox = Instantiate(unitIndicatorPrefab).transform;
    }

    private void Update()
    {
        InputHandler input = player.input;
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 clickPos = Input.mousePosition;
            Camera camera = GetComponentInChildren<Camera>();
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 100, 1 << LayerMask.NameToLayer("Unit"));

            if (hit.collider != null)
            {
                SelectedUnit = hit.collider.gameObject;
                EventListener.UnitSelected(SelectedUnit);
                SelectedBuilding = null;
            }
            else
            {
                Physics.Raycast(ray, out hit, 100, 1 << LayerMask.NameToLayer("Building"));
                if (hit.collider != null)
                {
                    SelectedBuilding = hit.collider.gameObject;
                    EventListener.UnitSelected(SelectedBuilding);
                    SelectedUnit = null;
                }
            }

        }
        
        if (SelectedBuilding == null && SelectedUnit == null)
        {
            selectionBox.position = new Vector3(0, -1, 0);
            return;
        }

        DrawRingAroundUnit();
        DrawRingAroundBuilding();
    }

    private void DrawRingAroundUnit()
    {
        if (SelectedUnit == null) return;

        selectionBox.position = new Vector3(SelectedUnit.transform.position.x, 3f, SelectedUnit.transform.position.z);
        selectionBox.Rotate(Vector3.forward * Time.deltaTime * 30f);
        selectionBox.GetComponent<Projector>().fieldOfView = 25f;
    }

    private void DrawRingAroundBuilding()
    {
        if (SelectedBuilding == null) return;

        selectionBox.position = new Vector3(SelectedBuilding.transform.position.x, 4f, SelectedBuilding.transform.position.z);
        selectionBox.GetComponent<Projector>().fieldOfView = 57.95f;
        selectionBox.Rotate(Vector3.forward * Time.deltaTime * 30f);
    }

}
