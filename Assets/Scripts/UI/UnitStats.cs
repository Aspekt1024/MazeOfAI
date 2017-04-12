using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitStats : MonoBehaviour {

    public Text UnitName;
    public Text UnitTask;

    private Unit unit;
    private Building building;
    
    public void UnitSelected(GameObject newObj)
    {
        unit = newObj.GetComponent<Unit>();
        building = newObj.GetComponent<Building>();
    }

    public void Deselected()
    {
        unit = null;
        building = null;
        UnitName.text = "";
        UnitTask.text = "";
    }

    private void Update()
    {
        if (unit != null)
            UpdateUnitStats();

        if (building != null)
            UpdateBuildingStats();
    }

    private void UpdateUnitStats()
    {
        UnitName.text = unit.Name;
        UnitTask.text = unit.Task.ToString();
    }

    private void UpdateBuildingStats()
    {
        UnitName.text = building.Name;
        UnitTask.text = building.GetProgressString();
    }
}
