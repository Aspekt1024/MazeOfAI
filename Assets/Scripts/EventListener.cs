using UnityEngine;

public static class EventListener {

    public delegate void SelectListener(Selectable unit);
    public static SelectListener OnUnitSelected;

    public delegate void DeselectListener();
    public static DeselectListener OnDeselected;

    public delegate void ResourceListener();
    public static ResourceListener OnCurrencyChange;

    public static void ObjectSelected(Selectable unit)
    {
        if (OnUnitSelected != null)
            OnUnitSelected(unit);
    }

    public static void Deselected()
    {
        if (OnDeselected != null)
            OnDeselected();
    }

    public static void CurrencyChange()
    {
        if (OnCurrencyChange != null)
            OnCurrencyChange();
    }
}
