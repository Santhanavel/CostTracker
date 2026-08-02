using UnityEngine;
using UnityEngine.UI;

public class OnboardingManager : MonoBehaviour
{
    [Header("UI Toggle References")]
    [SerializeField] private Toggle breakfastToggle;
    [SerializeField] private Toggle lunchToggle;
    [SerializeField] private Toggle dinnerToggle;

    [Header("Navigation References")]
    [SerializeField] private Button getStartedButton;
    [SerializeField] private GameObject onboardingPanel;
    [SerializeField] private GameObject mainDashboardPanel;

    private void Start()
    {
        // Bind button action
        if (getStartedButton != null)
        {
            getStartedButton.onClick.AddListener(OnGetStartedClicked);
        }

        // Initialize Toggle states based on current settings
        if (MealDataManager.Instance != null)
        {
            if (breakfastToggle != null) breakfastToggle.isOn = MealDataManager.Instance.TrackBreakfast;
            if (lunchToggle != null) lunchToggle.isOn = MealDataManager.Instance.TrackLunch;
            if (dinnerToggle != null) dinnerToggle.isOn = MealDataManager.Instance.TrackDinner;

            // If onboarding is already completed, automatically skip to the dashboard
            if (MealDataManager.Instance.IsOnboardingCompleted)
            {
                ShowDashboard();
            }
            else
            {
                ShowOnboarding();
            }
        }
    }

    private void OnGetStartedClicked()
    {
        if (MealDataManager.Instance == null) return;

        // Save preferences
        MealDataManager.Instance.TrackBreakfast = breakfastToggle != null ? breakfastToggle.isOn : true;
        MealDataManager.Instance.TrackLunch = lunchToggle != null ? lunchToggle.isOn : true;
        MealDataManager.Instance.TrackDinner = dinnerToggle != null ? dinnerToggle.isOn : true;
        MealDataManager.Instance.IsOnboardingCompleted = true;

        // Transition panels
        ShowDashboard();
    }

    private void ShowOnboarding()
    {
        if (onboardingPanel != null) onboardingPanel.SetActive(true);
        if (mainDashboardPanel != null) mainDashboardPanel.SetActive(false);
    }

    private void ShowDashboard()
    {
        if (onboardingPanel != null) onboardingPanel.SetActive(false);
        if (mainDashboardPanel != null) mainDashboardPanel.SetActive(true);

        // Refresh dynamic UI items if active
        var mealPage = mainDashboardPanel.GetComponentInChildren<MealUpdatePage>(true);
        if (mealPage != null)
        {
            mealPage.InitializeForDate(System.DateTime.Today);
        }
    }
}
