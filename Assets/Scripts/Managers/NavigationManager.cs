using UnityEngine;
using System.Collections.Generic;

namespace FoodTracker.Managers
{
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

        public void NavigateTo(string pageName)
        {
            Debug.Log($"Navigating to page: {pageName}");
            bool pageFound = false;

            foreach (var page in pages)
            {
                if (page != null)
                {
                    bool match = page.name.Equals(pageName, System.StringComparison.OrdinalIgnoreCase);
                    page.SetActive(match);
                    if (match)
                    {
                        pageFound = true;
                    }
                }
            }

            if (!pageFound)
            {
                Debug.LogWarning($"Page '{pageName}' not found or registered in NavigationManager.");
            }
        }

        public void RegisterPage(GameObject page)
        {
            if (page != null && !pages.Contains(page))
            {
                pages.Add(page);
            }
        }
    }
}
