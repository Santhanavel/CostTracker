using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using FoodTracker.Managers;
using FoodTracker.Persistence;
using FoodTracker.Data;

namespace FoodTracker.UI
{
    public class AddReminderController : MonoBehaviour
    {
        [Header("Inputs")]
        [SerializeField] private TMP_InputField labelInputField;
        [SerializeField] private TMP_InputField timeInputField;

        [Header("Buttons")]
        [SerializeField] private Button saveButton;
        [SerializeField] private Button cancelButton;

        private void Start()
        {
            if (saveButton != null) saveButton.onClick.AddListener(OnSaveClicked);
            if (cancelButton != null) cancelButton.onClick.AddListener(OnCancelClicked);
        }

        private void OnEnable()
        {
            // Reset input values
            if (labelInputField != null) labelInputField.text = "";
            if (timeInputField != null) timeInputField.text = "08:00 AM";
        }

        private void OnSaveClicked()
        {
            if (SaveManager.Instance == null || SaveManager.Instance.AppData == null) return;

            string labelText = labelInputField != null ? labelInputField.text.Trim() : "";
            string timeText = timeInputField != null ? timeInputField.text.Trim() : "08:00 AM";

            if (string.IsNullOrEmpty(labelText))
            {
                labelText = "Meal Reminder";
            }

            ReminderData newReminder = new ReminderData
            {
                id = Guid.NewGuid().ToString(),
                mealType = labelText,
                timeString = timeText,
                enabled = true,
                repeatDaily = true,
                message = "Time for your " + labelText
            };

            SaveManager.Instance.AppData.reminderRecords.Add(newReminder);
            SaveManager.Instance.Save();

            NavigateBack();
        }

        private void OnCancelClicked()
        {
            NavigateBack();
        }

        private void NavigateBack()
        {
            if (NavigationManager.Instance != null)
            {
                NavigationManager.Instance.NavigateTo("Reminders Page");
            }
        }
    }
}
