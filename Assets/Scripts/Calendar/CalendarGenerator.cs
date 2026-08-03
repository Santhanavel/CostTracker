using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using FoodTracker.UI;
using FoodTracker.Persistence;
using FoodTracker.Data;

namespace FoodTracker.Calendar
{
    public class CalendarGenerator : MonoBehaviour
    {
        [Header("Parents")]
        [SerializeField] private Transform dayParent;
        [SerializeField] private Transform dateParent;

        [Header("Prefabs")]
        [SerializeField] private GameObject dayPrefab;
        [SerializeField] private GameObject datePrefab;

        [Header("Header")]
        [SerializeField] private TMP_Text monthYearText;

        [Header("Day Cell State Colors")]
        [SerializeField] private Color activeCellColor = new Color(0.09f, 0.196f, 0.161f, 1.0f);   // `#173229` Secondary Card
        [SerializeField] private Color futureCellColor = new Color(0.09f, 0.196f, 0.161f, 0.4f);   // 40% opacity cell
        [SerializeField] private Color emptyCellColor = new Color(0.027f, 0.102f, 0.09f, 0.2f);     // Translucent `#071A17`
        [SerializeField] private Color selectedBorderColor = new Color(0.18f, 0.8f, 0.443f, 1.0f); // `#2ECC71` Selected glow border

        private readonly string[] dayNames = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
        private readonly List<DateCell> dateCells = new List<DateCell>();
        private bool initialized;

        private void Awake()
        {
            SelfWire();
            Initialize();
        }

        private void SelfWire()
        {
            if (dayParent == null) dayParent = transform.Find("Content Container/Header Card/Days Grid");
            if (dateParent == null) dateParent = transform.Find("Content Container/Dates Grid");
            if (monthYearText == null) monthYearText = transform.Find("Content Container/Header Card/Month Navigation Row/Title Wrap/Title Text")?.GetComponent<TMP_Text>();
        }

        private void Initialize()
        {
            if (initialized) return;
            initialized = true;

            SelfWire();
            GenerateDayHeaders();
            CreateDateCells();
        }

        private void GenerateDayHeaders()
        {
            if (dayParent == null) return;

            foreach (Transform child in dayParent)
                Destroy(child.gameObject);

            foreach (string day in dayNames)
            {
                GameObject obj = null;
                if (dayPrefab != null)
                {
                    obj = Instantiate(dayPrefab, dayParent);
                }
                else
                {
                    obj = new GameObject("DayHeader", typeof(RectTransform));
                    obj.transform.SetParent(dayParent, false);
                    obj.AddComponent<TextMeshProUGUI>();
                }

                TMP_Text text = obj.GetComponentInChildren<TMP_Text>();
                if (text != null)
                {
                    text.text = day;
                    text.color = new Color(0.655f, 0.718f, 0.694f, 1.0f); // Secondary light green `#A7B7B1`
                    text.fontStyle = FontStyles.Bold;
                    text.alignment = TextAlignmentOptions.Center;
                    text.fontSize = 18;
                }
            }
        }

        private void CreateDateCells()
        {
            if (dateParent == null || datePrefab == null) return;

            dateCells.Clear();
            foreach (Transform child in dateParent)
                Destroy(child.gameObject);

            for (int i = 0; i < 42; i++)
            {
                GameObject obj = Instantiate(datePrefab, dateParent);
                DateCell cell = obj.GetComponent<DateCell>();
                if (cell == null)
                {
                    Debug.LogError("Date Prefab is missing DateCell component.");
                    continue;
                }
                dateCells.Add(cell);
            }
        }

        public void GenerateCalendar(int year, int month, Action<DateTime> onDaySelected)
        {
            Initialize();

            DateTime firstDay = new DateTime(year, month, 1);
            if (monthYearText != null)
            {
                monthYearText.text = firstDay.ToString("MMMM yyyy");
            }

            int daysInMonth = DateTime.DaysInMonth(year, month);
            int startIndex = ((int)firstDay.DayOfWeek + 6) % 7; // Mon = 0, Sun = 6

            string selStr = PlayerPrefs.GetString("SelectedCalendarDate", "");
            DateTime selectedDate;
            if (!DateTime.TryParse(selStr, out selectedDate))
            {
                selectedDate = DateTime.Today;
            }

            // Fill previous month dates (faded opacity)
            DateTime prevMonth = firstDay.AddMonths(-1);
            int daysInPrevMonth = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);
            for (int i = 0; i < startIndex; i++)
            {
                int dayNum = daysInPrevMonth - (startIndex - 1 - i);
                DateTime cellDate = new DateTime(prevMonth.Year, prevMonth.Month, dayNum);
                DateCell cell = dateCells[i];
                cell.gameObject.SetActive(true);

                bool bComp = false, lComp = false, dComp = false;
                GetMealCompletionStates(cellDate, out bComp, out lComp, out dComp);

                cell.Setup(
                    cellDate,
                    cellDate.Date > DateTime.Today.Date,
                    false,
                    selectedBorderColor,
                    bComp,
                    lComp,
                    dComp,
                    true, // Faded month
                    onDaySelected
                );
            }

            // Fill current month days
            for (int day = 1; day <= daysInMonth; day++)
            {
                int index = startIndex + day - 1;
                if (index >= dateCells.Count) break;

                DateTime cellDate = new DateTime(year, month, day);
                DateCell cell = dateCells[index];

                bool isFuture = cellDate.Date > DateTime.Today.Date;
                bool isSelected = cellDate.Date == selectedDate.Date;

                bool bComp = false, lComp = false, dComp = false;
                GetMealCompletionStates(cellDate, out bComp, out lComp, out dComp);

                cell.gameObject.SetActive(true);
                cell.Setup(
                    cellDate,
                    isFuture,
                    isSelected,
                    selectedBorderColor,
                    bComp,
                    lComp,
                    dComp,
                    false, // Active month
                    onDaySelected
                );
            }

            // Fill next month dates (faded opacity)
            DateTime nextMonth = firstDay.AddMonths(1);
            int totalLogged = startIndex + daysInMonth;
            for (int i = totalLogged; i < 42; i++)
            {
                int dayNum = i - totalLogged + 1;
                DateTime cellDate = new DateTime(nextMonth.Year, nextMonth.Month, dayNum);
                DateCell cell = dateCells[i];
                cell.gameObject.SetActive(true);

                bool bComp = false, lComp = false, dComp = false;
                GetMealCompletionStates(cellDate, out bComp, out lComp, out dComp);

                cell.Setup(
                    cellDate,
                    cellDate.Date > DateTime.Today.Date,
                    false,
                    selectedBorderColor,
                    bComp,
                    lComp,
                    dComp,
                    true, // Faded month
                    onDaySelected
                );
            }
        }

        private void GetMealCompletionStates(DateTime date, out bool breakfast, out bool lunch, out bool dinner)
        {
            breakfast = false;
            lunch = false;
            dinner = false;

            if (SaveManager.Instance == null || SaveManager.Instance.AppData == null) return;

            string dateStr = date.ToString("yyyy-MM-dd");
            MealRecord record = SaveManager.Instance.AppData.mealRecords.Find(r => r.dateString == dateStr);

            if (record != null)
            {
                breakfast = record.breakfastCompleted;
                lunch = record.lunchCompleted;
                dinner = record.dinnerCompleted;
            }
        }

        private bool HasMealRecord(DateTime date)
        {
            if (SaveManager.Instance == null || SaveManager.Instance.AppData == null) return false;
            string dateStr = date.ToString("yyyy-MM-dd");
            return SaveManager.Instance.AppData.mealRecords.Exists(r => r.dateString == dateStr);
        }

        private int GetDayCompletedCount(DateTime date)
        {
            if (SaveManager.Instance == null || SaveManager.Instance.AppData == null) return 0;

            string dateStr = date.ToString("yyyy-MM-dd");
            MealRecord record = SaveManager.Instance.AppData.mealRecords.Find(r => r.dateString == dateStr);

            if (record == null) return 0;

            int completed = 0;
            if (record.lunchCompleted) completed++;
            if (record.dinnerCompleted) completed++;

            return completed;
        }
    }
}