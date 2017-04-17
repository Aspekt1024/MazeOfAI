using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Selector : MonoBehaviour {
    
    private const int maxSelectedObjects = 10;
    private int numObjectsSelected = 0;

    private Selectable[] SelectedObj;
    private Transform[] indicators;

    private bool mouseDown;
    private Vector2 mousePos;

    private void Awake()
    {
        SelectedObj = new Selectable[maxSelectedObjects];
        SetupIndicators();
    }

    private void Update()
    {
        GetMouseInput();
        DrawSelectionIndicator();
    }

    private void GetMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            mouseDown = true;
            mousePos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0) && mouseDown)
        {
            ClearSelections();
            mouseDown = false;

            Vector2 clickPos = Input.mousePosition;
            if (Vector2.Distance(mousePos, clickPos) < 0.2f)
            {
                SelectSingleObject(mousePos);
            }
            else
            {
                SelectObjectsInRange(mousePos, clickPos);
            }
        }
    }

    private void SelectSingleObject(Vector2 position)
    {
        Camera camera = GetComponentInChildren<Camera>();
        Ray ray = camera.ScreenPointToRay(position);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, 100, 1 << LayerMask.NameToLayer("SelectionBox"));

        if (hit.collider == null)
        {
            EventListener.Deselected();
        }
        else
        {
            SelectObject(0, hit.collider.transform);
            if (SelectedObj[0] != null)
                EventListener.ObjectSelected(SelectedObj[0]);
        }
    }

    private void SelectObjectsInRange(Vector2 startPos, Vector2 endPos)
    {
        Camera camera = GetComponentInChildren<Camera>();
        Ray ray = camera.ScreenPointToRay(startPos);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, 100, 1 << LayerMask.NameToLayer("Terrain"));

        EventListener.Deselected();
        if (hit.collider == null)
        {
            SelectSingleObject(startPos);
            return;
        }
        else
        {
            Vector3 worldPosStart = hit.point;
            ray = camera.ScreenPointToRay(endPos);
            Physics.Raycast(ray, out hit, 100, 1 << LayerMask.NameToLayer("Terrain"));
            if (hit.collider == null)
            {
                SelectSingleObject(endPos);
                return;
            }
            SelectAllInRange(worldPosStart, hit.point);
        }
    }

    private void SelectAllInRange(Vector3 worldPosStart, Vector3 worldPosEnd)
    {
        Vector3 center = new Vector3((worldPosStart.x + worldPosEnd.x) / 2, 1, (worldPosStart.z + worldPosEnd.z )/ 2);
        Vector3 halfExtents = new Vector3(Mathf.Abs(worldPosStart.x - worldPosEnd.x)/2, 1, Mathf.Abs(worldPosStart.z - worldPosEnd.z)/2);
        Collider[] objects = Physics.OverlapBox(center, halfExtents, Quaternion.identity, 1 << LayerMask.NameToLayer("SelectionBox"), QueryTriggerInteraction.Ignore);
        
        if (objects.Length > 0)
        {
            if (objects.Length == 1)
            {
                SelectObject(0, objects[0].transform);
                if (SelectedObj[0] != null)
                    EventListener.ObjectSelected(SelectedObj[0]);
            }
            else
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    SelectObject(i, objects[i].transform);
                }
            }
        }
    }

    private void DrawSelectionIndicator()
    {
        if (numObjectsSelected == 0)
        {
            ClearSelections();
        }
        else
        {
            for (int i = 0; i < numObjectsSelected; i++)
            {
                indicators[i].position = new Vector3(SelectedObj[i].transform.position.x, 3f, SelectedObj[i].transform.position.z);
                indicators[i].Rotate(Vector3.forward * Time.deltaTime * 30f);
                indicators[i].GetComponent<Projector>().fieldOfView = SelectedObj[i].ObjRadius;
            }
        }
    }

    private void SelectObject(int index, Transform obj)
    {
        Selectable objScript = null;
        while (objScript == null)
        {
            objScript = obj.GetComponent<Selectable>();

            if (objScript != null)
            {
                numObjectsSelected++;
                SelectedObj[index] = objScript;
                return;
            }

            if (obj == obj.root) break;
            obj = obj.transform.parent;
        }
    }

    private void SetupIndicators()
    {
        GameObject unitIndicatorPrefab = Resources.Load<GameObject>("Selection/UnitIndicator");
        indicators = new Transform[maxSelectedObjects];
        for (int i = 0; i < maxSelectedObjects; i++)
        {
            indicators[i] = Instantiate(unitIndicatorPrefab, transform).transform;
            indicators[i].position = Vector2.down;
        }
    }

    private void ClearSelections()
    {
        numObjectsSelected = 0;
        for (int i = 0; i < maxSelectedObjects; i++)
        {
            SelectedObj[i] = null;
            indicators[i].position = Vector2.down;
        }
    }
}
