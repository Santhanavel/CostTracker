using System;
using TMPro;
using UnityEngine;

public class MealUpdatePage : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text dateTitleText; // e.g. "Today - Aug 1, 2026"
    [SerializeField] private MealItemUI breakfastItem;
    [SerializeField] private MealItemUI lunchItem;
    [SerializeField] private MealItemUI dinnerItem;

    private int currentYear;
    private int currentMonth;
    private int currentDay;
    private DateTime selectedDate;

    // Call this method from the calendar when a date is selected, or default to today on Start
    private void Start()
    {
        // By default, initialize with today's date
        InitializeForDate(DateTime.Today);
    }

    public void InitializeForDate(DateTime date)
    {
        selectedDate = date.Date;
        currentYear = date.Year;
        currentMonth = date.Month;
        currentDay = date.Day;

        UpdateDateTitle();
        LoadAndDisplayMeals();
    }

    private void UpdateDateTitle()
    {
        if (dateTitleText == null) return;

        string prefix = (selectedDate == DateTime.Today) ? "Today" : selectedDate.ToString("dddd");
        dateTitleText.text = $"{prefix} • {selectedDate:MMM d, yyyy}";
    }

    private void LoadAndDisplayMeals()
    {
        DayData dayData = MealDataManager.Instance.GetDay(currentYear, currentMonth, currentDay);
        bool isEditable = selectedDate <= DateTime.Today;

        bool trackBreakfast = MealDataManager.Instance.TrackBreakfast;
        bool trackLunch = MealDataManager.Instance.TrackLunch;
        bool trackDinner = MealDataManager.Instance.TrackDinner;

        if (breakfastItem != null)
        {
            breakfastItem.gameObject.SetActive(trackBreakfast);
            if (trackBreakfast)
            {
                breakfastItem.Setup(
                    "Breakfast",
                    dayData.breakfast,
                    dayData.breakfastTime,
                    isEditable,
                    (val) => OnMealToggled("Breakfast", val)
                );
            }
        }

        if (lunchItem != null)
        {
            lunchItem.gameObject.SetActive(trackLunch);
            if (trackLunch)
            {
                lunchItem.Setup(
                    "Lunch",
                    dayData.lunch,
                    dayData.lunchTime,
                    isEditable,
                    (val) => OnMealToggled("Lunch", val)
                );
            }
        }

        if (dinnerItem != null)
        {
            dinnerItem.gameObject.SetActive(trackDinner);
            if (trackDinner)
            {
                dinnerItem.Setup(
                    "Dinner",
                    dayData.dinner,
                    dayData.dinnerTime,
                    isEditable,
                    (val) => OnMealToggled("Dinner", val)
                );
            }
        }
    }

    private void OnMealToggled(string mealName, bool completed)
    {
        // Safety check to prevent editing future dates
        if (selectedDate > DateTime.Today)
        {
            Debug.LogWarning("Cannot edit meals for future dates.");
            return;
        }

        switch (mealName)
        {
            case "Breakfast":
                MealDataManager.Instance.SetBreakfast(currentYear, currentMonth, currentDay, completed);
                break;
            case "Lunch":
                MealDataManager.Instance.SetLunch(currentYear, currentMonth, currentDay, completed);
                break;
            case "Dinner":
                MealDataManager.Instance.SetDinner(currentYear, currentMonth, currentDay, completed);
                break;
        }

        // Refresh the calendar and summary UI
        if (CalendarManager.Instance != null)
        {
            CalendarManager.Instance.Refresh();
        }

        // Refresh local UI display
        LoadAndDisplayMeals();
    }
}
