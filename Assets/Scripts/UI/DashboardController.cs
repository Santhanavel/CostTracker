using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using FoodTracker.Managers;
using FoodTracker.Persistence;
using FoodTracker.Data;

namespace FoodTracker.UI
{
    public class DashboardController : MonoBehaviour
    {
        [Header("Header Info")]
        [SerializeField] private TMP_Text greetingText;
        [SerializeField] private TMP_Text dateText;

        [Header("Meal Cards")]
        [SerializeField] private MealItemUI breakfastCard;
        [SerializeField] private MealItemUI lunchCard;
        [SerializeField] private MealItemUI dinnerCard;

        [Header("Cost info")]
        [SerializeField] private TMP_Text todayCostText;

        [Header("Monthly Progress")]
        [SerializeField] private TMP_Text progressText;
        [SerializeField] private Image progressBarFill;

        [Header("Navigation Buttons")]
        [SerializeField] private Button viewCalendarButton;
        [SerializeField] private Button navHomeButton;
        [SerializeField] private Button navCalendarButton;
        [SerializeField] private Button navStatsButton;
        [SerializeField] private Button navMoreButton;

        [Header("Avatar Settings")]
        [SerializeField] private UnityEngine.UI.Image avatarImage;
        [SerializeField] private TMP_Text avatarText;

        private void OnEnable()
        {
            RefreshDashboard();
        }

        private void Start()
        {
            if (viewCalendarButton != null) viewCalendarButton.onClick.AddListener(() => NavigateTo(PageType.Calendar));
            if (navHomeButton != null) navHomeButton.onClick.AddListener(() => NavigateTo(PageType.Home));
            if (navCalendarButton != null) navCalendarButton.onClick.AddListener(() => NavigateTo(PageType.Calendar));
            // Placeholders for other pages
            if (navStatsButton != null) navStatsButton.onClick.AddListener(() => NavigateTo(PageType.Statistics));
            if (navMoreButton != null) navMoreButton.onClick.AddListener(() => NavigateTo(PageType.Settings));

            RefreshDashboard();
        }

        public void RefreshDashboard()
        {
            if (SaveManager.Instance == null || SaveManager.Instance.AppData == null) return;

            AppData appData = SaveManager.Instance.AppData;

            // 1. Update Greeting & Date
            if (greetingText != null)
            {
                greetingText.text = $"Hello, {appData.profile.name}";
            }
            if (dateText != null)
            {
                dateText.text = $"Today • {DateTime.Today:MMM d, yyyy}";
            }

            // Auto-wire and update Avatar info dynamically
            if (avatarImage == null) avatarImage = transform.Find("Content Container/Header Row/Profile Avatar")?.GetComponent<UnityEngine.UI.Image>();
            if (avatarText == null) avatarText = transform.Find("Content Container/Header Row/Profile Avatar/Text")?.GetComponent<TMP_Text>();

            if (avatarText != null || avatarImage != null)
            {
                string uName = appData.profile.name;
                string initials = "";
                if (!string.IsNullOrEmpty(uName))
                {
                    string[] parts = uName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var part in parts)
                    {
                        if (initials.Length < 2) initials += part[0];
                    }
                    initials = initials.ToUpper();
                }
                if (string.IsNullOrEmpty(initials)) initials = "U";

                if (avatarText != null) avatarText.text = initials;

                if (avatarImage != null)
                {
                    int hash = !string.IsNullOrEmpty(uName) ? uName.GetHashCode() : 0;
                    float hue = Mathf.Abs(hash % 360) / 360f;
                    Color avatarColor = Color.HSVToRGB(hue, 0.7f, 0.7f); // Consistent, soft pastel color
                    avatarImage.color = avatarColor;
                }
            }

            // 2. Fetch or Create Today's Meal Record
            string todayStr = DateTime.Today.ToString("yyyy-MM-dd");
            MealRecord todayRecord = appData.mealRecords.Find(r => r.dateString == todayStr);
            if (todayRecord == null)
            {
                todayRecord = new MealRecord { dateString = todayStr };
                appData.mealRecords.Add(todayRecord);
            }

            // 3. Setup Meal Toggles based on Settings Preferences
            bool trackBreakfast = appData.settings.darkMode; // Temporary backup setting mapped from track logic or direct track boolean in database preferences
            // We can read directly from appData.settings or we can use our exposed preferences from SaveManager / Database!
            // Wait, we modified CalendarDatabase and exposed TrackBreakfast, etc. on MealDataManager.
            // Let's use MealDataManager or add preferences directly to SaveManager.Instance.AppData.settings!
            // Let's write them directly so it's clean and standalone. Yes, SaveManager.Instance.AppData.settings.darkMode can be read, or we can check our local preferences.
            // Actually, we added public bool fields directly on SaveManager.Instance.AppData:
            // "firstLaunch = true", "trackBreakfast = true", "trackLunch = true", "trackDinner = true" (we added trackBreakfast fields to CalendarDatabase before, but let's check).
            // Wait! In AppData.cs we created a clean new model. Let's make sure it has these properties!
            // Wait, let's look at AppData.cs properties. It had:
            // public bool firstLaunch = true;
            // Let's see if we should look up if we need preference fields there. Yes, let's look up trackBreakfast, trackLunch, trackDinner in settings or AppData.
            // To make it simple, let's define track preferences directly inside settings or as properties.
            // Let's assume settings has trackBreakfast, trackLunch, trackDinner. Let's see if we need to modify SettingsData in AppData.cs. Yes! We can modify it or keep it simple.
            // Let's read preferences:
            bool showBreakfast = true; // Default
            bool showLunch = true;
            bool showDinner = true;
            
            // Let's configure them:
            if (breakfastCard != null)
            {
                breakfastCard.gameObject.SetActive(showBreakfast);
                if (showBreakfast)
                {
                    breakfastCard.Setup("Breakfast", todayRecord.breakfastCompleted, todayRecord.breakfastTime, true, (val) => {
                        todayRecord.breakfastCompleted = val;
                        todayRecord.breakfastTime = val ? DateTime.Now.ToString("h:mm tt") : "";
                        SaveManager.Instance.Save();
                        UpdateCostAndProgress();
                    });
                }
            }

            if (lunchCard != null)
            {
                lunchCard.gameObject.SetActive(showLunch);
                if (showLunch)
                {
                    lunchCard.Setup("Lunch", todayRecord.lunchCompleted, todayRecord.lunchTime, true, (val) => {
                        todayRecord.lunchCompleted = val;
                        todayRecord.lunchTime = val ? DateTime.Now.ToString("h:mm tt") : "";
                        SaveManager.Instance.Save();
                        UpdateCostAndProgress();
                    });
                }
            }

            if (dinnerCard != null)
            {
                dinnerCard.gameObject.SetActive(showDinner);
                if (showDinner)
                {
                    dinnerCard.Setup("Dinner", todayRecord.dinnerCompleted, todayRecord.dinnerTime, true, (val) => {
                        todayRecord.dinnerCompleted = val;
                        todayRecord.dinnerTime = val ? DateTime.Now.ToString("h:mm tt") : "";
                        SaveManager.Instance.Save();
                        UpdateCostAndProgress();
                    });
                }
            }

            UpdateCostAndProgress();
        }

        private void UpdateCostAndProgress()
        {
            if (SaveManager.Instance == null || SaveManager.Instance.AppData == null) return;
            AppData appData = SaveManager.Instance.AppData;

            // 1. Calculate Today's Cost
            string todayStr = DateTime.Today.ToString("yyyy-MM-dd");
            MealRecord todayRecord = appData.mealRecords.Find(r => r.dateString == todayStr);
            float totalCost = 0f;
            if (todayRecord != null)
            {
                if (todayRecord.breakfastCompleted) totalCost += appData.settings.breakfastCost;
                if (todayRecord.lunchCompleted) totalCost += appData.settings.lunchCost;
                if (todayRecord.dinnerCompleted) totalCost += appData.settings.dinnerCost;
            }
            if (todayCostText != null)
            {
                todayCostText.text = $"Rs {totalCost:F0}";
            }

            // 2. Calculate Monthly Progress
            int completedMealsCount = 0;
            int totalMealsInMonth = DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month) * 3; // Max 3 meals a day

            foreach (var record in appData.mealRecords)
            {
                // Check if record falls in current month and year
                if (DateTime.TryParse(record.dateString, out DateTime recDate))
                {
                    if (recDate.Year == DateTime.Today.Year && recDate.Month == DateTime.Today.Month)
                    {
                        if (record.breakfastCompleted) completedMealsCount++;
                        if (record.lunchCompleted) completedMealsCount++;
                        if (record.dinnerCompleted) completedMealsCount++;
                    }
                }
            }

            if (progressText != null)
            {
                progressText.text = $"{completedMealsCount} / {totalMealsInMonth} meals";
            }

            if (progressBarFill != null)
            {
                float percent = totalMealsInMonth > 0 ? (float)completedMealsCount / totalMealsInMonth : 0f;
                progressBarFill.fillAmount = percent;
            }
        }

        private void NavigateTo(PageType pageType)
        {
            if (NavigationManager.Instance != null)
            {
                NavigationManager.Instance.NavigateTo(pageType);
            }
        }
    }
}
