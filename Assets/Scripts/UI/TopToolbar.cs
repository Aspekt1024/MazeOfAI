using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopToolbar : MonoBehaviour {

    public Text CapsuleText;

    private void OnEnable()
    {
        EventListener.OnCurrencyChange += UpdateCurrency;
    }
    private void OnDisable()
    {
        EventListener.OnCurrencyChange -= UpdateCurrency;
    }

    private void Start()
    {
        // This must be called after initial values have been set up (done in GameData's Awake())
        UpdateCurrency();
    }

    private void UpdateCurrency()
    {
        CapsuleText.text = GameData.Instance.Capsules.ToString();
    }
}
