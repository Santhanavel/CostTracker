using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using FoodTracker.Managers;
using FoodTracker.Persistence;
using FoodTracker.Data;

namespace FoodTracker.UI
{
    public class StatisticsController : MonoBehaviour
    {
        [Header("Month Comparison")]
        [SerializeField] private TMP_Text currentMonthCostText;
        [SerializeField] private TMP_Text previousMonthCostText;
        [SerializeField] private TMP_Text comparisonDifferenceText;

        [Header("Completion Rate")]
        [SerializeField] private TMP_Text completionRateText;
        [SerializeField] private Image completionRateBarFill;

        [Header("Weekday Costs (Mon-Sun Fills)")]
        [SerializeField] private Image[] weekdayBarFills; // Indexes 0=Mon, 1=Tue, ..., 6=Sun
        [SerializeField] private TMP_Text[] weekdayCostTexts;

        [Header("Bottom Nav Buttons")]
        [SerializeField] private Button navHomeButton;
        [SerializeField] private Button navCalendarButton;
        [SerializeField] private Button navStatsButton;
        [SerializeField] private Button navMoreButton;

        private void OnEnable()
        {
            CalculateAndRefreshStats();
        }

        private void Start()
        {
            if (navHomeButton != null) navHomeButton.onClick.AddListener(() => NavigateTo("Meal update page"));
            if (navCalendarButton != null) navCalendarButton.onClick.AddListener(() => NavigateTo("Calender Page"));
            if (navStatsButton != null) navStatsButton.onClick.AddListener(() => NavigateTo("Statistics Page"));
            if (navMoreButton != null) navMoreButton.onClick.AddListener(() => NavigateTo("Settings Page"));

            CalculateAndRefreshStats();
        }

        public void CalculateAndRefreshStats()
        {
            if (SaveManager.Instance == null || SaveManager.Instance.AppData == null) return;

            AppData appData = SaveManager.Instance.AppData;
            float mealCost = appData.settings.mealCost;

            DateTime today = DateTime.Today;
            int currentYear = today.Year;
            int currentMonth = today.Month;

            DateTime prevMonthDate = today.AddMonths(-1);
            int prevYear = prevMonthDate.Year;
            int prevMonth = prevMonthDate.Month;

            // 1. Costs calculation
            int curMonthMeals = 0;
            int prevMonthMeals = 0;

            float curMonthCost = 0f;
            float prevMonthCost = 0f;
            float[] weekdayMeals = new float[7]; // Group meals by day-of-week (0=Mon, ..., 6=Sun)
            float bCost = appData.settings.breakfastCost;
            float lCost = appData.settings.lunchCost;
            float dCost = appData.settings.dinnerCost;

            foreach (var record in appData.mealRecords)
            {
                if (DateTime.TryParse(record.dateString, out DateTime date))
                {
                    float cost = 0f;
                    int completed = 0;
                    if (record.breakfastCompleted) { completed++; cost += bCost; }
                    if (record.lunchCompleted) { completed++; cost += lCost; }
                    if (record.dinnerCompleted) { completed++; cost += dCost; }

                    if (date.Year == currentYear && date.Month == currentMonth)
                    {
                        curMonthMeals += completed;
                        curMonthCost += cost;

                        // Increment weekday stats
                        int dayIdx = ((int)date.DayOfWeek + 6) % 7; // Mon = 0 ... Sun = 6
                        weekdayMeals[dayIdx] += cost; // use cost instead of simple completed count for graph fill size
                    }
                    else if (date.Year == prevYear && date.Month == prevMonth)
                    {
                        prevMonthMeals += completed;
                        prevMonthCost += cost;
                    }
                }
            }

            if (currentMonthCostText != null) currentMonthCostText.text = $"Rs {curMonthCost:F0}";
            if (previousMonthCostText != null) previousMonthCostText.text = $"Rs {prevMonthCost:F0}";

            if (comparisonDifferenceText != null)
            {
                float diff = curMonthCost - prevMonthCost;
                if (diff >= 0)
                {
                    comparisonDifferenceText.text = $"+Rs {diff:F0} more than last month";
                    comparisonDifferenceText.color = new Color(0.9f, 0.4f, 0.4f); // Red alert color
                }
                else
                {
                    comparisonDifferenceText.text = $"-Rs {Mathf.Abs(diff):F0} saved this month";
                    comparisonDifferenceText.color = new Color(0.18f, 0.70f, 0.40f); // Green save color
                }
            }

            // 2. Completion rate calculation
            int daysInCurMonth = DateTime.DaysInMonth(currentYear, currentMonth);
            int totalPossibleMeals = daysInCurMonth * 3;
            float ratePercent = totalPossibleMeals > 0 ? ((float)curMonthMeals / totalPossibleMeals) * 100f : 0f;

            if (completionRateText != null)
            {
                completionRateText.text = $"{ratePercent:F1}% completion";
            }
            if (completionRateBarFill != null)
            {
                completionRateBarFill.fillAmount = ratePercent / 100f;
            }

            // 3. Weekday Cost split
            float maxDayCost = 1f;
            for (int i = 0; i < 7; i++)
            {
                if (weekdayMeals[i] > maxDayCost) maxDayCost = weekdayMeals[i];
            }

            for (int i = 0; i < 7; i++)
            {
                float dayCost = weekdayMeals[i];

                if (i < weekdayCostTexts.Length && weekdayCostTexts[i] != null)
                {
                    weekdayCostTexts[i].text = $"Rs {dayCost:F0}";
                }

                if (i < weekdayBarFills.Length && weekdayBarFills[i] != null)
                {
                    weekdayBarFills[i].fillAmount = weekdayMeals[i] / maxDayCost; // Normalized ratio
                }
            }
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
