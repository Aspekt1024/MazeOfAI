using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPanels : MonoBehaviour {

    public UnitButtons buttons;
    public UnitStats stats;

    private GameObject unit;

    private void OnEnable()
    {
        EventListener.OnUnitSelected += UnitSelected;
        EventListener.OnDeselected += UnitDeselected;
    }
    private void OnDisable()
    {
        EventListener.OnUnitSelected -= UnitSelected;
        EventListener.OnDeselected -= UnitDeselected;
    }

    private void UnitSelected(GameObject newUnit)
    {
        if (unit == newUnit) return;

        unit = newUnit;
        stats.UnitSelected(unit);
        buttons.UnitSelected(unit);
    }

    private void UnitDeselected()
    {
        if (unit == null) return;
        unit = null;
        stats.Deselected();
        buttons.Deselected();
    }
}
