using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using FoodTracker.Managers;
using FoodTracker.Persistence;
using FoodTracker.Data;

namespace FoodTracker.UI
{
    public class SettingsController : MonoBehaviour
    {
        [Header("Profile Section")]
        [SerializeField] private TMP_InputField nameInputField;

        [Header("Meal Costs Section")]
        [SerializeField] private TMP_InputField breakfastCostInput;
        [SerializeField] private TMP_InputField lunchCostInput;
        [SerializeField] private TMP_InputField dinnerCostInput;

        [Header("Reminders Section")]
        [SerializeField] private Transform remindersContainer;
        [SerializeField] private GameObject reminderRowPrefab;
        [SerializeField] private Button addReminderButton;

        [Header("Bottom Sheet Popup")]
        [SerializeField] private GameObject modalPopup; // Bottom Sheet GameObject
        [SerializeField] private TMP_Dropdown popupMealDropdown;
        [SerializeField] private TMP_InputField popupTimeInput;
        [SerializeField] private Button popupSaveButton;
        [SerializeField] private Button popupCancelButton;

        [Header("Bottom Navigation")]
        [SerializeField] private Button navHomeButton;
        [SerializeField] private Button navCalendarButton;
        [SerializeField] private Button navStatsButton;
        [SerializeField] private Button navMoreButton;

        [Header("Main Save/Back Buttons")]
        [SerializeField] private Button saveButton;
        [SerializeField] private Button backButton;

        private string editingReminderId = null; // null if adding, set if editing

        private void OnEnable()
        {
            LoadAllData();
            if (modalPopup != null) modalPopup.SetActive(false);
        }

        private void Start()
        {
            if (saveButton != null) saveButton.onClick.AddListener(OnMainSaveClicked);
            if (backButton != null) backButton.onClick.AddListener(OnBackClicked);

            if (addReminderButton != null) addReminderButton.onClick.AddListener(OnAddReminderClicked);
            if (popupSaveButton != null) popupSaveButton.onClick.AddListener(OnPopupSaveClicked);
            if (popupCancelButton != null) popupCancelButton.onClick.AddListener(() => { if (modalPopup != null) modalPopup.SetActive(false); });

            if (navHomeButton != null) navHomeButton.onClick.AddListener(() => NavigateTo("Meal update page"));
            if (navCalendarButton != null) navCalendarButton.onClick.AddListener(() => NavigateTo("Calender Page"));
            if (navStatsButton != null) navStatsButton.onClick.AddListener(() => NavigateTo("Statistics Page"));
            if (navMoreButton != null) navMoreButton.onClick.AddListener(() => NavigateTo("More Page"));

            LoadAllData();
            if (modalPopup != null) modalPopup.SetActive(false);
        }

        private void LoadAllData()
        {
            if (SaveManager.Instance == null || SaveManager.Instance.AppData == null) return;

            AppData appData = SaveManager.Instance.AppData;

            // 1. Load Profile Name
            if (nameInputField != null)
            {
                nameInputField.text = appData.profile.name;
            }

            // 2. Load Meal Costs
            if (breakfastCostInput != null) breakfastCostInput.text = appData.settings.breakfastCost.ToString("F0");
            if (lunchCostInput != null) lunchCostInput.text = appData.settings.lunchCost.ToString("F0");
            if (dinnerCostInput != null) dinnerCostInput.text = appData.settings.dinnerCost.ToString("F0");

            // 3. Load Reminders List
            RefreshRemindersList();
        }

        private void RefreshRemindersList()
        {
            if (remindersContainer == null || reminderRowPrefab == null || SaveManager.Instance == null || SaveManager.Instance.AppData == null) return;

            // Clear old children
            foreach (Transform child in remindersContainer)
            {
                Destroy(child.gameObject);
            }

            List<ReminderData> reminders = SaveManager.Instance.AppData.reminderRecords;

            // Default mock reminders if none exist
            if (reminders.Count == 0)
            {
                reminders.Add(new ReminderData { id = "1", mealType = "Breakfast", timeString = "08:00 AM", enabled = true });
                reminders.Add(new ReminderData { id = "2", mealType = "Lunch", timeString = "01:00 PM", enabled = true });
                reminders.Add(new ReminderData { id = "3", mealType = "Dinner", timeString = "08:00 PM", enabled = true });
                SaveManager.Instance.Save();
            }

            foreach (var reminder in reminders)
            {
                GameObject row = Instantiate(reminderRowPrefab, remindersContainer);
                row.name = "Reminder_" + reminder.id;

                TMP_Text nameTxt = row.transform.Find("Label")?.GetComponent<TMP_Text>();
                TMP_Text timeTxt = row.transform.Find("Time")?.GetComponent<TMP_Text>();
                Button editBtn = row.transform.Find("EditButton")?.GetComponent<Button>();
                Button deleteBtn = row.transform.Find("DeleteButton")?.GetComponent<Button>();

                if (nameTxt != null) nameTxt.text = reminder.mealType;
                if (timeTxt != null) timeTxt.text = reminder.timeString;

                if (editBtn != null)
                {
                    editBtn.onClick.RemoveAllListeners();
                    editBtn.onClick.AddListener(() => OpenEditPopup(reminder));
                }

                if (deleteBtn != null)
                {
                    deleteBtn.onClick.RemoveAllListeners();
                    deleteBtn.onClick.AddListener(() => DeleteReminder(reminder));
                }
            }
        }

        private void OnAddReminderClicked()
        {
            editingReminderId = null;
            if (popupTimeInput != null) popupTimeInput.text = "08:00 AM";
            if (popupMealDropdown != null) popupMealDropdown.value = 0;
            if (modalPopup != null) modalPopup.SetActive(true);
        }

        private void OpenEditPopup(ReminderData reminder)
        {
            editingReminderId = reminder.id;
            if (popupTimeInput != null) popupTimeInput.text = reminder.timeString;

            if (popupMealDropdown != null)
            {
                string meal = reminder.mealType.Replace(" Reminder", "").Trim();
                int idx = popupMealDropdown.options.FindIndex(opt => opt.text.Equals(meal, StringComparison.OrdinalIgnoreCase));
                if (idx >= 0) popupMealDropdown.value = idx;
            }

            if (modalPopup != null) modalPopup.SetActive(true);
        }

        private void DeleteReminder(ReminderData reminder)
        {
            if (SaveManager.Instance == null || SaveManager.Instance.AppData == null) return;

            SaveManager.Instance.AppData.reminderRecords.Remove(reminder);
            SaveManager.Instance.Save();
            RefreshRemindersList();
        }

        private void OnPopupSaveClicked()
        {
            if (SaveManager.Instance == null || SaveManager.Instance.AppData == null) return;

            string mealText = popupMealDropdown != null ? popupMealDropdown.options[popupMealDropdown.value].text : "Breakfast";
            string timeText = popupTimeInput != null ? popupTimeInput.text.Trim() : "08:00 AM";

            if (string.IsNullOrEmpty(editingReminderId))
            {
                // Create new
                ReminderData newReminder = new ReminderData
                {
                    id = Guid.NewGuid().ToString(),
                    mealType = mealText,
                    timeString = timeText,
                    enabled = true,
                    repeatDaily = true,
                    message = "Time for your " + mealText
                };
                SaveManager.Instance.AppData.reminderRecords.Add(newReminder);
            }
            else
            {
                // Edit existing
                ReminderData rem = SaveManager.Instance.AppData.reminderRecords.Find(r => r.id == editingReminderId);
                if (rem != null)
                {
                    rem.mealType = mealText;
                    rem.timeString = timeText;
                    rem.message = "Time for your " + mealText;
                }
            }

            SaveManager.Instance.Save();
            if (modalPopup != null) modalPopup.SetActive(false);
            RefreshRemindersList();
        }

        private void OnMainSaveClicked()
        {
            if (SaveManager.Instance == null || SaveManager.Instance.AppData == null) return;

            AppData appData = SaveManager.Instance.AppData;

            // 1. Save Profile Name
            if (nameInputField != null)
            {
                appData.profile.name = nameInputField.text.Trim();
            }

            // 2. Save Meal Costs
            if (breakfastCostInput != null && float.TryParse(breakfastCostInput.text, out float bCost))
            {
                appData.settings.breakfastCost = bCost;
            }
            if (lunchCostInput != null && float.TryParse(lunchCostInput.text, out float lCost))
            {
                appData.settings.lunchCost = lCost;
            }
            if (dinnerCostInput != null && float.TryParse(dinnerCostInput.text, out float dCost))
            {
                appData.settings.dinnerCost = dCost;
            }

            SaveManager.Instance.Save();
            NavigateTo("More Page");
        }

        private void OnBackClicked()
        {
            NavigateTo("More Page");
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
