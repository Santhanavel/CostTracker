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

    private readonly string[] dayNames =
    {
        "Mon","Tue","Wed","Thu","Fri","Sat","Sun"
    };

    private readonly List<TMP_Text> dateTexts = new List<TMP_Text>();

    private bool initialized = false;

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
        foreach (string day in dayNames)
        {
            GameObject obj = Instantiate(dayPrefab, dayParent);

            TMP_Text txt = obj.GetComponentInChildren<TMP_Text>();

            if (txt != null)
                txt.text = day;
        }
    }

    private void CreateDateCells()
    {
        for (int i = 0; i < 42; i++)
        {
            GameObject obj = Instantiate(datePrefab, dateParent);

            TMP_Text txt = obj.GetComponentInChildren<TMP_Text>();

            dateTexts.Add(txt);
        }
    }

    public void GenerateCalendar(MonthData monthData)
    {
        DateTime currentDate = new DateTime(monthData.year, monthData.month, 1);

        monthYearText.text = currentDate.ToString("MMMM yyyy");

        int daysInMonth = DateTime.DaysInMonth(
            monthData.year,
            monthData.month);

        int startIndex =
            ((int)currentDate.DayOfWeek + 6) % 7;

        foreach (TMP_Text txt in dateTexts)
        {
            txt.text = "";
            txt.transform.parent.gameObject.SetActive(false);
        }

        for (int day = 1; day <= daysInMonth; day++)
        {
            int index = startIndex + day - 1;

            dateTexts[index].transform.parent.gameObject.SetActive(true);
            dateTexts[index].text = day.ToString();

            // CalendarCell will be added later
        }
    }
}