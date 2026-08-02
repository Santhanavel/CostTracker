using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MealItemUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text mealNameText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private Toggle mealToggle;
    [SerializeField] private Image statusIcon; // Dot img / checkmark

    [Header("Graphics Settings")]
    [SerializeField] private Sprite completedIconSprite;
    [SerializeField] private Sprite notYetIconSprite;
    [SerializeField] private Color completedColor = new Color(0.18f, 0.54f, 0.34f); // Green
    [SerializeField] private Color notYetColor = Color.gray;

    private Action<bool> onToggleCallback;
    private bool isUpdatingUI;

    public void Setup(string mealName, bool isCompleted, string timestamp, bool isEditable, Action<bool> onToggle)
    {
        isUpdatingUI = true;

        mealNameText.text = mealName;
        mealToggle.isOn = isCompleted;
        mealToggle.interactable = isEditable;
        onToggleCallback = onToggle;

        UpdateStatusTextAndIcon(mealName, isCompleted, timestamp);

        isUpdatingUI = false;
    }

    private void Start()
    {
        if (mealToggle != null)
        {
            mealToggle.onValueChanged.AddListener(OnToggleChanged);
        }
    }

    private void OnDestroy()
    {
        if (mealToggle != null)
        {
            mealToggle.onValueChanged.RemoveListener(OnToggleChanged);
        }
    }

    private void OnToggleChanged(bool value)
    {
        if (isUpdatingUI) return;

        onToggleCallback?.Invoke(value);
    }

    public void UpdateStatusTextAndIcon(string mealName, bool isCompleted, string timestamp)
    {
        if (isCompleted)
        {
            statusText.text = "Completed";
            timeText.text = string.IsNullOrEmpty(timestamp) ? System.DateTime.Now.ToString("h:mm tt") : timestamp;
            if (statusIcon != null)
            {
                statusIcon.color = completedColor;
                if (completedIconSprite != null)
                {
                    statusIcon.sprite = completedIconSprite;
                }
            }
        }
        else
        {
            statusText.text = "Not yet";
            timeText.text = $"Mark your {mealName.ToLower()}";
            if (statusIcon != null)
            {
                statusIcon.color = notYetColor;
                if (notYetIconSprite != null)
                {
                    statusIcon.sprite = notYetIconSprite;
                }
            }
        }
    }
}
