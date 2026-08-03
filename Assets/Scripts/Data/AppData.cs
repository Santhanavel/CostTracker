using System;
using System.Collections.Generic;

namespace FoodTracker.Data
{
    [Serializable]
    public class AppData
    {
        public bool firstLaunch = true;
        public ProfileData profile = new ProfileData();
        public SettingsData settings = new SettingsData();
        public List<MealRecord> mealRecords = new List<MealRecord>();
        public List<WeightRecord> weightRecords = new List<WeightRecord>();
        public List<ReminderData> reminderRecords = new List<ReminderData>();
    }

    [Serializable]
    public class ProfileData
    {
        public string name = "Santhosh";
        public string email = "santhosh@example.com";
        public float height = 170f;
        public string activityLevel = "Moderate";
        public string photoPath = "";
    }

    [Serializable]
    public class SettingsData
    {
        public float mealCost = 50f;
        public float breakfastCost = 50f;
        public float lunchCost = 50f;
        public float dinnerCost = 50f;
        public string currency = "INR";
        public string startWeek = "Monday";
        public string theme = "System";
        public bool darkMode = true;
    }

    [Serializable]
    public class MealRecord
    {
        public string dateString; // "yyyy-MM-dd"
        public bool breakfastCompleted;
        public string breakfastTime = "";
        public bool lunchCompleted;
        public string lunchTime = "";
        public bool dinnerCompleted;
        public string dinnerTime = "";
        public string note = "";
    }

    [Serializable]
    public class WeightRecord
    {
        public string dateString;
        public float weight;
    }

    [Serializable]
    public class ReminderData
    {
        public string id;
        public string mealType;
        public string timeString; // "hh:mm tt"
        public bool repeatDaily;
        public string message;
        public bool enabled = true;
    }
}
