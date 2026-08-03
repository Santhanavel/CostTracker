using UnityEngine;
using System.Collections.Generic;

namespace FoodTracker.Managers
{
    public enum PageType
    {
        Home,
        Onboarding,
        DayDetails,
        Calendar,
        Statistics,
        Settings,
        Splash
    }

    public class NavigationManager : MonoBehaviour
    {
        public static NavigationManager Instance { get; private set; }

        [Header("Pages")]
        [SerializeField] private List<GameObject> pages = new List<GameObject>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void NavigateTo(PageType pageType)
        {
            string targetName = GetPageName(pageType);
            Debug.Log($"Navigating to PageType: {pageType} ({targetName})");
            bool pageFound = false;

            // Deactivate Splash Page if moving to another screen
            if (pageType != PageType.Splash)
            {
                GameObject splash = GameObject.Find("Splash Page");
                if (splash != null) splash.SetActive(false);
            }

            foreach (var page in pages)
            {
                if (page != null)
                {
                    bool match = page.name.Equals(targetName, System.StringComparison.OrdinalIgnoreCase);
                    page.SetActive(match);
                    if (match)
                    {
                        pageFound = true;
                    }
                }
            }

            if (!pageFound)
            {
                Debug.LogWarning($"Page for type '{pageType}' (name: '{targetName}') not found or registered in NavigationManager.");
            }
        }

        public void RegisterPage(GameObject page)
        {
            if (page != null && !pages.Contains(page))
            {
                pages.Add(page);
            }
        }

        private string GetPageName(PageType pageType)
        {
            switch (pageType)
            {
                case PageType.Home:
                    return "Meal update page";
                case PageType.Onboarding:
                    return "Onboarding Page";
                case PageType.DayDetails:
                    return "Day Details Page";
                case PageType.Calendar:
                    return "Calender Page";
                case PageType.Statistics:
                    return "Statistics Page";
                case PageType.Settings:
                    return "Settings Page";
                case PageType.Splash:
                    return "Splash Page";
                default:
                    return "";
            }
        }
    }
}
