using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour {

    private GameObject SelectedUnit;
    private Player player;
    private Transform selectionBox;

    private void Awake()
    {
        player = GetComponent<Player>();
        selectionBox = Instantiate(Resources.Load<GameObject>("Selection/Cylinder")).transform;
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
            }
        }
        
        DrawRingAroundUnit();
    }

    private void DrawRingAroundUnit()
    {
        if (SelectedUnit != null)
        {
            selectionBox.position = new Vector3(SelectedUnit.transform.position.x, 0f, SelectedUnit.transform.position.z);
        }
        else
        {
            selectionBox.position = new Vector3(0, -1, 0);
        }
    }

}
