using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertWindow : MonoBehaviour
{
    private Message[] messages;
    private const int maxItemsInQueue = 5;
    private const float messageDuration = 3f;

    private int currentMsgCount = 0;
    private int msgIndexOldest = 0;
    private int msgIndexNewest = 0;

    private Text[] alertText;

    private struct Message
    {
        public float messageStartTime;
        public string messageText;
        public Color colour;
    }

    private void Start()
    {
        messages = new Message[maxItemsInQueue];
        CreateTextBoxes();
    }

    private void Update()
    {
        if (currentMsgCount == 0) return;

        if (Time.time - messages[msgIndexOldest].messageStartTime > messageDuration)
        {
            msgIndexOldest++;
            if (msgIndexOldest == maxItemsInQueue)
                msgIndexOldest = 0;

            currentMsgCount--;
            UpdateMessages();
        }
    }

    private void UpdateMessages()
    {
        for (int i = 0; i < maxItemsInQueue; i++)
        {
            if (i < currentMsgCount)
            {
                int msgIndex = msgIndexOldest + i;
                if (msgIndex >= maxItemsInQueue)
                    msgIndex -= maxItemsInQueue;

                alertText[i].text = messages[msgIndex].messageText;
            }
            else
            {
                alertText[i].text = "";
            }
        }
    }

    private void OnEnable()
    {
        AlertEvents.OnInsufficientFunds += InsufficientFunds;
    }
    private void OnDisable()
    {
        AlertEvents.OnInsufficientFunds -= InsufficientFunds;
    }

    private void InsufficientFunds()
    {
        Message message = new Message()
        {
            messageStartTime = Time.time,
            messageText = "Insufficient Funds",
            colour = Color.yellow
        };

        if (currentMsgCount == 0)
        {
            messages[0] = message;
            msgIndexOldest = 0;
            msgIndexNewest = 0;
        }
        else
        {
            msgIndexNewest++;
            if (msgIndexNewest == maxItemsInQueue)
                msgIndexNewest = 0;

            if (currentMsgCount == maxItemsInQueue)
            {
                msgIndexOldest++;
                if (msgIndexOldest == maxItemsInQueue)
                    msgIndexOldest = 0;
            }
            messages[msgIndexNewest] = message;
        }
        currentMsgCount++;
        UpdateMessages();
    }

    private void CreateTextBoxes()
    {
        alertText = new Text[maxItemsInQueue];

        const float txtWidth = 200f;
        const float txtHeight = 25f;
        const float txtPadding = 3f;

        for (int i = 0; i < maxItemsInQueue; i++)
        {
            Text newText = Instantiate(Resources.Load<Text>("UI/TextPrefab"), transform);
            newText.rectTransform.sizeDelta = new Vector2(txtWidth - 2 * txtPadding, txtHeight - 2 * txtPadding);

            float xPos = transform.position.x + txtWidth / 2;
            float yPos = transform.position.y - txtHeight / 2 - i * txtHeight;
            newText.rectTransform.position = new Vector3(xPos, yPos, 0f);
            newText.text = "";

            newText.raycastTarget = false;
            alertText[i] = newText;
        }
    }
}

public static class AlertEvents
{
    public delegate void Alert();
    public static Alert OnInsufficientFunds;

    public static void InsufficientFunds()
    {
        if (OnInsufficientFunds != null)
            OnInsufficientFunds();
    }
}
