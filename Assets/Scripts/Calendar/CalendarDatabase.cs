using System;
using System.Collections.Generic;

[Serializable]
public class CalendarDatabase
{
    public List<MonthData> months = new List<MonthData>();

    public bool trackBreakfast = true;
    public bool trackLunch = true;
    public bool trackDinner = true;
    public bool isOnboardingCompleted = false;
}
