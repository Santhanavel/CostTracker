using System.IO;
using UnityEngine;

public class MealDataManager : MonoBehaviour
{
    public static MealDataManager Instance;

    private CalendarDatabase database;

    private string savePath;

    public CalendarDatabase Database => database;
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

    public void SetLunch(int year, int month, int day, bool value)
    {
        DayData data = GetDay(year, month, day);

        data.lunch = value;

        Save();
    }

    public void SetDinner(int year, int month, int day, bool value)
    {
        DayData data = GetDay(year, month, day);

        data.dinner = value;

        Save();
    }

    public void ToggleLunch(int year, int month, int day)
    {
        DayData data = GetDay(year, month, day);

        data.lunch = !data.lunch;

        Save();
    }

    public void ToggleDinner(int year, int month, int day)
    {
        DayData data = GetDay(year, month, day);

        data.dinner = !data.dinner;

        Save();
    }

    #endregion

    #region Summary

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