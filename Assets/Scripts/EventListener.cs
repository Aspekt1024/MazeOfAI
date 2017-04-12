using UnityEngine;

public static class EventListener {

    public delegate void SelectListener(GameObject unit);
    public static SelectListener OnUnitSelected;

    public delegate void DeselectListener();
    public static DeselectListener OnDeselected;

    public static void UnitSelected(GameObject unit)
    {
        if (OnUnitSelected != null)
            OnUnitSelected(unit);
    }

    public static void Deselected()
    {
        if (OnDeselected != null)
            OnDeselected();
    }
}
