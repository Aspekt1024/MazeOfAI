using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitButtons : MonoBehaviour {

    private CanvasGroup canvas;
    private Selectable selectedObj;

    List<Button> buttons = new List<Button>();
    
    private void Awake()
    {
        canvas = GetComponent<CanvasGroup>();
        canvas.alpha = 0f;
    }
    
    public void UnitSelected(Selectable newObj)
    {
        canvas.alpha = 1f;
        canvas.blocksRaycasts = true;

        selectedObj = newObj;

        CreateButtons();
    }

    private void CreateButtons()
    {
        DestroyExistingButtons();

        if (selectedObj != null)
        {
            Rect rect = GetComponent<RectTransform>().rect;
            List<Selectable.Task> tasks = selectedObj.GetTaskList();

            const float btnPadding = 3f;
            const float btnHeight = 30f - btnPadding * 2;
            float btnWidth = rect.width - btnPadding * 2;

            for (int i = 0; i < tasks.Count; i++)
            {
                Button btn = Instantiate(Resources.Load<Button>("UI/ButtonPrefab"), transform);
                RectTransform rt = btn.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(btnWidth, btnHeight);

                float xPos = transform.position.x - rect.width / 2 + rt.rect.width / 2 + btnPadding;
                float yPos = transform.position.y + rect.height / 2 - rt.rect.height / 2 - btnPadding - i * btnHeight;
                Vector3 pos = new Vector3(xPos, yPos, 0);

                btn.GetComponent<RectTransform>().position = pos;
                btn.GetComponentInChildren<Text>().text = tasks[i].taskName;

                int taskId = tasks[i].taskId;
                btn.onClick.AddListener(() => selectedObj.SetTask(taskId));
                
                btn.navigation = new Navigation() {
                    mode = Navigation.Mode.None
                };

                buttons.Add(btn);
            }
        }
    }

    private void DestroyExistingButtons()
    {
        foreach (Button btn in buttons)
        {
            Destroy(btn.gameObject);
        }
        buttons = new List<Button>();
    }

    public void Deselected()
    {
        canvas.alpha = 0f;
        canvas.blocksRaycasts = false;
        selectedObj = null;
    }
}
