using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FoodTracker.Managers;
using FoodTracker.Persistence;
using FoodTracker.Data;

namespace FoodTracker.UI
{
    public class SettingsController : MonoBehaviour
    {
        [Header("Settings Inputs")]
        [SerializeField] private TMP_InputField mealCostInputField;
        [SerializeField] private TMP_Dropdown currencyDropdown;
        [SerializeField] private TMP_Dropdown startWeekDropdown;
        [SerializeField] private Toggle darkModeToggle;

        [Header("Form Buttons")]
        [SerializeField] private Button saveButton;
        [SerializeField] private Button cancelButton;

        [Header("Bottom Nav Buttons")]
        [SerializeField] private Button navHomeButton;
        [SerializeField] private Button navCalendarButton;
        [SerializeField] private Button navStatsButton;
        [SerializeField] private Button navWeightButton;
        [SerializeField] private Button navMoreButton;

        private void OnEnable()
        {
            LoadSettingsData();
        }

        private void Start()
        {
            if (saveButton != null) saveButton.onClick.AddListener(OnSaveClicked);
            if (cancelButton != null) cancelButton.onClick.AddListener(OnCancelClicked);

            if (navHomeButton != null) navHomeButton.onClick.AddListener(() => NavigateTo("Meal update page"));
            if (navCalendarButton != null) navCalendarButton.onClick.AddListener(() => NavigateTo("Calender Page"));
            if (navStatsButton != null) navStatsButton.onClick.AddListener(() => NavigateTo("Statistics Page"));
            if (navWeightButton != null) navWeightButton.onClick.AddListener(() => NavigateTo("Meal update page"));
            if (navMoreButton != null) navMoreButton.onClick.AddListener(() => NavigateTo("More Page"));

            LoadSettingsData();
        }

        private void LoadSettingsData()
        {
            if (SaveManager.Instance == null || SaveManager.Instance.AppData == null) return;

            SettingsData settings = SaveManager.Instance.AppData.settings;

            if (mealCostInputField != null) mealCostInputField.text = settings.mealCost.ToString("F0");
            
            if (currencyDropdown != null)
            {
                int index = currencyDropdown.options.FindIndex(opt => opt.text.Equals(settings.currency, System.StringComparison.OrdinalIgnoreCase));
                if (index >= 0) currencyDropdown.value = index;
            }

            if (startWeekDropdown != null)
            {
                int index = startWeekDropdown.options.FindIndex(opt => opt.text.Equals(settings.startWeek, System.StringComparison.OrdinalIgnoreCase));
                if (index >= 0) startWeekDropdown.value = index;
            }

            if (darkModeToggle != null)
            {
                darkModeToggle.isOn = settings.darkMode;
            }
        }

        private void OnSaveClicked()
        {
            if (SaveManager.Instance == null || SaveManager.Instance.AppData == null) return;

            SettingsData settings = SaveManager.Instance.AppData.settings;

            if (mealCostInputField != null && float.TryParse(mealCostInputField.text, out float cost))
            {
                settings.mealCost = cost;
            }

            if (currencyDropdown != null)
            {
                settings.currency = currencyDropdown.options[currencyDropdown.value].text;
            }

            if (startWeekDropdown != null)
            {
                settings.startWeek = startWeekDropdown.options[startWeekDropdown.value].text;
            }

            if (darkModeToggle != null)
            {
                settings.darkMode = darkModeToggle.isOn;
            }

            SaveManager.Instance.Save();
            NavigateTo("More Page");
        }

        private void OnCancelClicked()
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
