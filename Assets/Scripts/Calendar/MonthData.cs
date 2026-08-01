using System;
using System.Collections.Generic;

[Serializable]
public class MonthData
{
    public int year;
    public int month;

    public List<DayData> days = new List<DayData>();

    public MonthData(int year, int month)
    {
        this.year = year;
        this.month = month;

        int totalDays = DateTime.DaysInMonth(year, month);

        for (int i = 1; i <= totalDays; i++)
        {
            days.Add(new DayData(i));
        }
    }

    public DayData GetDay(int day)
    {
        return days.Find(d => d.day == day);
    }
}
