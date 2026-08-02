using System;
using UnityEngine;

public class CalendarManager : MonoBehaviour
{
    public static CalendarManager Instance;

    [Header("References")]
    [SerializeField] private CalendarGenerator calendarGenerator;
    [SerializeField] private MonthlySummary monthlySummary;

    [Header("Settings")]
    [SerializeField] private bool startWithCurrentDate = true;

    private DateTime currentDate;

    public int CurrentYear => currentDate.Year;
    public int CurrentMonth => currentDate.Month;

    public MonthData CurrentMonthData =>
        MealDataManager.Instance.GetOrCreateMonth(CurrentYear, CurrentMonth);
   
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (startWithCurrentDate)
            currentDate = DateTime.Today;
        else
            currentDate = new DateTime(2026, 8, 1);

        Refresh();
    }

    #region Navigation

    public void NextMonth()
    {
        currentDate = currentDate.AddMonths(1);

        Refresh();
    }

    public void PreviousMonth()
    {
        currentDate = currentDate.AddMonths(-1);

        Refresh();
    }

    public void GoToToday()
    {
        currentDate = DateTime.Today;

        Refresh();
    }

    public void SetMonth(int year, int month)
    {
        currentDate = new DateTime(year, month, 1);

        Refresh();
    }

    #endregion

    #region Refresh

    public void Refresh()
    {
        MonthData monthData =
            MealDataManager.Instance.GetOrCreateMonth(CurrentYear, CurrentMonth);

        calendarGenerator.GenerateCalendar(monthData);

        UpdateSummary();
    }

    public void UpdateSummary()
    {
        int lunch =
            MealDataManager.Instance.GetLunchCount(CurrentYear, CurrentMonth);

        int dinner =
            MealDataManager.Instance.GetDinnerCount(CurrentYear, CurrentMonth);

        int meals =
            MealDataManager.Instance.GetMealCount(CurrentYear, CurrentMonth);

        int cost =
            MealDataManager.Instance.GetTotalCost(CurrentYear, CurrentMonth);

        monthlySummary.UpdateSummary(
            CurrentYear,
            CurrentMonth,
            lunch,
            dinner,
            meals,
            cost);
    }

    #endregion

    #region Cell Updates

    public void ToggleLunch(int day)
    {
        MealDataManager.Instance.ToggleLunch(
            CurrentYear,
            CurrentMonth,
            day);

        Refresh();
    }

    public void ToggleDinner(int day)
    {
        MealDataManager.Instance.ToggleDinner(
            CurrentYear,
            CurrentMonth,
            day);

        Refresh();
    }

    public DayData GetDay(int day)
    {
        return MealDataManager.Instance.GetDay(
            CurrentYear,
            CurrentMonth,
            day);
    }

    #endregion
}