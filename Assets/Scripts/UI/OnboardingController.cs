using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

        [Header("Name Entry Prefab")]
        [SerializeField] private GameObject nameEntryPrefab;

        private int currentSlideIndex = 0;
        private GameObject nameInputPopup = null;

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
            PromptNameEntry();
        }

        private void OnGetStartedClicked()
        {
            PromptNameEntry();
        }

        private void PromptNameEntry()
        {
            // If the user already has a name saved, bypass name entry
            if (SaveManager.Instance != null && SaveManager.Instance.AppData != null &&
                !string.IsNullOrEmpty(SaveManager.Instance.AppData.profile.name))
            {
                CompleteOnboarding();
                return;
            }

            // Open existing instance if already created
            if (nameInputPopup != null)
            {
                nameInputPopup.SetActive(true);
                return;
            }

            Canvas canvas = FindAnyObjectByType<Canvas>();
            if (canvas == null)
            {
                CompleteOnboarding();
                return;
            }

            GameObject targetPrefab = nameEntryPrefab;
            if (targetPrefab == null)
            {
#if UNITY_EDITOR
                targetPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/NameEntryOverlay.prefab");
#endif
            }

            if (targetPrefab != null)
            {
                nameInputPopup = Instantiate(targetPrefab, canvas.transform, false);
            }
            else
            {
                CompleteOnboarding();
                return;
            }

            // Self-wire components dynamically from the instantiated prefab
            TMP_InputField inputField = nameInputPopup.GetComponentInChildren<TMP_InputField>();
            Button continueBtn = nameInputPopup.transform.Find("CardBox/Continue Button")?.GetComponent<Button>();
            Image inputBg = nameInputPopup.transform.Find("CardBox/InputField")?.GetComponent<Image>();

            if (continueBtn != null && inputField != null)
            {
                continueBtn.onClick.RemoveAllListeners();
                continueBtn.onClick.AddListener(() => {
                    string enteredName = inputField.text.Trim();
                    if (!string.IsNullOrEmpty(enteredName))
                    {
                        if (SaveManager.Instance != null && SaveManager.Instance.AppData != null)
                        {
                            SaveManager.Instance.AppData.profile.name = enteredName;
                        }
                        nameInputPopup.SetActive(false);
                        CompleteOnboarding();
                    }
                    else
                    {
                        if (inputBg != null) inputBg.color = new Color(0.9f, 0.3f, 0.3f, 1.0f);
                    }
                });
            }

            if (inputField != null)
            {
                inputField.ActivateInputField();
            }
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
                NavigationManager.Instance.NavigateTo(PageType.Home);
            }
        }

        private void UpdateSlidesDisplay()
        {
            for (int i = 0; i < slides.Count; i++)
            {
                if (slides[i] != null)
                {
                    slides[i].SetActive(i == currentSlideIndex);
                }
            }

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

            bool isLastSlide = (currentSlideIndex == slides.Count - 1);
            if (nextButton != null) nextButton.gameObject.SetActive(!isLastSlide);
            if (skipButton != null) skipButton.gameObject.SetActive(!isLastSlide);
            if (getStartedButton != null) getStartedButton.gameObject.SetActive(isLastSlide);
        }
    }
}
