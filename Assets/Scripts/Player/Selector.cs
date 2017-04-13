using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour {

    private Selectable SelectedObj;
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
        GetMouseInput();
        DrawSelectionIndicator();
    }

    private void GetMouseInput()
    {
        InputHandler input = player.input;
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 clickPos = Input.mousePosition;
            Camera camera = GetComponentInChildren<Camera>();
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 100, 1 << LayerMask.NameToLayer("SelectionBox"));

            if (hit.collider == null) return;
            System.Type type = GetObjectType(hit.collider.transform);
            if (type == null || !type.Equals(typeof(Selectable))) return;
            EventListener.ObjectSelected(SelectedObj);
        }
    }

    private void DrawSelectionIndicator()
    {
        if (SelectedObj == null)
        {
            selectionBox.position = new Vector3(0, -1, 0);
        }
        else
        {
            selectionBox.position = new Vector3(SelectedObj.transform.position.x, 3f, SelectedObj.transform.position.z);
            selectionBox.Rotate(Vector3.forward * Time.deltaTime * 30f);
            selectionBox.GetComponent<Projector>().fieldOfView = SelectedObj.ObjRadius;
        }
    }

    private System.Type GetObjectType(Transform obj)
    {
        Selectable objScript = null;

        while (objScript == null)
        {
            objScript = obj.GetComponent<Selectable>();

            if (objScript != null)
            {
                SelectedObj = objScript;
                return typeof(Selectable);
            }

            if (obj == obj.root)
                break;

            obj = obj.transform.parent;
        }
        return null;
    }
}
