using UnityEngine;
using UnityEngine.UI;
using FoodTracker.Managers;

namespace FoodTracker.UI
{
    public class SplashScreenController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button getStartedButton;

        [Header("Transition settings")]
        [SerializeField] private string targetPageName = "Onboarding Page";

        private void Start()
        {
            if (getStartedButton != null)
            {
                getStartedButton.onClick.AddListener(OnGetStartedClicked);
            }
        }

        private void OnGetStartedClicked()
        {
            if (NavigationManager.Instance != null)
            {
                NavigationManager.Instance.NavigateTo(targetPageName);
            }
            else
            {
                // Fallback direct deactivate/activate if navigation manager is not in scene yet
                GameObject target = GameObject.Find(targetPageName);
                if (target != null)
                {
                    target.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
