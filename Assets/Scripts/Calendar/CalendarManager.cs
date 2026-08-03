using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FoodTracker.Calendar;
using FoodTracker.Persistence;
using FoodTracker.Data;
using FoodTracker.Managers;

namespace FoodTracker.UI
{
    public class CalendarManager : MonoBehaviour
    {
        public static CalendarManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private CalendarGenerator calendarGenerator;
        [SerializeField] private MonthlySummary monthlySummary;

        [Header("Completion Rate UI References")]
        [SerializeField] private TMP_Text completionPercentageTxt;
        [SerializeField] private Image circularRingImage; // Radial 360 fill Image
        [SerializeField] private TMP_Text mealsRatioTxt;
        [SerializeField] private Image horizontalProgressBarImage; // Horizontal fill Image
        [SerializeField] private TMP_Text remainingMealsTxt;

        [Header("Month Navigation Buttons")]
        [SerializeField] private Button prevMonthBtn;
        [SerializeField] private Button nextMonthBtn;

        [Header("Bottom Nav Buttons")]
        [SerializeField] private Button navHomeButton;
        [SerializeField] private Button navCalendarButton;
        [SerializeField] private Button navStatsButton;
        [SerializeField] private Button navMoreButton;

        [Header("Settings")]
        [SerializeField] private bool startWithCurrentDate = true;

        private DateTime currentDate;

        public int CurrentYear => currentDate.Year;
        public int CurrentMonth => currentDate.Month;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
                return;
            }

            SelfWire();
        }

        private void SelfWire()
        {
            if (calendarGenerator == null) calendarGenerator = GetComponent<CalendarGenerator>();
            if (monthlySummary == null) monthlySummary = transform.Find("Content Container/Monthly Summary Panel")?.GetComponent<MonthlySummary>();
            
            if (completionPercentageTxt == null) completionPercentageTxt = transform.Find("Content Container/Completion Rate Panel/Circle Wrap/Text")?.GetComponent<TMP_Text>();
            if (circularRingImage == null) circularRingImage = transform.Find("Content Container/Completion Rate Panel/Circle Wrap/Fill")?.GetComponent<Image>();
            if (mealsRatioTxt == null) mealsRatioTxt = transform.Find("Content Container/Completion Rate Panel/Details Wrap/Ratio Label")?.GetComponent<TMP_Text>();
            if (horizontalProgressBarImage == null) horizontalProgressBarImage = transform.Find("Content Container/Completion Rate Panel/Details Wrap/Progress Track/Fill")?.GetComponent<Image>();
            if (remainingMealsTxt == null) remainingMealsTxt = transform.Find("Content Container/Completion Rate Panel/Details Wrap/Subtext")?.GetComponent<TMP_Text>();

            if (prevMonthBtn == null) prevMonthBtn = transform.Find("Content Container/Header Card/Month Navigation Row/Prev Button")?.GetComponent<Button>();
            if (nextMonthBtn == null) nextMonthBtn = transform.Find("Content Container/Header Card/Month Navigation Row/Next Button")?.GetComponent<Button>();

            // Self-wire bottom navigation buttons dynamically at runtime
            Transform bottomPanel = transform.Find("Bottom");
            if (bottomPanel != null)
            {
                if (navHomeButton == null) navHomeButton = bottomPanel.Find("Home Tab Button")?.GetComponent<Button>();
                if (navCalendarButton == null) navCalendarButton = bottomPanel.Find("Calendar Tab Button")?.GetComponent<Button>();
                if (navStatsButton == null) navStatsButton = bottomPanel.Find("Stats Tab Button")?.GetComponent<Button>();
                if (navMoreButton == null) navMoreButton = bottomPanel.Find("More Tab Button")?.GetComponent<Button>();
            }
        }

        private void Start()
        {
            if (startWithCurrentDate)
                currentDate = DateTime.Today;
            else
                currentDate = new DateTime(2026, 8, 1);

            Color greenColor = new Color(0.18f, 0.8f, 0.443f, 1.0f); // `#2ECC71`
            Color yellowColor = new Color(1.0f, 0.757f, 0.027f, 1.0f); // `#FFC107`
            Color redColor = new Color(1.0f, 0.357f, 0.357f, 1.0f); // `#FF5B5B`
            Color grayColor = new Color(0.655f, 0.718f, 0.694f, 0.5f); // `#A7B7B1`

            SelfWire();

            if (prevMonthBtn != null) prevMonthBtn.onClick.AddListener(PreviousMonth);
            if (nextMonthBtn != null) nextMonthBtn.onClick.AddListener(NextMonth);

            if (navHomeButton != null) navHomeButton.onClick.AddListener(() => NavigateTo(PageType.Home));
            if (navCalendarButton != null) navCalendarButton.onClick.AddListener(() => NavigateTo(PageType.Calendar));
            if (navStatsButton != null) navStatsButton.onClick.AddListener(() => NavigateTo(PageType.Statistics));
            if (navMoreButton != null) navMoreButton.onClick.AddListener(() => NavigateTo(PageType.Settings));

            Refresh();
        }

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

        public void Refresh()
        {
            if (calendarGenerator != null)
            {
                calendarGenerator.GenerateCalendar(CurrentYear, CurrentMonth, OnDaySelected);
            }

            UpdateSummary();
        }

        public void UpdateSummary()
        {
            if (SaveManager.Instance == null || SaveManager.Instance.AppData == null || monthlySummary == null) return;

            AppData appData = SaveManager.Instance.AppData;
            float mealCost = appData.settings.mealCost;

            int breakfastCount = 0;
            int lunchCount = 0;
            int dinnerCount = 0;
            int totalMealsCount = 0;

            foreach (var record in appData.mealRecords)
            {
                if (DateTime.TryParse(record.dateString, out DateTime date))
                {
                    if (date.Year == CurrentYear && date.Month == CurrentMonth)
                    {
                        if (record.breakfastCompleted) { breakfastCount++; totalMealsCount++; }
                        if (record.lunchCompleted) { lunchCount++; totalMealsCount++; }
                        if (record.dinnerCompleted) { dinnerCount++; totalMealsCount++; }
                    }
                }
            }

            int cost = Mathf.RoundToInt(totalMealsCount * mealCost);

            // Update standard summary cards
            monthlySummary.UpdateSummary(
                CurrentYear,
                CurrentMonth,
                breakfastCount,
                lunchCount,
                dinnerCount,
                totalMealsCount,
                cost
            );

            // Calculate Completion Rate Metrics (using Lunch & Dinner for calculations as per image specs)
            int daysInMonth = DateTime.DaysInMonth(CurrentYear, CurrentMonth);
            int totalPossibleLunchDinner = daysInMonth * 2;
            int loggedLunchDinner = 0;

            foreach (var record in appData.mealRecords)
            {
                if (DateTime.TryParse(record.dateString, out DateTime date))
                {
                    if (date.Year == CurrentYear && date.Month == CurrentMonth)
                    {
                        if (record.lunchCompleted) loggedLunchDinner++;
                        if (record.dinnerCompleted) loggedLunchDinner++;
                    }
                }
            }

            float completionFraction = totalPossibleLunchDinner > 0 ? (float)loggedLunchDinner / totalPossibleLunchDinner : 0f;
            int percentage = Mathf.RoundToInt(completionFraction * 100f);
            int remaining = Mathf.Max(0, totalPossibleLunchDinner - loggedLunchDinner);

            if (completionPercentageTxt != null)
            {
                completionPercentageTxt.text = $"{percentage}%";
            }
            if (circularRingImage != null)
            {
                circularRingImage.type = Image.Type.Filled;
                circularRingImage.fillMethod = Image.FillMethod.Radial360;
                circularRingImage.fillAmount = completionFraction;
            }
            if (mealsRatioTxt != null)
            {
                mealsRatioTxt.text = $"{loggedLunchDinner} / {totalPossibleLunchDinner} Meals Completed";
            }
            if (horizontalProgressBarImage != null)
            {
                horizontalProgressBarImage.type = Image.Type.Filled;
                horizontalProgressBarImage.fillMethod = Image.FillMethod.Horizontal;
                horizontalProgressBarImage.fillAmount = completionFraction;
            }
            if (remainingMealsTxt != null)
            {
                remainingMealsTxt.text = $"{remaining} Meals Remaining";
            }
        }

        private void OnDaySelected(DateTime selectedDate)
        {
            Debug.Log($"Day selected: {selectedDate:yyyy-MM-dd}");
            PlayerPrefs.SetString("SelectedCalendarDate", selectedDate.ToString("yyyy-MM-dd"));
            
            if (calendarGenerator != null)
            {
                calendarGenerator.GenerateCalendar(CurrentYear, CurrentMonth, OnDaySelected);
            }

            if (NavigationManager.Instance != null)
            {
                NavigationManager.Instance.NavigateTo(PageType.DayDetails);
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