using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPanels : MonoBehaviour {

    public UnitButtons buttons;
    public UnitStats stats;

    private Selectable obj;

    private void OnEnable()
    {
        EventListener.OnUnitSelected += ObjectSelected;
        EventListener.OnDeselected += ObjectDeselected;
    }
    private void OnDisable()
    {
        EventListener.OnUnitSelected -= ObjectSelected;
        EventListener.OnDeselected -= ObjectDeselected;
    }

    private void ObjectSelected(Selectable newObj)
    {
        if (obj == newObj) return;

        obj = newObj;
        stats.UnitSelected(obj);
        buttons.UnitSelected(obj);
    }

    private void ObjectDeselected()
    {
        if (obj == null) return;
        obj = null;
        stats.Deselected();
        buttons.Deselected();
    }
}
