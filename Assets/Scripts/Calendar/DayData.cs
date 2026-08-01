using System;

[Serializable]
public class DayData
{
    public int day;

    public bool lunch;
    public bool dinner;

    public DayData(int day)
    {
        this.day = day;

        lunch = false;
        dinner = false;
    }

    public int MealCount
    {
        get
        {
            int count = 0;

            if (lunch)
                count++;

            if (dinner)
                count++;

            return count;
        }
    }
}