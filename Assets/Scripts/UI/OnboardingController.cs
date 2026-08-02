using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using FoodTracker.Managers;
using FoodTracker.Persistence;

namespace FoodTracker.UI
{
    public class OnboardingController : MonoBehaviour
    {
        [Header("Slides")]
        [SerializeField] private List<GameObject> slides = new List<GameObject>();

        [Header("Dot Indicators")]
        [SerializeField] private List<Image> dots = new List<Image>();
        [SerializeField] private Sprite activeDotSprite;
        [SerializeField] private Sprite inactiveDotSprite;
        [SerializeField] private Color activeDotColor = Color.white;
        [SerializeField] private Color inactiveDotColor = new Color(1, 1, 1, 0.4f);

        [Header("Navigation Buttons")]
        [SerializeField] private Button nextButton;
        [SerializeField] private Button skipButton;
        [SerializeField] private Button getStartedButton;

        [Header("Target Screen")]
        [SerializeField] private string targetScreenName = "Meal update page";

        private int currentSlideIndex = 0;

        private void Start()
        {
            if (nextButton != null) nextButton.onClick.AddListener(OnNextClicked);
            if (skipButton != null) skipButton.onClick.AddListener(OnSkipClicked);
            if (getStartedButton != null) getStartedButton.onClick.AddListener(OnGetStartedClicked);

            UpdateSlidesDisplay();
        }

        private void OnNextClicked()
        {
            if (currentSlideIndex < slides.Count - 1)
            {
                currentSlideIndex++;
                UpdateSlidesDisplay();
            }
        }

        private void OnSkipClicked()
        {
            CompleteOnboarding();
        }

        private void OnGetStartedClicked()
        {
            CompleteOnboarding();
        }

        private void CompleteOnboarding()
        {
            if (SaveManager.Instance != null && SaveManager.Instance.AppData != null)
            {
                SaveManager.Instance.AppData.firstLaunch = false;
                SaveManager.Instance.Save();
            }

            if (NavigationManager.Instance != null)
            {
                NavigationManager.Instance.NavigateTo(targetScreenName);
            }
        }

        private void UpdateSlidesDisplay()
        {
            // Activate current slide, deactivate others
            for (int i = 0; i < slides.Count; i++)
            {
                if (slides[i] != null)
                {
                    slides[i].SetActive(i == currentSlideIndex);
                }
            }

            // Update pagination dots
            for (int i = 0; i < dots.Count; i++)
            {
                if (dots[i] != null)
                {
                    dots[i].color = (i == currentSlideIndex) ? activeDotColor : inactiveDotColor;
                    if (activeDotSprite != null && inactiveDotSprite != null)
                    {
                        dots[i].sprite = (i == currentSlideIndex) ? activeDotSprite : inactiveDotSprite;
                    }
                }
            }

            // Show/hide navigation buttons
            bool isLastSlide = (currentSlideIndex == slides.Count - 1);
            if (nextButton != null) nextButton.gameObject.SetActive(!isLastSlide);
            if (skipButton != null) skipButton.gameObject.SetActive(!isLastSlide);
            if (getStartedButton != null) getStartedButton.gameObject.SetActive(isLastSlide);
        }
    }
}
