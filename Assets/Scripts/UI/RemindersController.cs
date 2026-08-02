using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using FoodTracker.Managers;
using FoodTracker.Persistence;
using FoodTracker.Data;

namespace FoodTracker.UI
{
    public class RemindersController : MonoBehaviour
    {
        [Header("UI Containers")]
        [SerializeField] private Transform remindersContainer;
        [SerializeField] private GameObject reminderRowPrefab;

        [Header("Buttons")]
        [SerializeField] private Button addReminderButton;

        [Header("Bottom Nav Buttons")]
        [SerializeField] private Button navHomeButton;
        [SerializeField] private Button navCalendarButton;
        [SerializeField] private Button navStatsButton;
        [SerializeField] private Button navWeightButton;
        [SerializeField] private Button navMoreButton;

        private void OnEnable()
        {
            RefreshRemindersList();
        }

        private void Start()
        {
            if (addReminderButton != null) addReminderButton.onClick.AddListener(OnAddReminderClicked);

            if (navHomeButton != null) navHomeButton.onClick.AddListener(() => NavigateTo("Meal update page"));
            if (navCalendarButton != null) navCalendarButton.onClick.AddListener(() => NavigateTo("Calender Page"));
            if (navStatsButton != null) navStatsButton.onClick.AddListener(() => NavigateTo("Statistics Page"));
            if (navWeightButton != null) navWeightButton.onClick.AddListener(() => NavigateTo("Meal update page"));
            if (navMoreButton != null) navMoreButton.onClick.AddListener(() => NavigateTo("More Page"));

            RefreshRemindersList();
        }

        public void RefreshRemindersList()
        {
            if (SaveManager.Instance == null || SaveManager.Instance.AppData == null) return;
            if (remindersContainer == null || reminderRowPrefab == null) return;

            // Clear old children
            foreach (Transform child in remindersContainer)
            {
                Destroy(child.gameObject);
            }

            List<ReminderData> reminders = SaveManager.Instance.AppData.reminderRecords;

            // Default mock reminders if none exist
            if (reminders.Count == 0)
            {
                reminders.Add(new ReminderData { id = "1", mealType = "Breakfast Reminder", timeString = "08:00 AM", enabled = true });
                reminders.Add(new ReminderData { id = "2", mealType = "Lunch Reminder", timeString = "01:00 PM", enabled = true });
                reminders.Add(new ReminderData { id = "3", mealType = "Dinner Reminder", timeString = "08:00 PM", enabled = true });
                SaveManager.Instance.Save();
            }

            foreach (var reminder in reminders)
            {
                GameObject row = Instantiate(reminderRowPrefab, remindersContainer);
                row.name = "Reminder_" + reminder.id;

                TMP_Text labelTxt = row.transform.Find("Label")?.GetComponent<TMP_Text>();
                TMP_Text timeTxt = row.transform.Find("Time")?.GetComponent<TMP_Text>();
                Toggle activeTgl = row.transform.Find("Toggle")?.GetComponent<Toggle>();

                if (labelTxt != null) labelTxt.text = reminder.mealType;
                if (timeTxt != null) timeTxt.text = reminder.timeString;
                if (activeTgl != null)
                {
                    activeTgl.isOn = reminder.enabled;
                    activeTgl.onValueChanged.RemoveAllListeners();
                    activeTgl.onValueChanged.AddListener((val) => {
                        reminder.enabled = val;
                        SaveManager.Instance.Save();
                    });
                }
            }
        }

        private void OnAddReminderClicked()
        {
            NavigateTo("Add Reminder Page");
        }

        private void NavigateTo(string pageName)
        {
            if (NavigationManager.Instance != null)
            {
                NavigationManager.Instance.NavigateTo(pageName);
            }
        }
    }
}
