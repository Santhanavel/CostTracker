using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using FoodTracker.Managers;
using FoodTracker.Persistence;
using FoodTracker.Data;

namespace FoodTracker.UI
{
    public class DayDetailsController : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] private TMP_Text dateTitleText;
        [SerializeField] private Button backButton;

        [Header("Meal Cards")]
        [SerializeField] private MealItemUI breakfastCard;
        [SerializeField] private MealItemUI lunchCard;
        [SerializeField] private MealItemUI dinnerCard;

        [Header("Cost Summary")]
        [SerializeField] private TMP_Text dayCostText;

        private DateTime selectedDate;
        private string dateKey;

        private void OnEnable()
        {
            LoadDayDetails();
        }

        private void Start()
        {
            if (backButton != null)
            {
                backButton.onClick.AddListener(OnBackClicked);
            }
            LoadDayDetails();
        }

        private void LoadDayDetails()
        {
            if (SaveManager.Instance == null || SaveManager.Instance.AppData == null) return;

            // 1. Resolve date to inspect from PlayerPrefs
            dateKey = PlayerPrefs.GetString("SelectedCalendarDate", DateTime.Today.ToString("yyyy-MM-dd"));
            if (!DateTime.TryParse(dateKey, out selectedDate))
            {
                selectedDate = DateTime.Today;
                dateKey = selectedDate.ToString("yyyy-MM-dd");
            }

            // 2. Set Header Text
            if (dateTitleText != null)
            {
                dateTitleText.text = selectedDate.ToString("MMMM d, yyyy");
            }

            // 3. Check if future date (only allow editing past & today)
            bool isFutureDate = selectedDate.Date > DateTime.Today;

            // 4. Fetch or Create Record
            AppData appData = SaveManager.Instance.AppData;
            MealRecord record = appData.mealRecords.Find(r => r.dateString == dateKey);
            if (record == null)
            {
                record = new MealRecord { dateString = dateKey };
                appData.mealRecords.Add(record);
            }

            // 5. Setup Meal Toggles
            if (breakfastCard != null)
            {
                breakfastCard.Setup("Breakfast", record.breakfastCompleted, record.breakfastTime, !isFutureDate, (val) => {
                    record.breakfastCompleted = val;
                    record.breakfastTime = val ? DateTime.Now.ToString("h:mm tt") : "";
                    SaveManager.Instance.Save();
                    UpdateCostSummary(record);
                });
            }

            if (lunchCard != null)
            {
                lunchCard.Setup("Lunch", record.lunchCompleted, record.lunchTime, !isFutureDate, (val) => {
                    record.lunchCompleted = val;
                    record.lunchTime = val ? DateTime.Now.ToString("h:mm tt") : "";
                    SaveManager.Instance.Save();
                    UpdateCostSummary(record);
                });
            }

            if (dinnerCard != null)
            {
                dinnerCard.Setup("Dinner", record.dinnerCompleted, record.dinnerTime, !isFutureDate, (val) => {
                    record.dinnerCompleted = val;
                    record.dinnerTime = val ? DateTime.Now.ToString("h:mm tt") : "";
                    SaveManager.Instance.Save();
                    UpdateCostSummary(record);
                });
            }

            UpdateCostSummary(record);
        }

        private void UpdateCostSummary(MealRecord record)
        {
            if (SaveManager.Instance == null || SaveManager.Instance.AppData == null) return;

            float costPerMeal = SaveManager.Instance.AppData.settings.mealCost;
            float dayCost = 0f;

            if (record.breakfastCompleted) dayCost += costPerMeal;
            if (record.lunchCompleted) dayCost += costPerMeal;
            if (record.dinnerCompleted) dayCost += costPerMeal;

            if (dayCostText != null)
            {
                dayCostText.text = $"{dayCost:F0}";
            }
        }

        private void OnBackClicked()
        {
            // Go back to calendar
            if (NavigationManager.Instance != null)
            {
                NavigationManager.Instance.NavigateTo("Calender Page");
            }
        }
    }
}
