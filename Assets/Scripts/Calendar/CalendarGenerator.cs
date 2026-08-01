using System;
using UnityEngine;
using TMPro;

public class CalendarGenerator : MonoBehaviour
{
    [Header("Header")]
    [SerializeField] private TMP_Text monthYearText;

    [Header("Parents")]
    public Transform dayParent;
    public Transform dateParent;

    [Header("Prefabs")]
    public GameObject dayPrefab;
    public GameObject datePrefab;

    [Header("Calendar")]
    public int month = 8;
    public int year = 2026;

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

    private void Start()
    {
        GenerateCalendar();
    }

    public void GenerateCalendar()
    {
        ClearChildren(dayParent);
        ClearChildren(dateParent);

        UpdateHeader();
        GenerateDayHeaders();
        GenerateDates();
    }

    private void UpdateHeader()
    {
        DateTime date = new DateTime(year, month, 1);
        monthYearText.text = date.ToString("MMMM yyyy");

        // Examples:
        // August 2026
        // September 2026
        // January 2027
    }

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

    private void GenerateDates()
    {
        DateTime firstDay = new DateTime(year, month, 1);

        int daysInMonth = DateTime.DaysInMonth(year, month);

        // Monday = 0
        int startIndex = ((int)firstDay.DayOfWeek + 6) % 7;

        // Empty cells before first day
        for (int i = 0; i < startIndex; i++)
        {
            GameObject obj = Instantiate(datePrefab, dateParent);

            TMP_Text txt = obj.GetComponentInChildren<TMP_Text>();

            if (txt != null)
                txt.text = "";
        }

        // Dates
        for (int i = 1; i <= daysInMonth; i++)
        {
            GameObject obj = Instantiate(datePrefab, dateParent);

            TMP_Text txt = obj.GetComponentInChildren<TMP_Text>();

            if (txt != null)
                txt.text = i.ToString();
        }
    }

    public void NextMonth()
    {
        month++;

        if (month > 12)
        {
            month = 1;
            year++;
        }

        GenerateCalendar();
    }

    public void PreviousMonth()
    {
        month--;

        if (month < 1)
        {
            month = 12;
            year--;
        }

        GenerateCalendar();
    }

    public void SetDate(int newMonth, int newYear)
    {
        month = Mathf.Clamp(newMonth, 1, 12);
        year = newYear;

        GenerateCalendar();
    }

    private void ClearChildren(Transform parent)
    {
        while (parent.childCount > 0)
        {
            Destroy(parent.GetChild(0).gameObject);
        }
    }
}
