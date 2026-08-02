using System.IO;
using UnityEngine;

public class MealDataManager : MonoBehaviour
{
    public static MealDataManager Instance;

    private CalendarDatabase database;

    private string savePath;

    public CalendarDatabase Database => database;

    public bool TrackBreakfast
    {
        get => database.trackBreakfast;
        set { database.trackBreakfast = value; Save(); }
    }

    public bool TrackLunch
    {
        get => database.trackLunch;
        set { database.trackLunch = value; Save(); }
    }

    public bool TrackDinner
    {
        get => database.trackDinner;
        set { database.trackDinner = value; Save(); }
    }

    public bool IsOnboardingCompleted
    {
        get => database.isOnboardingCompleted;
        set { database.isOnboardingCompleted = value; Save(); }
    }

    [SerializeField]
    private int mealPrice = 80; 
    
    public int GetTotalCost(int year, int month)
    {
        return GetMealCount(year, month) * mealPrice;
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            savePath = Path.Combine(Application.persistentDataPath, "MealCalendar.json");

            Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region Save / Load

    public void Save()
    {
        string json = JsonUtility.ToJson(database, true);

        File.WriteAllText(savePath, json);

        Debug.Log($"Calendar Saved\n{savePath}");
    }

    public void Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);

            database = JsonUtility.FromJson<CalendarDatabase>(json);

            Debug.Log("Calendar Loaded");
        }
        else
        {
            database = new CalendarDatabase();

            Save();

            Debug.Log("New Calendar Database Created");
        }
    }

    #endregion

    #region Month

    public MonthData GetOrCreateMonth(int year, int month)
    {
        MonthData monthData = database.months.Find(m =>
            m.year == year &&
            m.month == month);

        if (monthData == null)
        {
            monthData = new MonthData(year, month);

            database.months.Add(monthData);

            Save();
        }

        return monthData;
    }

    #endregion

    #region Day

    public DayData GetDay(int year, int month, int day)
    {
        MonthData monthData = GetOrCreateMonth(year, month);

        return monthData.GetDay(day);
    }

    #endregion

    #region Toggle Meals

    public void SetBreakfast(int year, int month, int day, bool value)
    {
        DayData data = GetDay(year, month, day);

        data.breakfast = value;
        data.breakfastTime = value ? System.DateTime.Now.ToString("h:mm tt") : "";

        Save();
    }

    public void SetLunch(int year, int month, int day, bool value)
    {
        DayData data = GetDay(year, month, day);

        data.lunch = value;
        data.lunchTime = value ? System.DateTime.Now.ToString("h:mm tt") : "";

        Save();
    }

    public void SetDinner(int year, int month, int day, bool value)
    {
        DayData data = GetDay(year, month, day);

        data.dinner = value;
        data.dinnerTime = value ? System.DateTime.Now.ToString("h:mm tt") : "";

        Save();
    }

    public void ToggleBreakfast(int year, int month, int day)
    {
        DayData data = GetDay(year, month, day);
        SetBreakfast(year, month, day, !data.breakfast);
    }

    public void ToggleLunch(int year, int month, int day)
    {
        DayData data = GetDay(year, month, day);
        SetLunch(year, month, day, !data.lunch);
    }

    public void ToggleDinner(int year, int month, int day)
    {
        DayData data = GetDay(year, month, day);
        SetDinner(year, month, day, !data.dinner);
    }

    #endregion

    #region Summary

    public int GetBreakfastCount(int year, int month)
    {
        MonthData monthData = GetOrCreateMonth(year, month);

        int count = 0;

        foreach (DayData day in monthData.days)
        {
            if (day.breakfast)
                count++;
        }

        return count;
    }

    public int GetLunchCount(int year, int month)
    {
        MonthData monthData = GetOrCreateMonth(year, month);

        int count = 0;

        foreach (DayData day in monthData.days)
        {
            if (day.lunch)
                count++;
        }

        return count;
    }

    public int GetDinnerCount(int year, int month)
    {
        MonthData monthData = GetOrCreateMonth(year, month);

        int count = 0;

        foreach (DayData day in monthData.days)
        {
            if (day.dinner)
                count++;
        }

        return count;
    }

    public int GetMealCount(int year, int month)
    {
        MonthData monthData = GetOrCreateMonth(year, month);

        int count = 0;

        foreach (DayData day in monthData.days)
        {
            count += day.MealCount;
        }

        return count;
    }

    #endregion
}