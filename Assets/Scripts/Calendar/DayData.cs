using System;

[Serializable]
public class DayData
{
    public int day;

    // Breakfast
    public bool breakfast;
    public string breakfastTime;

    // Lunch
    public bool lunch;
    public string lunchTime;

    // Dinner
    public bool dinner;
    public string dinnerTime;

    public DayData(int day)
    {
        this.day = day;

        breakfast = false;
        lunch = false;
        dinner = false;

        breakfastTime = "";
        lunchTime = "";
        dinnerTime = "";
    }

    public int MealCount
    {
        get
        {
            int count = 0;

            if (breakfast)
                count++;

            if (lunch)
                count++;

            if (dinner)
                count++;

            return count;
        }
    }
}