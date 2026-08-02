using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    [Header("Day Colors")]
    [SerializeField] private Color todayColor = new Color(0.25f, 0.75f, 0.35f);
    [SerializeField] private Color pastColor = new Color(0.75f, 0.75f, 0.75f);
    [SerializeField] private Color futureColor = Color.white;
    [SerializeField] private Color emptyCellColor = new Color(0.55f, 0.55f, 0.55f, 0.45f);
    private readonly string[] dayNames =
    {
        "Mon",
        "Tue",
        "Wed",
        "Thu",
        "Fri",
        "Sat",
        "Sun"
    };

    private readonly List<DateCell> dateCells = new List<DateCell>();

    private bool initialized;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (initialized)
            return;

        initialized = true;

        GenerateDayHeaders();
        CreateDateCells();
    }

    private void GenerateDayHeaders()
    {
        foreach (Transform child in dayParent)
            Destroy(child.gameObject);

        foreach (string day in dayNames)
        {
            GameObject obj = Instantiate(dayPrefab, dayParent);

            TMP_Text text = obj.GetComponentInChildren<TMP_Text>();

            if (text != null)
                text.text = day;
        }
    }

    private void CreateDateCells()
    {
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

    public void GenerateCalendar(MonthData monthData)
    {
        DateTime firstDay = new DateTime(monthData.year, monthData.month, 1);

        monthYearText.text = firstDay.ToString("MMMM yyyy");

        int daysInMonth = DateTime.DaysInMonth(monthData.year, monthData.month);
        // Monday = 0
        int startIndex = ((int)firstDay.DayOfWeek + 6) % 7;

        foreach (DateCell cell in dateCells)
        {
            cell.SetEmpty(emptyCellColor);
        }

        // Fill month
        for (int day = 1; day <= daysInMonth; day++)
        {
            int index = startIndex + day - 1;

            DateTime cellDate = new DateTime(
                monthData.year,
                monthData.month,
                day);

            Color color = GetDayColor(cellDate);

            DateCell cell = dateCells[index];

            cell.gameObject.SetActive(true);

            cell.SetState(
                color,
                day.ToString()
            );

            // Future:
            // cell.Initialize(monthData.GetDay(day));
        }
    }

    private Color GetDayColor(DateTime date)
    {
        DateTime today = DateTime.Today;

        if (date.Date == today)
            return todayColor;

        if (date.Date < today)
            return pastColor;

        return futureColor;
    }
}