using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FoodTracker.Managers;
using FoodTracker.Persistence;
using FoodTracker.Data;

namespace FoodTracker.UI
{
    public class ProfileController : MonoBehaviour
    {
        [Header("Profile Inputs")]
        [SerializeField] private TMP_InputField nameInputField;
        [SerializeField] private TMP_InputField emailInputField;
        [SerializeField] private TMP_InputField heightInputField;
        [SerializeField] private TMP_Dropdown activityLevelDropdown;

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
            LoadProfileData();
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

            LoadProfileData();
        }

        private void LoadProfileData()
        {
            if (SaveManager.Instance == null || SaveManager.Instance.AppData == null) return;

            ProfileData profile = SaveManager.Instance.AppData.profile;

            if (nameInputField != null) nameInputField.text = profile.name;
            if (emailInputField != null) emailInputField.text = profile.email;
            if (heightInputField != null) heightInputField.text = profile.height.ToString("F1");
            
            if (activityLevelDropdown != null)
            {
                int index = activityLevelDropdown.options.FindIndex(opt => opt.text.Equals(profile.activityLevel, System.StringComparison.OrdinalIgnoreCase));
                if (index >= 0) activityLevelDropdown.value = index;
            }
        }

        private void OnSaveClicked()
        {
            if (SaveManager.Instance == null || SaveManager.Instance.AppData == null) return;

            ProfileData profile = SaveManager.Instance.AppData.profile;

            if (nameInputField != null) profile.name = nameInputField.text.Trim();
            if (emailInputField != null) profile.email = emailInputField.text.Trim();
            
            if (heightInputField != null && float.TryParse(heightInputField.text, out float h))
            {
                profile.height = h;
            }

            if (activityLevelDropdown != null)
            {
                profile.activityLevel = activityLevelDropdown.options[activityLevelDropdown.value].text;
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
