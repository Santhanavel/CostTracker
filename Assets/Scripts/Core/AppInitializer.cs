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
            // 1. Ensure SaveManager initialized
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.Load();
            }

            // 2. Wait splash duration
            yield return new WaitForSeconds(splashDuration);

            // 3. Inspect AppData settings
            if (SaveManager.Instance != null && SaveManager.Instance.AppData != null)
            {
                bool isFirstLaunch = SaveManager.Instance.AppData.firstLaunch;
                if (isFirstLaunch)
                {
                    NavigateTo(onboardingScreenName);
                }
                else
                {
                    NavigateTo(homeScreenName);
                }
            }
            else
            {
                // Fallback to onboarding if manager is missing
                NavigateTo(onboardingScreenName);
            }
        }

        private void NavigateTo(string pageName)
        {
            if (NavigationManager.Instance != null)
            {
                NavigationManager.Instance.NavigateTo(pageName);
            }
            else
            {
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
