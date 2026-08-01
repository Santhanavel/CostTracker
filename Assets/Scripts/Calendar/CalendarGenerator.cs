using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    [Header("Starting Date")]
    [SerializeField] private bool useCurrentDate = true;
    [SerializeField] private int month = 8;
    [SerializeField] private int year = 2026;

    private DateTime currentDate;

    private readonly string[] dayNames =
    {
        "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"
    };

    private readonly List<TMP_Text> dateTexts = new List<TMP_Text>();

    private void Start()
    {
        if (useCurrentDate)
            currentDate = DateTime.Today;
        else
            currentDate = new DateTime(year, month, 1);

        GenerateDayHeaders();
        CreateDateCells();

        RefreshCalendar();
    }

    //==================================================
    // Create Week Headers Once
    //==================================================
    private void GenerateDayHeaders()
    {
        foreach (string day in dayNames)
        {
            GameObject obj = Instantiate(dayPrefab, dayParent);

            TMP_Text txt = obj.GetComponentInChildren<TMP_Text>();

            if (txt != null)
                txt.text = day;
        }
    }

    //==================================================
    // Create 42 Cells Once
    //==================================================
    private void CreateDateCells()
    {
        dateTexts.Clear();

        for (int i = 0; i < 42; i++)
        {
            GameObject obj = Instantiate(datePrefab, dateParent);

            TMP_Text txt = obj.GetComponentInChildren<TMP_Text>();

            dateTexts.Add(txt);
        }
    }

    //==================================================
    // Refresh Calendar
    //==================================================
    public void RefreshCalendar()
    {
        monthYearText.text = currentDate.ToString("MMMM yyyy");

        DateTime firstDay = new DateTime(currentDate.Year, currentDate.Month, 1);

        int daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

        // Monday = 0
        int startIndex = ((int)firstDay.DayOfWeek + 6) % 7;

        // Clear all cells
        foreach (TMP_Text txt in dateTexts)
        {
            txt.text = "";
            txt.transform.parent.gameObject.SetActive(false);
        }

        // Fill dates
        for (int day = 1; day <= daysInMonth; day++)
        {
            int index = startIndex + day - 1;

            dateTexts[index].transform.parent.gameObject.SetActive(true);
            dateTexts[index].text = day.ToString();
        }
    }

    //==================================================
    // Buttons
    //==================================================
    public void NextMonth()
    {
        currentDate = currentDate.AddMonths(1);
        RefreshCalendar();
    }

    public void PreviousMonth()
    {
        currentDate = currentDate.AddMonths(-1);
        RefreshCalendar();
    }

    public void GoToToday()
    {
        currentDate = DateTime.Today;
        RefreshCalendar();
    }

    public void SetDate(int year, int month)
    {
        currentDate = new DateTime(year, month, 1);
        RefreshCalendar();
    }
}