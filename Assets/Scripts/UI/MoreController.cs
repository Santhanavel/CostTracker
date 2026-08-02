using UnityEngine;
using UnityEngine.UI;
using FoodTracker.Managers;
using FoodTracker.Persistence;

namespace FoodTracker.UI
{
    public class MoreController : MonoBehaviour
    {
        [Header("Menu Buttons")]
        [SerializeField] private Button profileButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button remindersButton;
        [SerializeField] private Button replayOnboardingButton;

        [Header("Bottom Nav Buttons")]
        [SerializeField] private Button navHomeButton;
        [SerializeField] private Button navCalendarButton;
        [SerializeField] private Button navStatsButton;
        [SerializeField] private Button navWeightButton;
        [SerializeField] private Button navMoreButton;

        private void Start()
        {
            if (profileButton != null) profileButton.onClick.AddListener(() => NavigateTo("Profile Page"));
            if (settingsButton != null) settingsButton.onClick.AddListener(() => NavigateTo("Settings Page"));
            if (remindersButton != null) remindersButton.onClick.AddListener(() => NavigateTo("Reminders Page"));
            if (replayOnboardingButton != null) replayOnboardingButton.onClick.AddListener(OnReplayOnboardingClicked);

            if (navHomeButton != null) navHomeButton.onClick.AddListener(() => NavigateTo("Meal update page"));
            if (navCalendarButton != null) navCalendarButton.onClick.AddListener(() => NavigateTo("Calender Page"));
            if (navStatsButton != null) navStatsButton.onClick.AddListener(() => NavigateTo("Statistics Page"));
            if (navWeightButton != null) navWeightButton.onClick.AddListener(() => NavigateTo("Meal update page"));
            if (navMoreButton != null) navMoreButton.onClick.AddListener(() => NavigateTo("More Page"));
        }

        private void OnReplayOnboardingClicked()
        {
            if (SaveManager.Instance != null && SaveManager.Instance.AppData != null)
            {
                SaveManager.Instance.AppData.firstLaunch = true;
                SaveManager.Instance.Save();
            }
            NavigateTo("Onboarding Page");
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
