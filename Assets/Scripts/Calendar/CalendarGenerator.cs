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
        [SerializeField] private Color activeCellColor = new Color(0.06f, 0.16f, 0.13f, 1.0f);   // Spruce background
        [SerializeField] private Color futureCellColor = new Color(0.04f, 0.12f, 0.10f, 0.6f);   // Muted dark background
        [SerializeField] private Color emptyCellColor = new Color(0.02f, 0.08f, 0.06f, 0.2f);     // Very dark empty
        [SerializeField] private Color selectedBorderColor = new Color(0.18f, 0.74f, 0.46f, 1.0f); // Bright green outline

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
            if (dayParent == null) dayParent = transform.Find("Center/Days Grid");
            if (dateParent == null) dateParent = transform.Find("Center/Dates Grid");
            if (monthYearText == null) monthYearText = transform.Find("Header/Title Wrap/Title Text")?.GetComponent<TMP_Text>();
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
                    text.color = new Color(0.18f, 0.74f, 0.46f, 1.0f); // Green headers
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

            // Reset all cells
            foreach (DateCell cell in dateCells)
            {
                cell.SetEmpty(emptyCellColor);
            }

            string selStr = PlayerPrefs.GetString("SelectedCalendarDate", "");
            DateTime selectedDate;
            if (!DateTime.TryParse(selStr, out selectedDate))
            {
                selectedDate = DateTime.Today;
            }

            // Fill month days
            for (int day = 1; day <= daysInMonth; day++)
            {
                int index = startIndex + day - 1;
                if (index >= dateCells.Count) break;

                DateTime cellDate = new DateTime(year, month, day);
                DateCell cell = dateCells[index];

                int completedCount = GetDayCompletedCount(cellDate);
                bool isFutureOrNoData = cellDate.Date > DateTime.Today || !HasMealRecord(cellDate);
                bool isSelected = cellDate.Date == selectedDate.Date;

                Color bgColor = isFutureOrNoData ? futureCellColor : activeCellColor;

                cell.gameObject.SetActive(true);
                cell.Setup(
                    cellDate, 
                    bgColor, 
                    isSelected, 
                    selectedBorderColor, 
                    completedCount, 
                    isFutureOrNoData, 
                    onDaySelected
                );
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