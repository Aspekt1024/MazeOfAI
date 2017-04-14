using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitStats : MonoBehaviour {

    public Text ObjectName;

    private Selectable SelectedObject = null;

    List<string> statList = new List<string>();
    private Text[] textObjects = new Text[0];
    
    public void UnitSelected(Selectable newObj)
    {
        // Text objects only get recreated when a new object is selected
        // Otherwise, Update() will update the existing Text objects;
        if (SelectedObject == newObj) return;

        DeleteExistingText();
        SelectedObject = newObj;

        ObjectName.text = SelectedObject.Name;
        statList = SelectedObject.GetStatsList();
        CreateTextObjects();
        UpdateObjectStats();
    }

    public void Deselected()
    {
        SelectedObject = null;
        ObjectName.text = "";
        DeleteExistingText();
    }

    private void Start()
    {
        ObjectName.text = "";
    }

    private void Update()
    {
        if (SelectedObject == null) return;

        UpdateObjectStats();
    }

    private void UpdateObjectStats()
    {
        statList = SelectedObject.GetStatsList();
        for (int i = 0; i < statList.Count; i++)
        {
            textObjects[i].text = statList[i];
        }
    }

    private void CreateTextObjects()
    {
        textObjects = new Text[statList.Count];
        RectTransform rt = GetComponent<RectTransform>();

        const float padding = 3f;
        float textWidth = rt.rect.width;
        float textHeight = 20f;

        for (int i = 0; i < statList.Count; i++)
        {
            Text newText = Instantiate(Resources.Load<Text>("UI/StatTextPrefab"), transform);
            float xPos = transform.position.x - rt.rect.width / 2 + textWidth / 2 + padding;
            float yPos = transform.position.y + rt.rect.height / 2 - textHeight / 2 - textHeight * (i + 1);
            newText.rectTransform.sizeDelta = new Vector2(textWidth - 2 * padding, textHeight - 2 * padding);
            newText.rectTransform.position = new Vector3(xPos, yPos, 0f);
            newText.text = statList[i];

            textObjects[i] = newText;
        }
    }

    private void DeleteExistingText()
    {
        foreach(var obj in textObjects)
        {
            Destroy(obj.gameObject);
        }
        textObjects = new Text[0];
    }
}
