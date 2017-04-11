﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour {

    private GameObject SelectedUnit;
    private Player player;
    private Transform selectionBox;

    private float indicatorFoV;

    private void Awake()
    {
        player = GetComponent<Player>();
        selectionBox = GameObject.Find("UnitIndicator").transform;
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
            selectionBox.position = new Vector3(SelectedUnit.transform.position.x, 3f, SelectedUnit.transform.position.z);
            selectionBox.Rotate(Vector3.forward * Time.deltaTime * 20f);
        }
        else
        {
            selectionBox.position = new Vector3(0, -1, 0);
        }
    }

}