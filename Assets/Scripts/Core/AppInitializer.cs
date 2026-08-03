using UnityEngine;
using System.Collections;
using FoodTracker.Persistence;
using FoodTracker.Managers;

namespace FoodTracker.Core
{
    public class AppInitializer : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float splashDuration = 2.0f;
        [SerializeField] private string onboardingScreenName = "Onboarding Page";
        [SerializeField] private string homeScreenName = "Meal update page";

        private IEnumerator Start()
        {
            // Ensure NotificationManager is present
            if (FindAnyObjectByType<NotificationManager>() == null)
            {
                GameObject nMgr = new GameObject("NotificationManager", typeof(NotificationManager));
                DontDestroyOnLoad(nMgr);
            }

            // 1. Ensure SaveManager initialized
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.Load();
            }

            if (NotificationManager.Instance != null)
            {
                NotificationManager.Instance.ScheduleAllNotifications();
            }

            // 2. Wait splash duration
            yield return new WaitForSeconds(splashDuration);

            // 3. Inspect AppData settings
            if (SaveManager.Instance != null && SaveManager.Instance.AppData != null)
            {
                bool isFirstLaunch = SaveManager.Instance.AppData.firstLaunch;
                if (isFirstLaunch)
                {
                    NavigateTo(PageType.Onboarding);
                }
                else
                {
                    NavigateTo(PageType.Home);
                }
            }
            else
            {
                // Fallback to onboarding if manager is missing
                NavigateTo(PageType.Onboarding);
            }
        }

        private void NavigateTo(PageType pageType)
        {
            if (NavigationManager.Instance != null)
            {
                NavigationManager.Instance.NavigateTo(pageType);
            }
            else
            {
                string pageName = pageType == PageType.Onboarding ? "Onboarding Page" : "Meal update page";
                GameObject target = GameObject.Find(pageName);
                if (target != null)
                {
                    target.SetActive(true);
                    
                    // Find and deactivate Splash Page
                    GameObject splash = GameObject.Find("Splash Page");
                    if (splash != null) splash.SetActive(false);
                }
            }
        }
    }
}
